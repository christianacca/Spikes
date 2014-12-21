using System.Data.Common;

namespace Eca.Commons.Extensions
{
    public static class DataConnectionExtensions
    {
        #region Class Members

        /// <summary>
        /// Adds the ability to chain a call to <see cref="DbConnection.Open"/> with further method
        /// calls on the connection
        /// and then returns
        /// </summary>
        public static DbConnection OpenAndReturn(this DbConnection source)
        {
            source.Open();
            return source;
        }

        #endregion
    }
}