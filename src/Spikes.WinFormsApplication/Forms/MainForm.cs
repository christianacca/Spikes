using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Eca.Commons.Testing;
using Eca.Spikes.WinFormsApplication.Forms;

namespace Eca.Spikes.WinFormsApplication
{
    public partial class MainForm : Form
    {
        private Form _uheForm;
        private Form _trappingUheFrom;


        public MainForm()
        {
            InitializeComponent();
        }

        private void demoUheButton_Click(object sender, EventArgs e)
        {
            //for this example we want the default behaviour from WinForms
            //ie before any event handlers have been wired up to the unhandled exception events
            UnregisterGlobalExceptionTraps();

            DisposeOfPreviousForms();

            _uheForm = new UnhandledExceptionsDefaultBehaviourExample();
            _uheForm.Show();
        }


        private void demoTrappingUheButton_Click(object sender, EventArgs e)
        {
            RegisterGlobalExceptionTraps(SafeThreadExceptionHandler);

            DisposeOfPreviousForms();

            _trappingUheFrom = new TrappingUnhandledExceptionsExample();
            _trappingUheFrom.Show();
        }


        private void DisposeOfPreviousForms() 
        {
            DisposeOfForm(ref _trappingUheFrom);
            DisposeOfForm(ref _uheForm);
        }


        private void demoExceptionInThreadExceptionHandlerButton_Click(object sender, EventArgs e)
        {
            RegisterGlobalExceptionTraps(ThreadExceptionHandlerThatThrowsException);

            DisposeOfPreviousForms();

            _trappingUheFrom = new ExceptionThrownInThreadExceptionHandlerExample();
            _trappingUheFrom.Show();
        }


        private void RegisterGlobalExceptionTraps(ThreadExceptionEventHandler handler)
        {
            Application.ThreadException += handler;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;            
        }


        private void UnregisterGlobalExceptionTraps() 
        {
            Application.ThreadException -= SafeThreadExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }


        void SafeThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {

            StringBuilder sb = new StringBuilder(589);
            sb.AppendFormat(@"* An exception on the main UI thread has occurred. {0}", Environment.NewLine);
            sb.AppendFormat(@"* It has been trapped by an event handler that has been wired up to Application.ThreadException. {0}", Environment.NewLine);
            sb.AppendFormat(@"* You would ordinarily log exception information and notify that an exception has occurred so that ops can diagnose and fix the cause ");
            sb.AppendFormat(@"of the exception.{0}", Environment.NewLine);
            sb.AppendFormat(@"* It is common convention in a WinForms app to ask the user whether the app should be closed down ");
            sb.AppendFormat(@"or allowed to continue{0}", Environment.NewLine);
            sb.AppendFormat(@"* Just because the app code has trapped this exception, it does not mean that the exception has ");
            sb.AppendFormat(@"been handled in the application code. {0}", Environment.NewLine);
            sb.AppendFormat(@"* Instead consider the exception to have been logged but left unhandled in the code.");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Do you want to continue running this demonstration?");

            DialogResult result = MessageBox.Show(sb.ToString(), "Unhandled exception occurred in main UI thread", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
                Application.Exit();
        }


        private void ThreadExceptionHandlerThatThrowsException(object sender, ThreadExceptionEventArgs e)
        {
            StringBuilder sb = new StringBuilder(318);
            sb.AppendFormat(@"* Throwing an exception in the handler wired up to Application.ThreadException ");
            sb.AppendFormat(@"will result in the exception bubbling. {0}", Environment.NewLine);
            sb.AppendFormat(@"* The bubbled up exception will be trapped by any handler wired up to ");
            sb.AppendFormat(@"AppDomain.CurrentDomain.UnhandledException event just before the app is ");
            sb.AppendFormat(@"terminated{0}", Environment.NewLine);
            sb.AppendFormat(@"{0}", Environment.NewLine);
            sb.AppendFormat(@"An exception is now going to be thrown...");

            MessageBox.Show(sb.ToString(), "Bad Application.ThreadException handler");

            throw new Exception("Exception raised in handler wired up to the ThreadException event");
        }


        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sb = new StringBuilder(500);
            sb.AppendFormat(@"* An exception has occurred on a worker thread (ie not the main UI thread).{0}", Environment.NewLine);
            sb.AppendFormat(@"* It has been trapped by an event handler that has been wired up to ");
            sb.AppendFormat(@"AppDomain.CurrentDomain.UnhandledException.{0}", Environment.NewLine);
            sb.AppendFormat(@"* You would ordinarily log and notify this exception so that ops can ");
            sb.AppendFormat(@"diagnose and fix the cause of the exception.{0}", Environment.NewLine);
            sb.AppendFormat(@"* WinForms will always show a crash dialog and then close the application.{0}", Environment.NewLine);
            sb.AppendFormat(@"* Details of exception:\n{0}", e.ExceptionObject);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Here comes the crash dialog box...");

            MessageBox.Show(sb.ToString(), "Unhandled exception occurred in a worker thread");
        }


        private void DisposeOfForm(ref Form formToDispose)
        {
            if (formToDispose != null)
            {
                formToDispose.Close();
                formToDispose = null;
            }
        }

        private void simpleResponsiveFormEg_Click(object sender, EventArgs e)
        {
            new SimpleResponsiveForm().Show();
        }

        private void fileSystemSecurityTest_Click(object sender, EventArgs e)
        {
            FileInfo file;
            try
            {
                file = TempFile.Create();
            }
            finally
            {
                TempDir.Remove();
            }
            MessageBox.Show(string.Format("Successfully created a deleted the following file: {0}", file.FullName));
        }

        private void startMeInAnotherProcessButton_Click(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath, "StartedBy:-Process.Start");
        }

        private void openDrawingExamplesButton_Click(object sender, EventArgs e)
        {
            new DrawingExamples().Show();
        }

        private void openPerformanceCounterExamplesButton_Click(object sender, EventArgs e)
        {
            new PerformanceCounterExamples().Show();
        }
    }
}