using System.IO;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    public abstract class FileSystemTestsBase
    {
        #region Properties

        public DirectoryInfo RootDirectory
        {
            get { return TempDir.Root; }
        }

        #endregion


        public FileInfo NewFileFor(DirectoryInfo directory, string fileName)
        {
            string filePath = Path.Combine(directory.FullName, fileName);
            File.WriteAllText(filePath, "");
            return new FileInfo(filePath);
        }


        public abstract void ReleaseFileLocksIfAnyHeld();


        [TearDown]
        public virtual void TestCleanup()
        {
            ReleaseFileLocksIfAnyHeld();
            TempDir.Remove();
        }


        [SetUp]
        public virtual void TestInitialise()
        {
            TempDir.CreateNew();
        }
    }
}