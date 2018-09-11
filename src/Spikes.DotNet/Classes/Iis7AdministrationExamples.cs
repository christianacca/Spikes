using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Eca.Commons.App.Web;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using NUnit.Framework;

namespace Eca.Spikes.DotNet.Classes
{
    public class Credentials
    {
        public Credentials(string username, string password, ServiceAccount account)
        {
            Username = username;
            Password = password;
            Account = account;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
        public ServiceAccount Account { get; private set; }
    }

    [TestFixture, Ignore]
    public class Iis7AdministrationExamples
    {
        /// <remarks>
        /// To create this local user run the following in an elevated powershell prompt:
        /// <code>$p = ConvertTo-SecureString '(pe!ter4powershell)' -AsPlainText -Force; New-LocalUser HangfireLocalUser -Password $p -PasswordNeverExpires</code>
        /// </remarks>
        private readonly Credentials _testUser = new Credentials($@"{Environment.MachineName}\HangfireLocalUser", "(pe!ter4powershell)", ServiceAccount.User);

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


        [Test]
        public void CanListSites()
        {
            using (var iis = new ServerManager())
            {
                Assert.That(iis.Sites.Count, Is.GreaterThanOrEqualTo(1));
            }

        }

        [Test]
        public void CanListAppPools()
        {
            using (var iis = new ServerManager())
            {
                Assert.That(iis.ApplicationPools.Count, Is.GreaterThanOrEqualTo(1));
            }

        }

        [Test]
        public void CanCreateAndDeleteAppPool()
        {
            var poolName = "OK_TO_DELETE_ME";

            // when (create)
            using (var iis = new ServerManager())
            {
                AddNewAppPool(iis, poolName, _testUser);
                iis.CommitChanges();
            }

            // then
            using (var iis = new ServerManager())
            {
                ApplicationPool pool = iis.ApplicationPools[poolName];
                Assert.That(pool, Is.Not.Null);
                Assert.That(pool.ProcessModel.IdentityType, Is.EqualTo(ProcessModelIdentityType.SpecificUser));
                Assert.That(pool.ProcessModel.UserName, Is.EqualTo(_testUser.Username));
            }

            // when (delete)
            using (var iis = new ServerManager())
            {
                var pool = iis.ApplicationPools[poolName];
                iis.ApplicationPools.Remove(pool);
                iis.CommitChanges();
            }

            // then
            using (var iis = new ServerManager())
            {
                Assert.That(iis.ApplicationPools[poolName], Is.Null);
            }
        }


        [Test]
        public void CanCreateAndDeleteSite()
        {
            var name = "OK_TO_DELETE_ME";

            // given
            RemoveSite(name);

            string path = Path.Combine(Path.GetTempPath(), name);

            // when (create)
            NewSite(name, _testUser, path);

            // then
            using (var iis = new ServerManager())
            {
                Assert.That(iis.Sites[name], Is.Not.Null);
                Assert.That(iis.ApplicationPools[name], Is.Not.Null);
                var sitePath = iis.Sites[name].Applications[0].VirtualDirectories[0].PhysicalPath;
                Assert.That(Directory.Exists(sitePath), Is.True);
            }

            // when (delete)
            RemoveSite(name);

            // then
            using (var iis = new ServerManager())
            {
                Assert.That(iis.Sites[name], Is.Null);
                Assert.That(iis.ApplicationPools[name], Is.Null);
                Assert.That(Directory.Exists(path), Is.False);
            }
        }


        [Test]
        public void Can_set_environment_variable_for_specific_apppool_user()
        {
            SetUserEnviornmentVariable(_testUser, key =>
            {
                key.SetValue("COR_ENABLE_PROFILING", "1");
                key.SetValue("COR_PROFILER", "{324F817A-7420-4E6D-B3C1-143FBED6D855}");
                key.SetValue("MicrosoftInstrumentationEngine_Host", "{CA487940-57D2-10BF-11B2-A3AD5A13CBC0}");
            });
        }

        private static void SetUserEnviornmentVariable(Credentials user, Action<RegistryKey> registryConfig,
            string name = "SITE_USED_TO_GEN_USER_PROFILE_OK_TO_DELETE_ME")
        {
            try
            {
                RemoveSite(name);
                NewSite(name, user);

                // need to wait to allow IIS site above to start so that the registry entries for the user profile to be created
                Thread.Sleep(TimeSpan.FromSeconds(2));

                string userSid = new NTAccount(user.Username).Translate(typeof(SecurityIdentifier)).Value;
                using (RegistryKey environKey = Registry.Users.OpenSubKey($@"{userSid}\Environment", true))
                {
                    registryConfig(environKey);
                }
            }
            finally
            {
                RemoveSite(name);
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Can_find_sid()
        {
            var account = new NTAccount(_testUser.Username);
            string sid = account.Translate(typeof(SecurityIdentifier)).Value;
            Console.Out.WriteLine("sid = {0}", sid);
        }

        private static void NewSite(string name, Credentials credentials, string path = null)
        {
            path = path ?? Path.Combine(Path.GetTempPath(), name);
            using (var iis = new ServerManager())
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                AddNewAppPool(iis, name, credentials);

                Site site = iis.Sites.Add(name, "http", $"*:999:{name}", path);
                site.Applications[0].ApplicationPoolName = name;
                iis.CommitChanges();
            }
        }

        private static void RemoveSite(string siteName)
        {
            using (var iis = new ServerManager())
            {
                var site = iis.Sites[siteName];

                if (site == null) return;

                var poolNames = site.Applications.Select(a => a.ApplicationPoolName).Distinct();
                var paths = site.Applications
                    .SelectMany(a => a.VirtualDirectories.Select(v => v.PhysicalPath)).Distinct().ToList();
                iis.Sites.Remove(site);
                poolNames.Select(n => iis.ApplicationPools[n]).ToList().ForEach(p => { iis.ApplicationPools.Remove(p); });
                iis.CommitChanges();

                paths.ForEach(Directory.Delete);
            }
        }


        private static void AddNewAppPool(ServerManager iis, string name, Credentials credentials)
        {
            ApplicationPool pool = iis.ApplicationPools.CreateElement();
            pool.Name = name;
            pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            pool.StartMode = StartMode.AlwaysRunning;
            pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
            pool.ProcessModel.UserName = credentials.Username;
            if (!string.IsNullOrWhiteSpace(credentials.Password))
            {
                pool.ProcessModel.Password = credentials.Password;
            }
            iis.ApplicationPools.Add(pool);
        }
    }
}