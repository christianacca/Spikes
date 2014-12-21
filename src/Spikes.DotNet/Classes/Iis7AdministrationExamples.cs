using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Eca.Commons.App.Web;
using Microsoft.Web.Administration;
using NUnit.Framework;

namespace Eca.Spikes.DotNet.Classes
{
    [TestFixture, Ignore]
    public class Iis7AdministrationExamples
    {
        #region Test helpers

        private static ApplicationPool GetIisPoolToManage()
        {
//            ServerManager iisManager = ServerManager.OpenRemote("uatwb02vm");
            var iisManager = new ServerManager();
            ApplicationPool pool = iisManager.ApplicationPools["MemberServices"];
            return pool;
        }


        private static void PrintPoolState(ApplicationPool pool)
        {
            Console.Out.WriteLine("pool.State now: {0}", pool.State);
            Thread.Sleep(500);
            Console.Out.WriteLine("pool.State after 500ms: {0}", pool.State);
        }

        #endregion


        [Test]
        public void CanDetermineStatusOfPool()
        {
            var pool = GetIisPoolToManage();
            PrintPoolState(pool);
        }


        [Test]
        public void CanStopPool()
        {
            ApplicationPool pool = GetIisPoolToManage();
            pool.Stop();
            PrintPoolState(pool);
        }


        [Test]
        public void CanStartPool()
        {
            ApplicationPool pool = GetIisPoolToManage();
            Assert.That(pool.State,
                        Is.EqualTo(ObjectState.Stopped),
                        string.Format("checking assumptions - {0} pool should be stopped", pool.Name));

            pool.Start();
            PrintPoolState(pool);
        }


        [Test]
        public void CanStopPoolAndWaitForWorkerProcessToFinish()
        {
            ApplicationPool pool = GetIisPoolToManage();
            Assert.That(pool.State,
                        Is.EqualTo(ObjectState.Started),
                        string.Format("checking assumptions - {0} pool should be started", pool.Name));

            IEnumerable<Process> w3cWorkers =
                pool.WorkerProcesses.Select(p => Process.GetProcessById(p.ProcessId)).ToList();
            pool.Stop();
            while (!w3cWorkers.All(wp => wp.HasExited))
            {
                Thread.Sleep(1);
                Console.Out.WriteLine("Waiting on process to exit");
            }
            foreach (var worker in w3cWorkers)
            {
                Console.Out.WriteLine("Process {0} now closed", worker.Id);
            }
        }


        [Test]
        public void CanStopAndStartPool()
        {
            ApplicationPool pool = GetIisPoolToManage();
            Assert.That(pool.State,
                        Is.EqualTo(ObjectState.Started),
                        string.Format("checking assumptions - {0} pool should be started", pool.Name));

            pool.Restart(5000);
            PrintPoolState(pool);
        }


        [Test]
        public void CanDetermineWhetherSiteIsUp()
        {
            WebRequest request = HttpWebRequest.Create("http://www.uat.eca.co.uk/MemberServices/LogOn");
            var response = (HttpWebResponse) request.GetResponse();
            Console.Out.WriteLine("response.StatusCode = {0}", response.StatusCode);
        }
    }
}