using System;
using System.Threading;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication
{
    public partial class UnhandledExceptionsDefaultBehaviourExample : Form
    {
        public UnhandledExceptionsDefaultBehaviourExample()
        {
            InitializeComponent();
        }

        private void mainUiThreadUheButton_Click(object sender, EventArgs e)
        {
            throw new InvalidOperationException("Unhandled exception in main UI thread");
        }

        private void workerThreadUheButton_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(GoBoom);
            thread.Start();
        }

        void GoBoom()
        {
            int a = 0;
            int b = 1;
            int c = b / a;
        }
    }
}