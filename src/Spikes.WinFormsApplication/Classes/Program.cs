using System;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication
{
    public static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args != null && args.Length >0)
            {
                MainForm mainForm = new MainForm();
                mainForm.Text = args[0];
                Application.Run(mainForm);
            }
            else 
                Application.Run(new MainForm());
        }
    }
}