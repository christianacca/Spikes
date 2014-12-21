using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Eca.Commons
{
    ///<summary>
    /// Can be used to access the 64-bit registry regardless of the process
    /// the code is actually running in.
    ///</summary>
    public class Registry64
    {
        #region Member Variables

        private const int KEY_WOW64_32KEY = 0x200;
        private const int KEY_WOW64_64KEY = 0x100;
        private const int READ_RIGHTS = 131097;
        private const int REG_BINARY = 3;
        private const int REG_DWORD = 4;
        private const int REG_DWORD_BIG_ENDIAN = 5;
        private const int REG_EXPAND_SZ = 2;
        private const int REG_MULTI_SZ = 7;
        private const int REG_NONE = 0;
        private const int REG_QWORD = 11;
        private const int REG_SZ = 1;
        private readonly IntPtr _hKey;

        #endregion


        #region Constructors

        ///<summary>
        /// Initializes a new instance of the <see cref="Registry64"/> class.
        ///</summary>
        ///<param name="_hKey">The base HKey</param>
        private Registry64(IntPtr _hKey)
        {
            this._hKey = _hKey;
        }

        #endregion


        ///<summary>
        /// Returns the value data for the specified value name under the
        /// given sub key.
        ///</summary>
        ///<param name="subKey">The sub key to look under.</param>
        ///<param name="valueName">The name fo the value to retrieve.</param>
        ///<returns>The value data as a string.</returns>
        public object GetValue(string subKey, string valueName)
        {
            IntPtr openKey = OpenSubKey(subKey);

            try
            {
                return RegQueryValue(openKey, valueName);
            }
            finally
            {
                RegCloseKey(openKey);
            }
        }


        private IntPtr OpenSubKey(string subKey)
        {
            IntPtr openKey;
            int resultCode = RegOpenKeyEx(_hKey, subKey, 0, KEY_WOW64_64KEY | READ_RIGHTS, out openKey);
            if (2 == resultCode)
                resultCode = RegOpenKeyEx(_hKey, subKey, 0, KEY_WOW64_32KEY | READ_RIGHTS, out openKey);
            if (resultCode != 0) ThrowException(resultCode);
            return openKey;
        }


        private object RegQueryValue(IntPtr key, string value)
        {
            int error, type, dataLength = 0xfde8;
            int returnLength = dataLength;
            var data = new byte[dataLength];
            while ((error = RegQueryValueEx(key, value, IntPtr.Zero, out type, data, ref returnLength)) == 0xea)
            {
                dataLength *= 2;
                returnLength = dataLength;
                data = new byte[dataLength];
            }
            if (error != 0) ThrowException(error);

            switch (type)
            {
                case REG_NONE:
                case REG_BINARY:
                    return data;
                case REG_DWORD:
                    return (((data[0] | (data[1] << 8)) | (data[2] << 16)) | (data[3] << 24));
                case REG_DWORD_BIG_ENDIAN:
                    return (((data[3] | (data[2] << 8)) | (data[1] << 16)) | (data[0] << 24));
                case REG_QWORD:
                    {
                        var numLow = (uint) (((data[0] | (data[1] << 8)) | (data[2] << 16)) | (data[3] << 24));
                        var numHigh = (uint) (((data[4] | (data[5] << 8)) | (data[6] << 16)) | (data[7] << 24));
                        return (long) (((ulong) numHigh << 32) | numLow);
                    }
                case REG_SZ:
                case REG_EXPAND_SZ:
                    {
                        string queryValue = Encoding.Unicode.GetString(data, 0, returnLength);
                        //remove termination character if present
                        return queryValue.EndsWith("\0") ? queryValue.Substring(0, queryValue.Length - 1) : queryValue;
                    }
                case REG_MULTI_SZ:
                    {
                        var strings = new List<string>();
                        string packed = Encoding.Unicode.GetString(data, 0, returnLength);
                        int start = 0;
                        int end = packed.IndexOf('\0', start);
                        while (end > start)
                        {
                            strings.Add(packed.Substring(start, end - start));
                            start = end + 1;
                            end = packed.IndexOf('\0', start);
                        }
                        return strings.ToArray();
                    }
                default:
                    throw new NotSupportedException();
            }
        }


        private void ThrowException(int errorCode)
        {
            switch (errorCode)
            {
                case 2:
                    throw new Win32Exception(2, "Error 2: Key or value name not found.");
                case 3:
                    throw new Win32Exception(3, "Error 3: Path not found.");
                case 5:
                    throw new Win32Exception(5, "Error 5: Access is denied.");
                case 6:
                    throw new Win32Exception(6, "Error 6: Invalid handle");
                case 9:
                    throw new Win32Exception(9, "Error 9: Invalid block");
                case 12:
                    throw new Win32Exception(12, "Error 12: Invalid Access");
                default:
                    throw new Win32Exception(errorCode,
                                             "Error " + errorCode +
                                             ". Please refer to msdn documention on Winerror.h for further information.");
            }
        }


        #region Class Members

        public static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(-2147483648);
        public static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);
        public static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
        public static readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);


        private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
        public static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);
        public static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);


        ///<summary>
        /// Initializes a new instance of the <see cref="Registry64"/> class
        /// with HKEY_LOCAL_MACHINE set as the HKey.
        ///</summary>
        public static Registry64 LocalMachine
        {
            get { return new Registry64(HKEY_LOCAL_MACHINE); }
        }


        [DllImport("advapi32.dll")]
        private static extern int RegCloseKey(IntPtr hKey);


        [DllImport("Advapi32.dll", EntryPoint = "RegOpenKeyExW", CharSet = CharSet.Unicode)]
        private static extern int RegOpenKeyEx(IntPtr hKey,
                                               [In] string lpSubKey,
                                               int ulOptions,
                                               int samDesired,
                                               out IntPtr phkResult);


        [DllImport("Advapi32.dll", EntryPoint = "RegQueryValueExW", CharSet = CharSet.Unicode)]
        private static extern int RegQueryValueEx(IntPtr hKey,
                                                  [In] string lpValueName,
                                                  IntPtr lpReserved,
                                                  out int lpType,
                                                  [Out] byte[] lpData,
                                                  ref int lpcbData);

        #endregion
    }
}