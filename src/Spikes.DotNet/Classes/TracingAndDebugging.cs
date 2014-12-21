#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class TracingAndDebugging
    {
        private List<TraceListener> _existingListeners;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            RemoveExistingListeners();
        }


        [TearDown]
        public void TestCleanup()
        {
            Debug.Listeners.Clear();
            RestoreExistingListeners();
        }

        #endregion


        #region Test helpers

        private void RemoveExistingListeners()
        {
            _existingListeners = new List<TraceListener>(Debug.Listeners.Count);
            foreach (TraceListener listener in Debug.Listeners)
                _existingListeners.Add(listener);

            Debug.Listeners.Clear();
        }


        private void RestoreExistingListeners()
        {
            //this will also add back listeners to Trace class
            foreach (TraceListener listener in _existingListeners)
                Debug.Listeners.Add(listener);
        }

        #endregion


        [Test]
        public void CanListenForDebugLogMessages()
        {
            var listener = new InMemoryListTraceListener();
            Debug.Listeners.Add(listener);

            Debug.Write("Hello world");
            Assert.That(listener.Messages[0], Is.EqualTo("Hello world"));
        }


        [Test]
        public void CanListenForTraceLogMessages()
        {
            var listener = new InMemoryListTraceListener();
            Trace.Listeners.Add(listener);

            Trace.Write("Hello world");
            Assert.That(listener.Messages[0], Is.EqualTo("Hello world"));
        }


        [Test]
        public void DebugAndTraceClassBothShareTheSameListenersCollection()
        {
            var listener = new InMemoryListTraceListener();
            Trace.Listeners.Add(listener);

            Assert.That(Debug.Listeners, Has.Member(listener));
        }


        [Test]
        public void CanStopAssertDisplayingDialogBox()
        {
            //setup (because we removed listeners at the start of every test)
            RestoreExistingListeners();


            //the default listener is usually the one that will pop up the dialog
            var defaultListener = (DefaultTraceListener) Trace.Listeners["Default"];
            defaultListener.AssertUiEnabled = false; //disable UI dialog boxes

            //...therefore this assert failure will not display dialog
            Trace.Assert(false);
        }


        [Test]
        public void CanTestWhetherDefaultTraceListenerAlreadyPresentInListenersCollection()
        {
            //setup (because we removed listeners at the start of every test)
            RestoreExistingListeners();

            Assert.That(Trace.Listeners.OfType<DefaultTraceListener>().Any(), Is.True);
        }


        [Test]
        public void CanRemoveDefaultTraceListener()
        {
            //setup (because we removed listeners at the start of every test)
            RestoreExistingListeners();

            //when
            Trace.Listeners.Remove("Default");

            //then
            Assert.That(Trace.Listeners.OfType<DefaultTraceListener>().Any(), Is.False);
        }
    }



    [TestFixture]
    public class TraceSourceExamples
    {
        [Test]
        public void CanCreateAndUseTraceSourceThatIsNotDefinedInConfiguration()
        {
            var listener = new InMemoryListTraceListener();
            var adhocSource = new TraceSource("NotInConfig", SourceLevels.Information);
            adhocSource.Listeners.Clear();
            adhocSource.Listeners.Add(listener);
            var consoleTraceListener =
                new ConsoleTraceListener
                    {
                        TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId | TraceOptions.ThreadId
                    };
            adhocSource.Listeners.Add(consoleTraceListener);

            adhocSource.TraceInformation("Hello");

            Assert.That(listener.Messages[1], Is.EqualTo("Hello\r\n"));
        }
    }



    [TestFixture]
    public class SetupFromConfiguration
    {
        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            ClearInMemoryTraceListeners();
        }

        #endregion


        #region Test helpers

        private void ClearInMemoryTraceListeners()
        {
            ((InMemoryListTraceListener) Trace.Listeners["CustomListener"]).Messages.Clear();

            var source = new TraceSource("TestSource");
            ((InMemoryListTraceListener) source.Listeners["CustomListener"]).Messages.Clear();
            ((InMemoryListTraceListener) source.Listeners["FilteredCustomListener"]).Messages.Clear();
        }

        #endregion


        [Test]
        public void CanRetrieveTraceListener()
        {
            TraceListener listener = Trace.Listeners["CustomListener"];
            Assert.That(listener, Is.TypeOf(typeof (InMemoryListTraceListener)));
        }


        [Test]
        public void CanUseTraceSwitchClassToControlTraceVerbosity()
        {
            //setup
            var listener = (InMemoryListTraceListener) Trace.Listeners["CustomListener"];

            //run test...
            var traceSwitch = new TraceSwitch("TestSwitch", "Test switch from configs");
            Assert.That(traceSwitch.Level, Is.EqualTo(TraceLevel.Warning), "configured at Warning level");

            string traceExpected = "This trace will be output";
            Trace.WriteIf(traceSwitch.TraceWarning, traceExpected);
            Trace.WriteIf(traceSwitch.TraceInfo, "This trace will NOT be output");

            Assert.That(listener.Messages.Count, Is.EqualTo(1), "one msg output");
            Assert.That(listener.Messages, Has.Member(traceExpected), "expected msg output");
        }


        [Test]
        public void CanUseTraceSourceToEncapsulateBothSwitchAndCollectionOfListeners()
        {
            var source = new TraceSource("TestSource");

            Assert.That(source.Listeners["CustomListener"], Is.TypeOf(typeof (InMemoryListTraceListener)));
            Assert.That(source.Switch.Level, Is.EqualTo(SourceLevels.Information));
        }


        [Test]
        public void CanSpecifyTheInfomationTraceListenerOutputs()
        {
            var source = new TraceSource("TestSource");

            Assert.That(source.Listeners["CustomListener"].TraceOutputOptions,
                        Is.EqualTo(TraceOptions.Callstack));
        }


        [Test]
        public void CanUseTraceSourceToPerformTracing()
        {
            //setup
            var source = new TraceSource("TestSource");

            //not testing yet just clarifying assumptions
            Assert.That(source.Switch.Level, Is.EqualTo(SourceLevels.Information), "will log info msgs");

            //run test
            source.TraceData(TraceEventType.Information, 0, "A trace message");
            source.TraceData(TraceEventType.Error, 0, "Simulating something terrible");

            //assert expectations
            var listener = (InMemoryListTraceListener) source.Listeners["CustomListener"];
            Assert.That(listener.Messages[1], Is.EqualTo("A trace message\r\n"));
            Assert.That(listener.Messages[4], Is.EqualTo("Simulating something terrible\r\n"));
        }


        [Test]
        public void CanFilterIndividualTraceListener()
        {
            //setup
            var source = new TraceSource("TestSource");

            //not testing yet just clarifying assumptions
            Assert.That(source.Switch.Level, Is.EqualTo(SourceLevels.Information), "will log info msgs");

            //run test
            source.TraceData(TraceEventType.Information, 0, "A trace message");

            //assert expectations
            var listener = (InMemoryListTraceListener) source.Listeners["CustomListener"];
            var filteredListener =
                (InMemoryListTraceListener) source.Listeners["FilteredCustomListener"];

            Assert.That(filteredListener.Messages.Count, Is.EqualTo(0), "listener has not received msg");
            Assert.That(listener.Messages.Count, Is.Not.EqualTo(0), "listener received msg");
        }
    }
}