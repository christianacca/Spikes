using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture, Ignore("Don't have permissions to write to registry from this machine")]
    public class WritingToEventLog
    {
        private const string ExistingEvenLogName = "ExistingTestLog_OkToDelete";
        private const string ExistingEventSourceName = "ExistingEventSource_OkToDelete";
        private const string TempEventLogName = "SomeTestingEventLog_OkToDelete";
        protected const string TempEventSourceName = "SomeTestingEventSource_OkToDelete";
        private EventLog _existingLog;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            RemoveTestLogs();

            _existingLog = new EventLog(ExistingEvenLogName, ".", ExistingEventSourceName);
            _existingLog.WriteEntry("Initial entry"); //this ensures the log is actually created
        }


        [TearDown]
        public void TestCleanup()
        {
            RemoveTestLogs();
        }

        #endregion


        #region Test helpers

        private void RemoveTestLogs()
        {
            if (EventLog.Exists(ExistingEvenLogName))
                EventLog.Delete(ExistingEvenLogName);

            if (EventLog.Exists(TempEventLogName))
                EventLog.Delete(TempEventLogName);

            //this shouldn't be necessary because we're deleting the event log that owns this source
            //but getting errors in a test saying source already exists on this machine
            if (EventLog.SourceExists(TempEventSourceName))
                EventLog.DeleteEventSource(TempEventSourceName);
        }


        private R[] ToArray<R>(ICollection items)
        {
            List<R> result = new List<R>(items.Count);
            foreach (R item in items)
                result.Add(item);
            return result.ToArray();
        }

        #endregion


        [Test]
        [Category("Basic")]
        public void HowToWriteToDefaultEventLog()
        {
            EventLog.WriteEntry("OkToDelete", "Hello world", EventLogEntryType.Information);
        }


        [Test]
        [Category("Detailed")]
        public void WritingToDefaultLogWillCreateEventSourceIfNotAlreadyPresent()
        {
            //not testing yet just clarifying assumptions
            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.False);

            EventLog.WriteEntry(TempEventSourceName, "Hello world", EventLogEntryType.Information);

            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.True, "Event source created");
        }


        [Test]
        [Category("Basic")]
        public void HowToWriteToCustomEventLog()
        {
            _existingLog.WriteEntry("This is a test", EventLogEntryType.Information);

            int secondLogEntry = 1;
            Assert.That(_existingLog.Entries[secondLogEntry].Message, Is.EqualTo("This is a test"));
            Assert.That(_existingLog.Entries[secondLogEntry].EntryType, Is.EqualTo(EventLogEntryType.Information));
        }


        [Test]
        public void HowToRetrieveEventLogEntriesFilteredByEventSource()
        {
            //setup
            EventLog.WriteEntry(ExistingEvenLogName, "Hello 1");
            EventLog.WriteEntry(ExistingEvenLogName, "Hello 2");
            const string otherEventSource = "AnotherTempSource_OkToDelete";
            EventLog.CreateEventSource(otherEventSource, ExistingEvenLogName);
            EventLog.WriteEntry(otherEventSource, string.Format("{0} entry1", otherEventSource));


            EventLog eventLog = new EventLog(ExistingEvenLogName, ".", otherEventSource);
            Assert.That(eventLog.Entries.Count,
                        Is.Not.EqualTo(1),
                        "eventLog.Entries are not filtered by the event source used to new up eventLog");

            //must manually filter the event log entries
            Predicate<EventLogEntry> filterByEventSource = delegate(EventLogEntry obj) {
                return (obj.Source == otherEventSource);
            };
            EventLogEntry[] filteredEntries =
                Array.FindAll(ToArray<EventLogEntry>(eventLog.Entries), filterByEventSource);
            Assert.That(filteredEntries.Length, Is.EqualTo(1));
        }


        [Test]
        [Category("Basic")]
        public void HowToCreateCustomEventLogAndSource_Option1()
        {
            EventLog log = new EventLog(TempEventLogName, ".", TempEventSourceName);

            Assert.That(log.Log, Is.EqualTo(TempEventLogName));
            Assert.That(log.Source, Is.EqualTo(TempEventSourceName));
        }


        [Test]
        [Category("Basic")]
        public void HowToCreateCustomEventLogAndSource_Option2()
        {
            EventSourceCreationData creationParamaters =
                new EventSourceCreationData(TempEventSourceName, TempEventLogName);
            creationParamaters.MachineName = ".";
            EventLog.CreateEventSource(creationParamaters);

            Assert.That(EventLog.Exists(TempEventLogName), Is.True);
            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.True);
        }


        [Test]
        [Category("Basic")]
        public void HowToDeleteCustomEventLog()
        {
            EventLog.Delete(ExistingEvenLogName);
            Assert.That(EventLog.Exists(ExistingEvenLogName), Is.False);
        }


        [Test]
        [Category("Basic")]
        public void HowToDeleteCustomEventSource()
        {
            EventLog.DeleteEventSource(ExistingEventSourceName);
            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.False);
        }


        [Test]
        [Category("Basic")]
        public void DeletingEventLogWillDeleteAssociatedEventSource()
        {
            EventLog.Delete(ExistingEvenLogName);
            Assert.That(EventLog.SourceExists(ExistingEventSourceName), Is.False);
        }


        [Test]
        [Category("Detailed")]
        public void NewingUpCustomEventLogWithCustomSourceWillNotPhysicallyCreateLogOrSource()
        {
            //not testing yet just clarifying assumptions
            Assert.That(EventLog.Exists(TempEventLogName), Is.False, "log does not exist at start of test");
            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.False, "source does not exist at start of test");

            new EventLog(TempEventLogName, ".", TempEventSourceName);
            Assert.That(EventLog.Exists(TempEventLogName), Is.False, "log does not exist");
            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.False, "source does not exist");
        }


        [Test]
        [Category("Detailed")]
        public void EventLogOnlyCreatedWhenEntryIsWrittenToLog()
        {
            //not testing yet just clarifying assumptions
            Assert.That(EventLog.Exists(TempEventLogName), Is.False, "log does not exist at start of test");

            EventLog log = new EventLog(TempEventLogName, ".", TempEventSourceName);
            log.WriteEntry("First entry");

            Assert.That(EventLog.Exists(TempEventLogName), Is.True, "log created");
            Assert.That(EventLog.SourceExists(TempEventSourceName), Is.True, "source created");
        }


        [Test]
        [Category("Detailed")]
        public void HowToDeleteEventLogWhenYouKnowTheSourceNameOnly()
        {
            string logName = EventLog.LogNameFromSourceName(ExistingEventSourceName, ".");
            EventLog.Delete(logName);

            Assert.That(EventLog.Exists(logName), Is.False);
        }


        [Test]
        [Category("Detailed")]
        public void CanCreateEventLogThatAlreadyExistis()
        {
            new EventLog(ExistingEvenLogName, ".", ExistingEventSourceName);
        }


        [Test]
        [Category("Detailed")]
        public void CreatingEventSourceThatAlreadyExistsThrows()
        {
            try
            {
                EventLog.CreateEventSource(ExistingEventSourceName, ExistingEvenLogName);
                Assert.Fail("Should have thrown");
            }
            catch (ArgumentException) {}
        }


        [Test]
        [Category("Detailed")]
        public void Gotcha_DeletingEventSourceThatSharesTheSameNameAsLogWillThrow()
        {
            string sourceName = TempEventLogName;
            EventLog.CreateEventSource(sourceName, sourceName);
            try
            {
                EventLog.DeleteEventSource(sourceName);
                Assert.Fail("Should have thrown");
            }
            catch (InvalidOperationException) {}
        }
    }
}