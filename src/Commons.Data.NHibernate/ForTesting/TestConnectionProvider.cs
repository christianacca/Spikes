using System;
using System.Data;
using NHibernate.Connection;

namespace Eca.Commons.Data.NHibernate.ForTesting
{
    /// <summary>
    /// A connection provider that opens and serves up only one connection. Typically used when testing with an in-memory database engine
    /// such as SQLite so that the database will not be dropped until the connection is manually dropped with an explicit call to
    /// <see cref="CloseDatabase"/>
    /// </summary>
    public class TestConnectionProvider : DriverConnectionProvider
    {
        public override void CloseConnection(IDbConnection conn)
        {
            // Do nothing
        }


        public override IDbConnection GetConnection()
        {
            if (Connection == null)
            {
                Connection = Driver.CreateConnection();
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
            return Connection;
        }


        #region Class Members

        [ThreadStatic] private static IDbConnection Connection;


        public static void CloseDatabase()
        {
            if (Connection != null)
                Connection.Dispose();
            Connection = null;
        }

        #endregion
    }
}