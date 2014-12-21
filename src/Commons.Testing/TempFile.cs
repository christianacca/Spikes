using System;
using System.IO;
using System.Text;

namespace Eca.Commons.Testing
{
    public sealed class TempFile
    {
        #region Class Members

        /// <summary>
        /// Creates a small temp file returns its file info.
        /// </summary>
        public static FileInfo Create()
        {
            return Create(Path.GetRandomFileName(), string.Empty, null);
        }


        public static FileInfo Create(string relativeFilePath)
        {
            return Create(relativeFilePath, string.Empty, null);
        }


        private static FileInfo Create(string relativeFilePath, string contents, Encoding encoding)
        {
            if (!TempDir.Root.Exists)
                TempDir.CreateNew();

            string pathFragment = Path.GetDirectoryName(relativeFilePath);
            if (!string.IsNullOrEmpty(pathFragment))
                TempDir.CreateSubdirectory(pathFragment);

            return CreateNewFile(TempDir.AbsPathFor(relativeFilePath), contents, encoding);
        }


        private static FileInfo CreateNewFile(string filePath, string contents, Encoding encoding)
        {
            using (var f = new FileStream(filePath, FileMode.CreateNew))
            {
                if (!string.IsNullOrEmpty(contents))
                {
                    StreamWriter s;
                    if (encoding == null)
                        s = new StreamWriter(f);
                    else
                        s = new StreamWriter(f, encoding);
                    s.Write(contents);
                    s.Close();
                    f.Close();
                }
            }
            return new FileInfo(filePath);
        }


        public static FileInfo CreateWithContents()
        {
            return CreateWithContents("Ok to delete this file");
        }


        public static FileInfo CreateWithContents(string contents)
        {
            return Create(Path.GetRandomFileName(), contents, null);
        }


        public static FileInfo CreateWithContents(string contents, string relativeFilePath)
        {
            return Create(relativeFilePath, contents, null);
        }


        public static FileInfo CreateWithContents(string contents, string relativeFilePath, Encoding encoding)
        {
            return Create(relativeFilePath, contents, encoding);
        }


        public static FileInfo CreateWithContents(string contents, Encoding encoding)
        {
            return Create(Path.GetRandomFileName(), contents, encoding);
        }

        #endregion
    }
}