using System.Linq;

namespace Eca.Commons.Extensions
{
    public static class UserExtensions
    {
        #region Class Members

        public static string BaseUserName(this string username)
        {
            if (string.IsNullOrEmpty(username)) return string.Empty;

            string[] usernameParts = username.Split('\\');
            return usernameParts.Last();
        }

        #endregion
    }
}