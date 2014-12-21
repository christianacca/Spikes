using System;
using System.Collections.Generic;
using System.IO;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    [TestFixture]
    public class NavigatingFileSystem : FileSystemTestsBase
    {
        private List<string> _loggedNames;

        #region Setup/Teardown

        public override void TestInitialise()
        {
            _loggedNames = new List<string>();
            base.TestInitialise();
        }


        public override void ReleaseFileLocksIfAnyHeld() {}

        #endregion

        #region Test helpers

        private string[] FullNamesFor(params FileSystemInfo[] nodes)
        {
            List<string> names = new List<string>(nodes.Length);
            foreach (FileSystemInfo node in nodes)
                names.Add(node.FullName);
            return names.ToArray();
        }


        private FileSystemNavigator NewLoggingNavigatorForTempDirectory
        {
            get
            {
                return new FileSystemNavigator(TempDir.Root,
                                               delegate(FileSystemInfo node) {
                                                   _loggedNames.Add(node.FullName);
                                               });
            }
        }

        #endregion

        [Test]
        public void WillExecuteOnceForStartingDirectory()
        {
            FileSystemNavigator navigator = NewLoggingNavigatorForTempDirectory;
            navigator.Walk();

            Assert.That(_loggedNames, Is.EqualTo(FullNamesFor(TempDir.Root)));
        }


        [Test]
        public void WillExecuteOnceForEachSubdirectory()
        {
            DirectoryInfo subdirectory1 = TempDir.CreateSubdirectory("1");
            DirectoryInfo subdirectory2 = TempDir.CreateSubdirectory("2");

            FileSystemNavigator navigator = NewLoggingNavigatorForTempDirectory;
            navigator.Walk();

            Assert.That(_loggedNames, Is.EqualTo(FullNamesFor(TempDir.Root, subdirectory1, subdirectory2)));
        }


        [Test]
        public void WillExecuteOnceForEachFileWithinStartingDirectory()
        {
            FileInfo file1 = TempFile.Create("1.txt");
            FileInfo file2 = TempFile.Create("2.txt");

            FileSystemNavigator navigator = NewLoggingNavigatorForTempDirectory;
            navigator.Walk();

            Assert.That(_loggedNames, Is.EqualTo(FullNamesFor(TempDir.Root, file1, file2)));
        }


        [Test]
        public void WillExecuteOnceForEachDescendantDirectory_TwoGenerationsDeep()
        {
            DirectoryInfo subDirectory1 = TempDir.CreateSubdirectory("1");
            DirectoryInfo subSubDirectory1 = subDirectory1.CreateSubdirectory("1.1");
            DirectoryInfo subSubDirectory2 = subDirectory1.CreateSubdirectory("1.2");

            DirectoryInfo subDirectory2 = TempDir.CreateSubdirectory("2");

            FileSystemNavigator navigator = NewLoggingNavigatorForTempDirectory;
            navigator.Walk();

            string[] expectedNames =
                FullNamesFor(TempDir.Root, subDirectory1, subSubDirectory1, subSubDirectory2, subDirectory2);
            Assert.That(_loggedNames, Is.EqualTo(expectedNames));
        }


        [Test]
        public void WillExecuteOnceForEachDescendantDirectoryAndFile_TwoGenerationsDeep()
        {
            FileInfo file1 = TempFile.Create("1.txt");

            DirectoryInfo subDirectory1 = TempDir.CreateSubdirectory("1");
            DirectoryInfo subSubDirectory1 = subDirectory1.CreateSubdirectory("1.1");
            DirectoryInfo subSubDirectory2 = subDirectory1.CreateSubdirectory("1.2");
            FileInfo file2 = NewFileFor(subSubDirectory2, "2.txt");
            FileInfo file3 = NewFileFor(subSubDirectory2, "3.txt");

            DirectoryInfo subDirectory2 = TempDir.CreateSubdirectory("2");

            FileSystemNavigator navigator = NewLoggingNavigatorForTempDirectory;
            navigator.Walk();

            string[] expectedNames = FullNamesFor(TempDir.Root,
                                                  file1,
                                                  subDirectory1,
                                                  subSubDirectory1,
                                                  subSubDirectory2,
                                                  file2,
                                                  file3,
                                                  subDirectory2);
            Assert.That(_loggedNames, Is.EqualTo(expectedNames));
        }


        [Test]
        public void WillExecuteOnceForEachDescendantDirectory_ThreeGenerationsDeep()
        {
            DirectoryInfo subDirectory1 = TempDir.CreateSubdirectory("1");
            DirectoryInfo subSubDirectory1 = subDirectory1.CreateSubdirectory("1.1");
            DirectoryInfo subSubDirectory2 = subDirectory1.CreateSubdirectory("1.2");
            DirectoryInfo subSubSubDirectory = subSubDirectory2.CreateSubdirectory("1.2.1");

            DirectoryInfo subDirectory2 = TempDir.CreateSubdirectory("2");


            FileSystemNavigator navigator = NewLoggingNavigatorForTempDirectory;
            navigator.Walk();

            string[] expectedNames = FullNamesFor(TempDir.Root,
                                                  subDirectory1,
                                                  subSubDirectory1,
                                                  subSubDirectory2,
                                                  subSubSubDirectory,
                                                  subDirectory2);
            Assert.That(_loggedNames, Is.EqualTo(expectedNames));
        }
    }



    public delegate void ProcessFileSystemNode(FileSystemInfo node);



    public class FileSystemNavigator
    {
        private readonly ProcessFileSystemNode _actionToExecuteForEachNode;
        private readonly DirectoryInfo _rootDirectory;


        public FileSystemNavigator(DirectoryInfo directoryInfo, ProcessFileSystemNode actionToExecuteForEachNode)
        {
            _rootDirectory = directoryInfo;
            _actionToExecuteForEachNode = actionToExecuteForEachNode;
        }


        public void Walk()
        {
            WalkRecursively(_rootDirectory);
        }


        public void WalkRecursively(DirectoryInfo parent)
        {
            _actionToExecuteForEachNode(parent);

            foreach (FileInfo file in parent.GetFiles())
                _actionToExecuteForEachNode(file);

            foreach (DirectoryInfo directory in parent.GetDirectories())
                WalkRecursively(directory);
        }
    }
}