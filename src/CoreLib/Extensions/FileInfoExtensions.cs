using System;
using System.IO;

namespace Eca.Commons.Extensions
{
    public static class FileInfoExtensions
    {
        #region Class Members

        public static void Rename(this FileInfo file, string newName)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (newName == null) throw new ArgumentNullException("newName");
            if (newName.Length == 0) throw new ArgumentException("The name is empty.", "newName");
            if (newName.IndexOf(Path.DirectorySeparatorChar) >= 0 ||
                newName.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            {
                throw new ArgumentException("The name contains path separators. The file would be moved.", "newName");
            }

            string directoryPath = !String.IsNullOrEmpty(file.DirectoryName)
                                       ? file.DirectoryName
                                       : AppDomain.CurrentDomain.BaseDirectory;
            string newPath = Path.Combine(directoryPath, newName);
            file.MoveTo(newPath);
        }

        #endregion
    }
}