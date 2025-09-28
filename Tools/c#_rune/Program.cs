namespace c__rune;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Form1 form1 = new Form1();

        if (args.Length > 0)
        {
            form1.richTextBox1.Text = args[0];
            form1.richTextBox1.SelectionStart = form1.richTextBox1.Text.Length; // ƒLƒƒƒŒƒbƒg‚ð––”ö‚É
            form1.richTextBox1.Focus();
        }

        Application.Run(form1);

 
    }    
}