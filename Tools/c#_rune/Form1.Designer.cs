namespace c__rune;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        txtResult = new TextBox();
        txtResultUTF16 = new TextBox();
        richTextBox1 = new RichTextBox();
        SuspendLayout();
        // 
        // txtResult
        // 
        txtResult.Location = new Point(270, 16);
        txtResult.Multiline = true;
        txtResult.Name = "txtResult";
        txtResult.Size = new Size(216, 158);
        txtResult.TabIndex = 0;
        // 
        // txtResultUTF16
        // 
        txtResultUTF16.Location = new Point(526, 16);
        txtResultUTF16.Multiline = true;
        txtResultUTF16.Name = "txtResultUTF16";
        txtResultUTF16.Size = new Size(216, 158);
        txtResultUTF16.TabIndex = 0;
        // 
        // richTextBox1
        // 
        richTextBox1.BorderStyle = BorderStyle.None;
        richTextBox1.Location = new Point(12, 16);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.Size = new Size(216, 158);
        richTextBox1.TabIndex = 1;
        richTextBox1.Text = "";
        richTextBox1.TextChanged += richTextBox1_TextChanged;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(richTextBox1);
        Controls.Add(txtResultUTF16);
        Controls.Add(txtResult);
        Name = "Form1";
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private TextBox txtResult;
    private TextBox txtResultUTF16;
    public RichTextBox richTextBox1;
}
