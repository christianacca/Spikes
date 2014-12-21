using System.IO;
using System.IO.IsolatedStorage;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class UsingIsolatedStorage
    {
        private const string RootTestDirectory = "MyDirectory";


        [SetUp]
        public void TestInitialise()
        {
            CreateRootTestDirectoryInStore();
        }


        [TearDown]
        public void TestCleanup()
        {
            NewStore().Remove();
        }


        private void CreateRootTestDirectoryInStore()
        {
            using (IsolatedStorageFile store = NewStore())
            {
                store.CreateDirectory(RootTestDirectory);
            }
        }


        private IsolatedStorageFile NewStore() 
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }


        private void CreateFileInStore(string relativeFilePath, IsolatedStorageFile store)
        {
            using (new IsolatedStorageFileStream(relativeFilePath, FileMode.CreateNew, store)) {}
        }


        [Test]
        public void CanCreateAStoreForThisAssembly()
        {
            using(IsolatedStorageFile store = NewStore())
            {
                Assert.That(store, Is.Not.Null);
            }
        }


        [Test]
        public void CanCreateDirectoryWithinStore()
        {
            using (IsolatedStorageFile store = NewStore())
            {
                store.CreateDirectory("MyDirectory");
                Assert.That(store.GetDirectoryNames("*"), Has.Member("MyDirectory"));
            }
        }


        [Test]
        public void CanDeleteEmptyDirectoryWithinStore()
        {
            using (IsolatedStorageFile store = NewStore())
            {
                store.DeleteDirectory(RootTestDirectory);
                Assert.That(store.GetDirectoryNames("*"), Has.No.Member(RootTestDirectory));
            }

        }


        private void DeleteDirectoryFromStore(string directory, IsolatedStorageFile store)
        {
            foreach (string fileName in store.GetFileNames(string.Format(@"{0}\*", directory)))
            {
                store.DeleteFile(Path.Combine(directory, fileName));
            }
            foreach (string subDirectory in store.GetDirectoryNames(Path.Combine(directory, "*")))
            {
                DeleteDirectoryFromStore(Path.Combine(directory, subDirectory), store);
            }
            store.DeleteDirectory(directory);
        }


        [Test]
        public void CanCreateFileWithContentWithinStore()
        {
            using (IsolatedStorageFile store = NewStore())
            {
                string fileName = "MyFile.txt";
                string relativeFilePath = Path.Combine(RootTestDirectory, fileName);
                using (StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(relativeFilePath, FileMode.CreateNew, store)))
                {
                    writer.WriteLine("Hello World");
                }
                string searchPattern = Path.Combine(RootTestDirectory, "*");
                Assert.That(store.GetFileNames(searchPattern), Has.Member(fileName));
            }
        }


        [Test]
        public void CanDeleteNestedDirectoriesContainingFiles()
        {
            using (IsolatedStorageFile store = NewStore())
            {
                //setup
                string fileInRootDirectory = Path.Combine(RootTestDirectory, "MyFile.txt");
                CreateFileInStore(fileInRootDirectory, store);

                string subDirectoryPath = Path.Combine(RootTestDirectory, "SubDirectory");
                store.CreateDirectory(subDirectoryPath);

                fileInRootDirectory = Path.Combine(subDirectoryPath, "AnotherMyFile.txt");
                CreateFileInStore(fileInRootDirectory, store);

                //run test
                DeleteDirectoryFromStore(RootTestDirectory, store);
                Assert.That(store.GetDirectoryNames("*"), Has.No.Member(RootTestDirectory));
            }
        }
    }
}