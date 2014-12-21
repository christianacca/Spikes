using System;

namespace Eca.Commons.Extensions
{
    public static class BoolExtensions
    {
        #region Class Members

        public static string YesNo(this bool source)
        {
            return source ? "Yes" : "No";
        }

        #endregion
    }
}