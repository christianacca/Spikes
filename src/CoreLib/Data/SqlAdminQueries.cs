using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace Eca.Commons.Data
{
    public static class SqlAdminQueries
    {
        #region DbObjectType enum

        public enum DbObjectType
        {
            [Description("U")] Table,
            [Description("P")] StoredProc
        }

        #endregion


        #region Class Members

        private static readonly DbInstance _db = new DbInstance();


        public static DbInstance Db
        {
            get { return _db; }
        }


        /// <summary>
        /// Can we connect to the specified server
        /// </summary>
        /// <param name="serverName">The server name</param>
        /// <returns>true if we can connect to the server</returns>
        public static bool CanConnectToServer(string serverName)
        {
            try
            {
                var cn = new SqlConnection
                             {
                                 ConnectionString = ("Server=" + serverName + ";Database=;Integrated Security=SSPI")
                             };
                cn.Open();
                cn.Dispose();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }


        public static void CreateDatabase(string serverName, string databaseName, bool drop)
        {
            if (drop)
            {
                DropDatabaseIfExists(serverName, databaseName);
            }

            string createDatabaseScript = "IF (SELECT DB_ID('" + databaseName + "')) IS NULL  "
                                          + " CREATE DATABASE " + databaseName;

            string connectionString = DbConnectionInfo.GetMsSqlServerConnectionString(serverName, "master");
            SimpleDataAccess.ExecuteSql(createDatabaseScript, connectionString);
        }


        /// <summary>
        /// Can we connect to the specified database
        /// </summary>
        /// <param name="connectionString">The SQL connection string</param>
        /// <param name="databaseName">The server name</param>
        /// <returns>true if we can connect to the database</returns>
        public static bool DatabaseExists(string connectionString, string databaseName)
        {
            try
            {
                DataTable dt = SimpleDataAccess.ExecuteDataTable(cmd => {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT ISNULL(HAS_DBACCESS('" + databaseName + "'),-2) AS ReturnValue";
                },
                                                                 connectionString);

                if (dt.Rows.Count == 0)
                {
                    return false;
                }

                DataRow dr = dt.Rows[0];

                if (dr.ItemArray.GetValue(0).ToString() == "-2")
                {
                    return false;
                }
                else if (dr.ItemArray.GetValue(0).ToString() == "0")
                {
                    return false;
                }
                else if (dr.ItemArray.GetValue(0).ToString() == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }


        public static void DropDatabase(string serverName, string databaseName)
        {
            DropUserConnections(serverName, databaseName);

            var connectionInfo = new DbConnectionInfo {DatabaseName = "master", ServerName = serverName};
            string sql = String.Format("DROP DATABASE {0}", databaseName);
            SimpleDataAccess.ExecuteSql(sql, DbConnectionInfo.GetMsSqlServerConnectionString(connectionInfo));

            //Necessary so as to avoid a connection to the database that just been dropped being being reused from the pool
            //Not clearing the pool, leads to your app throwing an exception when it tried to access the dropped database 
            //from a pooled connection that was no longer open
            SqlConnection.ClearAllPools();
        }


        public static void DropDatabaseIfExists(string serverName, string databaseName)
        {
            string connectionString = DbConnectionInfo.GetMsSqlServerConnectionString(serverName, "master");
            if (!DatabaseExists(connectionString, databaseName)) return;
            DropDatabase(serverName, databaseName);
        }


        public static void DropUserConnections(string serverName, string databaseName)
        {
            var connectionInfo = new DbConnectionInfo {DatabaseName = "master", ServerName = serverName};
            string sql = String.Format("ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE", databaseName);
            SimpleDataAccess.ExecuteSql(sql, DbConnectionInfo.GetMsSqlServerConnectionString(connectionInfo));
        }


        public static bool ObjectExistsInDatabase(string connectionString, string objectName)
        {
            string sql = String.Format("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}')", objectName);
            DataTable table
                = SimpleDataAccess.ExecuteDataTable(cmd => {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                },
                                                    connectionString);

            return table.Rows.Count > 0;
        }

        #endregion


        public class DbInstance
        {
            #region Properties

            public string ConnectionString { get; set; }

            #endregion


            public void DropStoredProc(string procName)
            {
                string sql =
                    string.Format("IF  EXISTS ({0}) DROP PROC {1};",
                                  ObjectExistsSql(procName, DbObjectType.StoredProc),
                                  procName);
                SimpleDataAccess.ExecuteSql(sql, ConnectionString);
            }


            public void DropTable(string tableName)
            {
                string sql =
                    String.Format("IF  EXISTS ({0}) DROP TABLE {1};",
                                  ObjectExistsSql(tableName, DbObjectType.Table),
                                  tableName);
                SimpleDataAccess.ExecuteSql(sql, ConnectionString);
            }


            public bool ObjectExists(string tableName, DbObjectType objectType)
            {
                string sql = ObjectExistsSql(tableName, objectType);
                DataTable table
                    = SimpleDataAccess.ExecuteDataTable(cmd => {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                    },
                                                        ConnectionString);

                return table.Rows.Count > 0;
            }


            public bool StoredProcExists(string procName)
            {
                return ObjectExists(procName, DbObjectType.StoredProc);
            }


            public bool TableExists(string tableName)
            {
                return ObjectExists(tableName, DbObjectType.Table);
            }


            #region Class Members

            private static string ObjectExistsSql(string objectName, DbObjectType objectType)
            {
                return String.Format("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') AND type = N'{1}'",
                                     objectName,
                                     objectType.ToDescriptionString());
            }

            #endregion
        }
    }
}