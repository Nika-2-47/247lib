using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OfficeOpenXml;

class Program
{
    static async Task Main()
    {
        ExcelPackage.License.SetNonCommercialPersonal("test");

        Console.Write("検索対象ディレクトリを入力してください（例: G:\\@source）: ");
        string? inputDir = Console.ReadLine();
        string targetDir = string.IsNullOrWhiteSpace(inputDir) ? @".\\" : inputDir.Trim();

        Console.Write("検索パターン（正規表現）を入力してください（例: .*）: ");
        string? inputPattern = Console.ReadLine();
        string searchPattern = string.IsNullOrWhiteSpace(inputPattern) ? ".*" : inputPattern.Trim();

        int maxDegreeOfParallelism = 16;
        var regex = new Regex(searchPattern);
        var xlsxQueue = new ConcurrentQueue<string>();

        int fileCount = 0;
        int processedCount = 0;
        var foundFiles = new ConcurrentQueue<(string FilePath, string SheetName)>();
        object consoleLock = new object();

        var startTime = DateTime.Now;

        var producer = Task.Run(() =>
        {
            foreach (var file in Directory.EnumerateFiles(targetDir, "*.xlsx", SearchOption.AllDirectories))
            {
                xlsxQueue.Enqueue(file);
                Interlocked.Increment(ref fileCount);
            }
        });

        int lastFoundCount = 0;

        var consumers = new Task[maxDegreeOfParallelism];
        for (int i = 0; i < maxDegreeOfParallelism; i++)
        {
            consumers[i] = Task.Run(async () =>
            {
                while (!producer.IsCompleted || !xlsxQueue.IsEmpty)
                {
                    if (xlsxQueue.TryDequeue(out var file))
                    {
                        var foundSheet = await ContainsTextInXlsxAsync(file, regex);
                        int current = Interlocked.Increment(ref processedCount);

                        if (!string.IsNullOrEmpty(foundSheet))
                        {
                            foundFiles.Enqueue((Path.GetFullPath(file), foundSheet));
                        }

                        lock (consoleLock)
                        {
                            int foundCount = foundFiles.Count;
                            int progressLine = GetDisplayLineCount(foundFiles);

                            if (foundCount > lastFoundCount)
                            {
                                Console.Clear();
                                foreach (var foundFile in foundFiles)
                                {
                                    WriteWrappedLineWithSheet(foundFile.FilePath, foundFile.SheetName);
                                }
                                Console.SetCursorPosition(0, progressLine);
                                Console.Write($"処理済み: {current}/{fileCount} ファイル   ");
                                lastFoundCount = foundCount;
                            }
                            else
                            {
                                Console.SetCursorPosition(0, progressLine);
                                Console.Write($"処理済み: {current}/{fileCount} ファイル   ");
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(50);
                    }
                }
            });
        }

        await producer;
        await Task.WhenAll(consumers);

        var endTime = DateTime.Now;
        var duration = (endTime - startTime).TotalSeconds;

        lock (consoleLock)
        {
            Console.Clear();
            foreach (var found in foundFiles)
            {
                WriteWrappedLineWithSheet(found.FilePath, found.SheetName);
            }
            int progressLine = GetDisplayLineCount(foundFiles);
            Console.SetCursorPosition(0, progressLine);
            Console.WriteLine($"処理済み: {processedCount}/{fileCount} ファイル");
            Console.WriteLine();
            Console.WriteLine("全てのxlsxファイルの検索が完了しました。");
            Console.WriteLine($"処理時間: {duration:F2} 秒");
        }
    }

    // ファイル名とシート名を同じ行で、シート名は必ず1行で切れずに表示
    static void WriteWrappedLineWithSheet(string filePath, string sheetName)
    {
        int width = Console.WindowWidth > 0 ? Console.WindowWidth : 80;
        string sheetSuffix = $" | シート: {sheetName}";
        int fileWidth = width - GetDisplayWidth(sheetSuffix);

        if (GetDisplayWidth(filePath) <= fileWidth)
        {
            // 1行で収まる場合
            Console.WriteLine(filePath + sheetSuffix);
        }
        else
        {
            // 複数行に分割
            int current = 0;
            int filePathLen = filePath.Length;
            while (current < filePathLen)
            {
                // 最終行だけはシート名を付ける
                int remain = filePathLen - current;
                if (GetDisplayWidth(filePath.Substring(current)) <= fileWidth)
                {
                    Console.WriteLine(filePath.Substring(current) + sheetSuffix);
                    break;
                }
                else
                {
                    // 文字幅で切る（簡易: 文字数で切る）
                    Console.WriteLine(filePath.Substring(current, fileWidth));
                    current += fileWidth;
                }
            }
        }
    }

    // 表示幅（全角対応簡易版、必要なら厳密化可）
    static int GetDisplayWidth(string s)
    {
        int width = 0;
        foreach (var c in s)
        {
            width += (c > 0xFF) ? 2 : 1;
        }
        return width;
    }

    // ファイル名の折り返しを考慮して表示行数を計算
    static int GetDisplayLineCount(ConcurrentQueue<(string FilePath, string SheetName)> foundFiles)
    {
        int width = Console.WindowWidth > 0 ? Console.WindowWidth : 80;
        int lines = 0;
        foreach (var found in foundFiles)
        {
            string sheetSuffix = $" | シート: {found.SheetName}";
            int fileWidth = width - GetDisplayWidth(sheetSuffix);
            if (GetDisplayWidth(found.FilePath) <= fileWidth)
            {
                lines += 1;
            }
            else
            {
                int remain = found.FilePath.Length;
                int current = 0;
                while (current < found.FilePath.Length)
                {
                    if (GetDisplayWidth(found.FilePath.Substring(current)) <= fileWidth)
                    {
                        lines += 1;
                        break;
                    }
                    else
                    {
                        lines += 1;
                        current += fileWidth;
                    }
                }
            }
        }
        return lines;
    }

    static async Task<string?> ContainsTextInXlsxAsync(string filePath, Regex regex)
    {
        return await Task.Run(() =>
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        var dimension = worksheet.Dimension;
                        if (dimension == null) continue;

                        var cells = worksheet.Cells[dimension.Address];
                        foreach (var cell in cells)
                        {
                            var value = cell.Value?.ToString();
                            if (!string.IsNullOrEmpty(value) && regex.IsMatch(value))
                            {
                                return worksheet.Name;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 必要に応じてエラー出力
            }
            return null;
        });
    }
}