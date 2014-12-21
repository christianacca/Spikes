using System;
using System.Security.Principal;

namespace Eca.Commons.DomainLayer
{
    /// <summary>
    /// Don't like this design for the following reasons:
    /// <list type="bullet">
    /// <item>The hard coded lookup to WindowsIdentity - does not cater for FormsAuthentication etc</item>
    /// <item>The indeterminant nature of when the Current user is set ie when this class is first loaded 
    /// into the app domain - would prefer this to explicit at the start of an app</item>
    /// </list>
    /// </summary>
    public static class User
    {
        #region Class Members

        public static readonly IUser UnknownUser = new SimpleUser {Authenticated = false, Username = "Unknown"};


        static User()
        {
            Current = FromCurrentWindowsIdentity ?? UnknownUser;
        }


        public static IUser Current { get; set; }

        public static string CurrentUserName
        {
            get { return Current.Username; }
        }

        public static IUser FromCurrentWindowsIdentity
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity == null) return null;

                return new SimpleUser
                           {
                               Authenticated = identity.IsAuthenticated,
                               Username = RemoveDomainFromIdentity(identity.Name)
                           };
            }
        }


        private static string RemoveDomainFromIdentity(string name)
        {
            // Parse the string to check if domain name is present.
            int idx = name.IndexOf('\\');
            if (idx == -1)
            {
                idx = name.IndexOf('@');
            }

            string loginName;

            if (idx != -1)
            {
                loginName = name.Substring(idx + 1);
            }
            else
            {
                loginName = name;
            }
            return loginName;
        }

        #endregion


        private class SimpleUser : IUser
        {
            #region IUser Members

            public bool Authenticated { get; set; }
            public string Username { get; set; }

            #endregion
        }
    }
}