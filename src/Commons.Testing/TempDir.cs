using System;
using System.IO;

namespace Eca.Commons.Testing
{
    public sealed class TempDir
    {
        #region Class Members

        private static string _rootPath;


        public static DirectoryInfo Root
        {
            get { return new DirectoryInfo(RootPath); }
        }

        public static string RootPath
        {
            get { return _rootPath = _rootPath ?? Path.Combine(Path.GetTempPath(), "TestDirectoryOkToDelete"); }
        }


        public static string AbsPathFor(params string[] paths)
        {
            string result = RootPath;
            for (int i = 0; i < paths.Length; i++)
            {
                result = Path.Combine(result, paths[i]);
            }
            return result;
        }


        private static DirectoryInfo CreateDirectory(string path, bool deleteFirst)
        {
            if (deleteFirst)
                Delete(path);

            Directory.CreateDirectory(path);
            return new DirectoryInfo(path);
        }


        public static DirectoryInfo CreateNew()
        {
            return CreateDirectory(RootPath, true);
        }


        /// <summary>Creates a temporary directory for a unit test.</summary>
        /// <remarks>
        /// If the directory already exists it will first be deleted and a new empty directory will
        /// be created.
        /// </remarks>
        /// <returns>The complete path to the created directory.</returns>
        public static DirectoryInfo CreateNewSubDirectory(string name)
        {
            if (Path.IsPathRooted(name)) throw new ArgumentException("Path must not be rooted");
            return CreateDirectory(Path.Combine(_rootPath, name), true);
        }


        public static DirectoryInfo CreateSubdirectory()
        {
            //GetRandomFileName returns directory or file name
            return CreateSubdirectory(Path.GetRandomFileName());
        }


        public static DirectoryInfo CreateSubdirectory(string name)
        {
            if (Path.IsPathRooted(name)) throw new ArgumentException("Path must not be rooted");
            return CreateDirectory(Path.Combine(_rootPath, name), false);
        }


        /// <summary>Delete the directory at the given path and everything in it.</summary>
        public static void Delete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    // ensure directorty is writable
                    File.SetAttributes(path, FileAttributes.Normal);
                    // ensure all files and subdirectories are writable
                    SetAllFileAttributesToNormal(path);
                    string[] directoryNames = Directory.GetDirectories(path);
                    foreach (string directoryName in directoryNames)
                    {
                        Delete(directoryName);
                    }
                    string[] fileNames = Directory.GetFiles(path);
                    foreach (string fileName in fileNames)
                    {
                        File.Delete(fileName);
                    }
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                throw new NUnit.Framework.AssertionException("Unable to cleanup '" + path + "'.  " + ex.Message, ex);
            }
        }


        public static string GeneratePathToFileNotYetCreated()
        {
            return Path.Combine(RootPath, Path.GetRandomFileName());
        }


        public static string PathFor(string child)
        {
            return Path.Combine(RootPath, child);
        }


        public static string RelativePathFor(params string[] paths)
        {
            string result = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                result = Path.Combine(result, paths[i]);
            }
            return result;
        }


        public static void Remove()
        {
            Delete(RootPath);
        }


        /// <summary>
        /// Recurse over all files in the directory setting each file's attributes 
        /// to <see cref="FileAttributes.Normal" />.
        /// </summary>
        private static void SetAllFileAttributesToNormal(string path)
        {
            string[] fileNames = Directory.GetFiles(path);
            foreach (string fileName in fileNames)
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
            }

            string[] directoryNames = Directory.GetDirectories(path);
            foreach (string directoryName in directoryNames)
            {
                File.SetAttributes(directoryName, FileAttributes.Normal);
                SetAllFileAttributesToNormal(directoryName);
            }
        }

        #endregion
    }
}