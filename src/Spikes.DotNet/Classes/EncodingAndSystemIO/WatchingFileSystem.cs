using System;
using System.IO;
using System.Threading;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    [TestFixture]
    public class WatchingFileSystem : FileSystemTestsBase
    {
        private int _createEventsFired;
        private int _renameEventsFired;

        #region Setup/Teardown

        public override void TestInitialise()
        {
            _createEventsFired = 0;
            _renameEventsFired = 0;
            base.TestInitialise();
        }


        public override void ReleaseFileLocksIfAnyHeld() {}

        #endregion

        #region Test helpers

        private void AssertCreationEventHasFired()
        {
            Thread.Sleep(100);
            Assert.That(_createEventsFired > 0, Is.True);
        }


        private void AssertNumberOfCreationEventsFiredIs(int expectedNumberOfEvents)
        {
            Thread.Sleep(100);
            Assert.That(_createEventsFired, Is.EqualTo(expectedNumberOfEvents), "expected number of events watched");
        }


        private void AssertRenameEventFired(bool expectation)
        {
            Thread.Sleep(100);
            Assert.That(_renameEventsFired > 0, Is.EqualTo(expectation));
        }


        private void AssertRenameEventHasFired()
        {
            AssertRenameEventFired(true);
        }


        private void AssertRenameEventHasNotFired()
        {
            AssertRenameEventFired(false);
        }


        private FileSystemWatcher WatchForFileSystemEventsWithin(DirectoryInfo directory)
        {
            return WatchForFileSystemEventsWithin(directory, "*");
        }


        private FileSystemWatcher WatchForFileSystemEventsWithin(DirectoryInfo directory, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(directory.FullName, filter);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += delegate {
                _renameEventsFired++;
            };
            watcher.Created += delegate {
                _createEventsFired++;
            };
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        #endregion

        [Test]
        public void CanWatchForDirectoryBeingAdded()
        {
            using (WatchForFileSystemEventsWithin(RootDirectory))
            {
                RootDirectory.CreateSubdirectory("OkToDelete1");
                AssertCreationEventHasFired();
            }
        }


        [Test]
        public void CanWatchForFileBeingAdded()
        {
            using (WatchForFileSystemEventsWithin(RootDirectory))
            {
                NewFileFor(RootDirectory, "OkToDelete.txt");
                AssertCreationEventHasFired();
            }
        }


        [Test]
        public void CanWatchForRenamedFile()
        {
            FileInfo fileToRename = NewFileFor(RootDirectory, "OkToDelete.txt");

            using (WatchForFileSystemEventsWithin(RootDirectory))
            {
                fileToRename.MoveTo(Path.Combine(fileToRename.Directory.FullName, "OkToDelete1_Renamed.txt"));
                AssertRenameEventHasFired();
            }
        }


        [Test]
        public void CanWatchForRenamedDirectory()
        {
            DirectoryInfo directoryToRename = RootDirectory.CreateSubdirectory("OkToDelete1");

            using (WatchForFileSystemEventsWithin(RootDirectory))
            {
                directoryToRename.MoveTo(Path.Combine(directoryToRename.Parent.FullName, "OkToDelete1_Renamed"));
                AssertRenameEventHasFired();
            }
        }


        [Test]
        public void CannotWatchForTheRenameOfDirectoryBeingWatched()
        {
            DirectoryInfo directoryToRename = RootDirectory.CreateSubdirectory("OkToDelete1");

            using (WatchForFileSystemEventsWithin(directoryToRename))
            {
                directoryToRename.MoveTo(Path.Combine(directoryToRename.Parent.FullName, "OkToDelete1_Renamed"));
                AssertRenameEventHasNotFired();
            }
        }


        [Test]
        public void CanIgnoreEventsForSpecificFileTypes()
        {
            using (WatchForFileSystemEventsWithin(RootDirectory, "*.txt"))
            {
                NewFileFor(RootDirectory, "IgnoreThisFilesCreation.bak");
                NewFileFor(RootDirectory, "WatchThisFilesCreation.txt");
                AssertNumberOfCreationEventsFiredIs(1);
            }
        }
    }
}