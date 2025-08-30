using System;
using System.Text;
using System.Collections.Generic;

namespace lib247.Unicode
{
    public static class UnicodeConverter
    {
        // 文字列から全コードポイントとUTF-16BEバイト列を取得
        public static (List<int> codePoints, byte[] utf16beBytes) GetCodePointsAndUtf16be(string s)
        {
            var codePoints = new List<int>();
            var utf16beList = new List<byte>();

            for (int i = 0; i < s.Length;)
            {
                int codePoint = char.ConvertToUtf32(s, i);
                codePoints.Add(codePoint);

                var (bytes, _) = ConvertCodePointToUtf16(codePoint);
                utf16beList.AddRange(bytes);

                i += char.IsSurrogatePair(s, i) ? 2 : 1;
            }

            return (codePoints, utf16beList.ToArray());
        }

        // コードポイント→UTF-16BEバイト列
        public static (byte[] utf16beBytes, string decoded) ConvertCodePointToUtf16(int codePoint)
        {
            byte[] utf16beBytes;
            string decoded;

            if (codePoint <= 0xFFFF)
            {
                utf16beBytes = new byte[2];
                utf16beBytes[0] = (byte)(codePoint >> 8 & 0xFF);
                utf16beBytes[1] = (byte)(codePoint & 0xFF);
                decoded = char.ConvertFromUtf32(codePoint);
            }
            else
            {
                int originalCodePoint = codePoint;
                codePoint -= 0x10000;
                int highSurrogate = 0xD800 + (codePoint >> 10 & 0x3FF);
                int lowSurrogate = 0xDC00 + (codePoint & 0x3FF);

                utf16beBytes = new byte[4];
                utf16beBytes[0] = (byte)(highSurrogate >> 8 & 0xFF);
                utf16beBytes[1] = (byte)(highSurrogate & 0xFF);
                utf16beBytes[2] = (byte)(lowSurrogate >> 8 & 0xFF);
                utf16beBytes[3] = (byte)(lowSurrogate & 0xFF);
                decoded = char.ConvertFromUtf32(originalCodePoint);
            }

            return (utf16beBytes, decoded);
        }
    }
}
