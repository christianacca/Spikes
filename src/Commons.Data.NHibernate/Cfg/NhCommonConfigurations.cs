using System;
using System.Collections.Generic;
using Environment = NHibernate.Cfg.Environment;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    /// <summary>
    /// Convenient sets of nhibernate configuration for various <see cref="DatabaseEngine"/>s
    /// </summary>
    public static class NhCommonConfigurations
    {
        #region Class Members

        public static string SQLiteDbName = ":memory:";


        static NhCommonConfigurations()
        {
            ByteCodeProviderName = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";
        }


        public static string ByteCodeProviderName { get; set; }


        public static IDictionary<string, string> For(DatabaseEngine databaseEngine,
                                                      DbConnectionInfo connectionInfo,
                                                      bool isForTesting)
        {
            switch (databaseEngine)
            {
                case DatabaseEngine.SQLite:
                    return Sqlite(connectionInfo, true);
                case DatabaseEngine.MsSqlCe:
                    return MsSqlCe(connectionInfo);
                case DatabaseEngine.MsSql2005:
                    return MsSqlServer(connectionInfo);
                default:
                    throw new ArgumentOutOfRangeException("databaseEngine");
            }
        }


        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> donor)
        {
            foreach (KeyValuePair<TKey, TValue> x in donor)
            {
                target[x.Key] = x.Value;
            }
        }


        public static IDictionary<string, string> MsSqlCe(DbConnectionInfo connectionInfo)
        {
            string connectionString = String.Format("Data Source={0};", connectionInfo.DatabaseName);
            return new Dictionary<string, string>
                       {
                           {Environment.ConnectionDriver, "NHibernate.Driver.SqlServerCeDriver"},
                           {Environment.Dialect, "NHibernate.Dialect.MsSqlCeDialect"},
                           {Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider"},
                           {Environment.ConnectionString, connectionString},
                           {Environment.ShowSql, "true"},
                           {Environment.FormatSql, "true"},
                           {Environment.ReleaseConnections, "on_close"},
                           {Environment.Hbm2ddlKeyWords, "none"},
                           {Environment.ProxyFactoryFactoryClass, ByteCodeProviderName}
                       };
        }


        public static IDictionary<string, string> MsSqlServer(DbConnectionInfo connectionInfo)
        {
            return new Dictionary<string, string>
                       {
                           {Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver"},
                           {Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect"},
                           {Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider"},
                           {
                               Environment.ConnectionString,
                               DbConnectionInfo.GetMsSqlServerConnectionString(connectionInfo)
                               },
                           {Environment.ShowSql, "true"},
                           {Environment.FormatSql, "true"},
                           {Environment.ReleaseConnections, "on_close"},
                           //by specifying a default schema, nhibernate's dynamic sql queries benefit from caching
                           {Environment.DefaultSchema, String.Format("{0}.dbo", connectionInfo.DatabaseName)},
                           {Environment.Hbm2ddlKeyWords, "none"},
                           {Environment.ProxyFactoryFactoryClass, ByteCodeProviderName}
                       };
        }


        public static IDictionary<string, string> Sqlite(DbConnectionInfo connectionInfo, bool isForTesting)
        {
            string databaseName = isForTesting ? SQLiteDbName : connectionInfo.DatabaseName;
            return
                new Dictionary<string, string>
                    {
                        {Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver"},
                        {Environment.Dialect, "NHibernate.Dialect.SQLiteDialect"},
                        {
                            Environment.ConnectionProvider,
                            "Eca.Commons.Data.NHibernate.ForTesting.TestConnectionProvider, Eca.Commons.Data.NHibernate"
                            },
                        {Environment.ConnectionString, DbConnectionInfo.SqliteConnectionString(databaseName)},
                        {Environment.ShowSql, "true"},
                        {Environment.FormatSql, "true"},
                        {Environment.ReleaseConnections, "on_close"},
                        {Environment.Hbm2ddlKeyWords, "none"},
                        {Environment.ProxyFactoryFactoryClass, ByteCodeProviderName}
                    };
        }

        #endregion
    }
}