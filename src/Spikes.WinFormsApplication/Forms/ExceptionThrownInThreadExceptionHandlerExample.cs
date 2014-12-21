using System;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication
{
    public partial class ExceptionThrownInThreadExceptionHandlerExample : Form
    {
        public ExceptionThrownInThreadExceptionHandlerExample()
        {
            InitializeComponent();
        }


        private void triggerExceptionButton_Click(object sender, EventArgs e)
        {
            throw new InvalidOperationException("Unhandled exception in main UI thread");
        }


        void GoBoom()
        {
            int a = 0;
            int b = 1;
            int c = b / a;
        }

        private void ExceptionThrownInThreadExceptionHandlerExample_Load(object sender, EventArgs e)
        {

        }
    }
}