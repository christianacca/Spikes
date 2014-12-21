using System;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication.Forms
{
    public partial class PerformanceCounterExamples : Form
    {
        public PerformanceCounterExamples()
        {
            InitializeComponent();
        }

        private void measureExceptionsButton_Click(object sender, EventArgs e)
        {
            DateTime end = DateTime.Now.AddSeconds(1);
            while(DateTime.Now < end)
            {
                try
                {
                    throw new InvalidOperationException();
                }
                catch (InvalidOperationException) { }
            }
            MessageBox.Show(string.Format("Number of exceptions thrown: {0}", performanceCounter.NextValue()),
                            string.Format("Performance counter: {0}", performanceCounter.CounterName),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
    }
}