using System;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication
{
    [Serializable]
    public class ReflectionFriendlyLauncher
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public void LaunchApp()
        {
            Run();
        }
    }
}