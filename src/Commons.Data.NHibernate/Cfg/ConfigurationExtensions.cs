using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using NHibernate.Dialect;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    public static class ConfigurationExtensions
    {
        #region Class Members

        public static void AddPropertyIfMissing(this Configuration configs, string propertyName, string propertyValue)
        {
            if (!String.IsNullOrEmpty(configs.GetProperty(propertyName))) return;
            configs.Properties.Add(propertyName, propertyValue);
        }


        public static DbConnectionStringBuilder GetConnectionStringBuilder(this Configuration cfg)
        {
            DatabaseEngine? databaseEngine = cfg.GetDatabaseEngine();
            if (databaseEngine == null) return null;

            string connectionString = cfg.GetProperty(Environment.ConnectionString) ??
                                      GetConnectionStringFromNamedConnection(cfg);
            if (connectionString == null) return null;

            return databaseEngine == DatabaseEngine.MsSql2005
                       ? new SqlConnectionStringBuilder(connectionString)
                       : new DbConnectionStringBuilder {ConnectionString = connectionString};
        }


        private static string GetConnectionStringFromNamedConnection(Configuration cfg)
        {
            var connectionName = cfg.GetProperty(Environment.ConnectionStringName);
            var connectionString =
                EntLibConnectionStringForNHibernate.GetConnectionStringFromEntLibConfigFile(connectionName);
            if (connectionString != null)
            {
                return connectionString;
            }
            else
            {
                ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings[connectionName];
                return connectionSettings != null ? connectionSettings.ConnectionString : null;
            }
        }


        public static DatabaseEngine? GetDatabaseEngine(this Configuration cfg)
        {
            string dialectClassName = cfg.GetProperty(Environment.Dialect);
            if (String.IsNullOrEmpty(dialectClassName)) return null;
            if (dialectClassName.Equals(typeof (MsSql2005Dialect).FullName) ||
                dialectClassName.Equals(typeof (MsSql2008Dialect).FullName))
            {
                return DatabaseEngine.MsSql2005;
            }
            else if (dialectClassName.Equals(typeof (MsSqlCeDialect).FullName))
            {
                return DatabaseEngine.MsSqlCe;
            }
            else if (dialectClassName.Equals(typeof (SQLiteDialect).FullName))
            {
                return DatabaseEngine.SQLite;
            }
            else
            {
                return null;
            }
        }


        public static DbConnectionInfo GetDbConnectionInfo(this Configuration cfg)
        {
            DbConnectionStringBuilder connectionStringBuilder = cfg.GetConnectionStringBuilder();
            if (connectionStringBuilder == null) return null;

            var sqlConnectionStringBuilder = connectionStringBuilder as SqlConnectionStringBuilder;
            if (sqlConnectionStringBuilder != null)
            {
                return new DbConnectionInfo
                           {
                               DatabaseName = sqlConnectionStringBuilder.InitialCatalog,
                               ServerName = sqlConnectionStringBuilder.DataSource
                           };
            }
            else
            {
                return new DbConnectionInfo {DatabaseName = (string) connectionStringBuilder["Data Source"]};
            }
        }


        public static void SetDbConnectionInfo(this Configuration cfg, DbConnectionInfo dbConnectionInfo)
        {
            var connectionStringBuilder = cfg.GetConnectionStringBuilder() as SqlConnectionStringBuilder;
            if (connectionStringBuilder == null)
            {
                throw new NotImplementedException("Setting database connection currently only supported for Sql Server");
            }
            if (!String.IsNullOrEmpty(dbConnectionInfo.DatabaseName))
            {
                connectionStringBuilder.InitialCatalog = dbConnectionInfo.DatabaseName;
            }
            if (!String.IsNullOrEmpty(dbConnectionInfo.ServerName))
            {
                connectionStringBuilder.DataSource = dbConnectionInfo.ServerName;
            }
            cfg.SetProperty(Environment.ConnectionString, connectionStringBuilder.ConnectionString);
        }

        #endregion
    }
}