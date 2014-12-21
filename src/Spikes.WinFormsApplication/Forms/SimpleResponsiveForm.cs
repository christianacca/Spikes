using System;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication.Forms
{
    public partial class SimpleResponsiveForm : Form
    {
        public SimpleResponsiveForm()
        {
            InitializeComponent();
        }

        private void doUnresponsiveWorkButton_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;

            DateTime future = DateTime.Now.AddSeconds(10);
            DateTime previousTime = DateTime.Now;
            while (DateTime.Now < future)
            {
                //Note: DO NOT USE DoEvents() as per this article: http://blogs.msdn.com/jfoscoding/archive/2005/08/06/448560.aspx
                //While Application.DoEvents() would make the app appear more responsive it would cause reentrant problems
                //It also would still cause this routine to hog the cpu

                //simulate work
                if (!DateTime.Now.Second.Equals(previousTime.Second))
                {
                    if (progressBar1.Value < 100)
                    {
                        progressBar1.Value += 10;
                        progressBar1.Invalidate(); // at least repaint certain controls to give some feedback
                    }
                }
                previousTime = DateTime.Now;
            }
        }
    }
}