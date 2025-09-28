using System.Text;

namespace c__rune;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        richTextBox1.Font = new Font("�s�������W�����p����", 12, FontStyle.Regular);
        //txtInput.MaxLength = 1;
    }




    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {

        var runes = richTextBox1.Text.EnumerateRunes().ToArray();


        if (runes.Length > 1)
        {
            // �Ō��Rune��IVS������
            bool IsIVS(Rune rune) =>
                (rune.Value >= 0xFE00 && rune.Value <= 0xFE0F) || // �W���o���A���g
                (rune.Value >= 0xE0100 && rune.Value <= 0xE01EF);  // �⏕�o���A���g

            if (IsIVS(runes[^1]) && runes.Length >= 2)
            {
                // �Ō�Ƃ��̑O��Rune���c��
                richTextBox1.Text = runes[^2].ToString() + runes[^1].ToString();
            }
            else
            {
                // �Ō��Rune�����c��
                richTextBox1.Text = runes[^1].ToString();
            }
            richTextBox1.SelectionStart = richTextBox1.Text.Length; // �L�����b�g�𖖔���
        }

        string input = richTextBox1.Text;
        var codePoints = input.EnumerateRunes()
                              .Select(rune => $"U+{rune.Value:X4}")
                              .ToArray();


        string result = string.Join(", ", codePoints);
        //MessageBox.Show(result, "Unicode�R�[�h�|�C���g");
        txtResult.Text = result;



        var utf16Codes = input.Select(c => $"{(int)c:X4}").ToArray();
        string resultUTF16 = string.Join("", utf16Codes);
        txtResultUTF16.Text = resultUTF16;


        Console.WriteLine(result+", "+utf16Codes);
    }

}

