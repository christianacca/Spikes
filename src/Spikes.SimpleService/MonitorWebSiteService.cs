using System;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace Eca.Spikes.SimpleService
{
    public partial class MonitorWebSiteService : ServiceBase
    {
        readonly Timer _timer;

        public MonitorWebSiteService()
        {
            _timer = new Timer(HandleTimerEvent, null, Timeout.Infinite, Timeout.Infinite);
            InitializeComponent();
        }


        private string LogFilePath
        {
            get
            {
                string applicationDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                return Path.Combine(applicationDirectory, "SimpleServiceLog.txt");
            }
        }


        protected override void OnStart(string[] args)
        {
            StartTimer();
        }


        private void StartTimer() 
        {
            _timer.Change(0, 10000);
        }


        protected override void OnStop()
        {
            StopTimer();
        }


        private void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }


        protected override void OnContinue()
        {
            StartTimer();
        }


        protected override void OnPause()
        {
            StopTimer();
        }


        protected override void OnShutdown()
        {
            StopTimer();
        }


        private void HandleTimerEvent(object state)
        {
            string url = "http://www.microsoft.com";
            try
            {
                LogStatusOfWebsite(url);
            }
            catch (Exception ex)
            {
                //note: swallowing exceptions is a terrible thing to do. Please never do this in production code
                EventLog.WriteEntry(
                    string.Format("Problem occurred trying log status of: {0}/n/rDetails: {1}", url, ex));
            }
        }


        private void LogStatusOfWebsite(string url)
        {
            using (TextWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine("{0} - Status of website {1}", DateTime.Now, GetWebSiteStatus(url));
            }
        }


        private string GetWebSiteStatus(string url)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                return response.StatusDescription;
            }
        }
    }
}