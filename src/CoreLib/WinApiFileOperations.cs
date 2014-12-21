using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Eca.Commons
{
    public static class WinApiFileOperations
    {
        #region Class Members

        public static string GetLongName(string shortFileName)
        {
            var buffer = new StringBuilder(259);
            int len = GetLongPathName(shortFileName, buffer, buffer.Capacity);
            if (len == 0) throw new Win32Exception();
            return buffer.ToString();
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetLongPathName(string path, StringBuilder longPath, int longPathLength);


        public static string GetShortName(string longFileName)
        {
            var buffer = new StringBuilder(259);
            int len = GetShortPathName(longFileName, buffer, buffer.Capacity);
            if (len == 0) throw new Win32Exception();
            return buffer.ToString();
        }


        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string longPath, StringBuilder shortPath, int bufSize);

        #endregion
    }
}