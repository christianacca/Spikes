using System;
using System.Collections;
using System.Diagnostics;
using System.Management;
using System.Threading;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class WmiTests
    {
        private string _eventLogSource = "WmiTests_OkToDelete";


        #region Test helpers

        private ManagementScope NewPrivilegedConnection
        {
            get
            {
                ConnectionOptions connectionOptions = new ConnectionOptions();
                connectionOptions.EnablePrivileges = true; //need to do this to access Event log instrumentation
                return new ManagementScope(new ManagementPath(), connectionOptions);
            }
        }


        private T SelectFirst<T>(ManagementObjectCollection instances)
        {
            IEnumerator enumerator = instances.GetEnumerator();
            enumerator.MoveNext();
            return (T) enumerator.Current;
        }

        #endregion


        [Test]
        public void CanRetrieveAllInstancesOfAnInstrumentedClass()
        {
            //This is shorthand for querying the Win32_Service class
            //- in the wmi namespace \root\cimv2
            //- on the local machine

            using (ManagementClass serviceClass = new ManagementClass("Win32_Service"))
            {
                using (ManagementObjectCollection serviceInstances = serviceClass.GetInstances())
                {
                    Assert.That(serviceInstances, Is.Not.Empty);
                    using (ManagementObject service = SelectFirst<ManagementObject>(serviceInstances))
                    {
                        Assert.That(service["__CLASS"], Is.EqualTo("Win32_Service"));
                    }
                }
            }
        }


        [Test]
        public void CanRetreiveInstrumentedInstancesMatchingPredicate()
        {
            //This is shorthand for querying the Win32_Service class
            //- in the wmi namespace \root\cimv2
            //- on the local machine

            SelectQuery query = new SelectQuery("Win32_Service", "Started = TRUE");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                using (ManagementObjectCollection startedService = searcher.Get())
                {
                    Assert.That(startedService.Count > 0, Is.True);
                    using (ManagementObject service = SelectFirst<ManagementObject>(startedService))
                    {
                        Assert.That(service["Started"], Is.True);
                    }
                }
            }
        }


        [Test]
        public void CanMakeAnExplicitWmiConnection()
        {
            ManagementPath path = new ManagementPath();
            path.Server = "."; //this local machine
            path.NamespacePath = @"root\CIMV2"; //containing the WMI classes that you're going to query
            path.ClassName = "Win32_Service"; //the WMI class you're going to query
            ManagementScope scope = new ManagementScope(path);
            scope.Connect();

            Assert.That(scope.IsConnected, Is.True);
        }


        [Test, Ignore("Test will work in isolation but not when run in test suite")]
        public void CanSynchronouslyListenToWmiEvents()
        {
            string eventFilter =
                string.Format("TargetInstance ISA \"Win32_NTLogEvent\" AND TargetInstance.SourceName = \"{0}\"",
                              _eventLogSource);
            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent", TimeSpan.FromSeconds(1), eventFilter);

            EventWatcherOptions watcherOptions = new EventWatcherOptions();
            watcherOptions.Timeout = TimeSpan.FromMilliseconds(1000);

            using (
                ManagementEventWatcher watcher =
                    new ManagementEventWatcher(NewPrivilegedConnection, query, watcherOptions))
            {
                watcher.Start();

                ThreadPool.QueueUserWorkItem(delegate {
                    Thread.Sleep(200); //pause to allow for watcher to start waiting for event
                    EventLog.WriteEntry(_eventLogSource, "Hello World", EventLogEntryType.Information);
                });

                try
                {
                    using (ManagementBaseObject nextEvent = watcher.WaitForNextEvent())
                    {
                        Assert.That(nextEvent, Is.Not.Null);
                    }
                }
                finally
                {
                    watcher.Stop();
                }
            }
        }


        [Test, Ignore("Test will work in isolation but not when run in test suite")]
        public void CanAsynchronouslyListenToWmiEvents()
        {
            string eventFilter =
                string.Format("TargetInstance ISA \"Win32_NTLogEvent\" AND TargetInstance.SourceName = \"{0}\"",
                              _eventLogSource);
            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent", TimeSpan.FromSeconds(1), eventFilter);

            using (ManagementEventWatcher watcher = new ManagementEventWatcher(NewPrivilegedConnection, query))
            {
                bool eventReceived = false;
                watcher.EventArrived += delegate {
                    eventReceived = true;
                };

                watcher.Start();

                try
                {
                    EventLog.WriteEntry(_eventLogSource, "Hello World", EventLogEntryType.Information);
                    Thread.Sleep(300); //wait for the watcher to be notified of the event log entry we've just written
                    Assert.That(eventReceived, Is.True);
                }
                finally
                {
                    watcher.Stop();
                }
            }
        }


        [Test, Ignore("Test will work in isolation but not when run in test suite")]
        public void CanRetreieveUnderlyingObjectThatWmiEventReleatesTo()
        {
            string eventFilter =
                string.Format("TargetInstance ISA \"Win32_NTLogEvent\" AND TargetInstance.SourceName = \"{0}\"",
                              _eventLogSource);
            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent", TimeSpan.FromSeconds(1), eventFilter);

            EventWatcherOptions watcherOptions = new EventWatcherOptions();
            watcherOptions.Timeout = TimeSpan.FromMilliseconds(1000);

            using (
                ManagementEventWatcher watcher =
                    new ManagementEventWatcher(NewPrivilegedConnection, query, watcherOptions))
            {
                watcher.Start();

                ThreadPool.QueueUserWorkItem(delegate {
                    Thread.Sleep(100); //pause to allow for watcher to start waiting for event
                    EventLog.WriteEntry(_eventLogSource, "Hello World", EventLogEntryType.Information);
                });

                try
                {
                    using (ManagementBaseObject nextEvent = watcher.WaitForNextEvent())
                    {
                        ManagementBaseObject eventLogEntry = (ManagementBaseObject) nextEvent["TargetInstance"];
                        Assert.That(eventLogEntry["Message"], Is.EqualTo("Hello World\r\n"));
                    }
                }
                finally
                {
                    watcher.Stop();
                }
            }
        }
    }
}