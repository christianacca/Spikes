using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Eca.Commons.Data
{
    /// <summary>
    /// Connection string information such as database name and server
    /// </summary>
    public class DbConnectionInfo : IEquatable<DbConnectionInfo>
    {
        #region Properties

        public string DatabaseName { get; set; }
        public string ServerName { get; set; }

        #endregion


        #region IEquatable<DbConnectionInfo> Members

        public bool Equals(DbConnectionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.DatabaseName, DatabaseName) && Equals(other.ServerName, ServerName);
        }

        #endregion


        public DbConnectionInfo Merge(DbConnectionInfo other)
        {
            return new DbConnectionInfo
                       {
                           DatabaseName = String.IsNullOrEmpty(other.DatabaseName) ? DatabaseName : other.DatabaseName,
                           ServerName = String.IsNullOrEmpty(other.ServerName) ? ServerName : other.ServerName
                       };
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DbConnectionInfo)) return false;
            return Equals((DbConnectionInfo) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return ((DatabaseName != null ? DatabaseName.GetHashCode() : 0)*397) ^
                       (ServerName != null ? ServerName.GetHashCode() : 0);
            }
        }


        public override string ToString()
        {
            return String.IsNullOrEmpty(ServerName) ? DatabaseName : string.Format("{0}.{1}", ServerName, DatabaseName);
        }

        #endregion


        #region Class Members

        static DbConnectionInfo()
        {
            LocalSqlServerInstanceNames = GetLocalSqlServerInstanceNames().ToList();

            if (!LocalSqlServerInstanceNames.Contains("localhost"))
            {
                DefaultSqlServerName = LocalSqlServerInstanceNames.FirstOrDefault();
            }
            DefaultSqlServerName = DefaultSqlServerName ?? "localhost";
        }


        /// <summary>
        /// The name of the sql server instance that will be used when building a connection string when the server name has not been supplied.
        /// </summary>
        /// <remarks>
        /// An initial name will be assigned to this property based on the sql server instances installed on this machine: 
        /// if there are a choice of instances installed on this machine, then localhost will be used in preference.
        /// If there are only named instances, the first of these as defined in the registry will be used
        /// </remarks>
        public static string DefaultSqlServerName { get; set; }

        /// <summary>
        /// Names of the sql server instances installed on this machine. 
        /// 'localhost' is used to denote the default instance (ie a default installation that was not named)
        /// </summary>
        public static IEnumerable<string> LocalSqlServerInstanceNames { get; set; }


        private static IEnumerable<string> GetLocalSqlServerInstanceNames()
        {
            object rawValue;
            try
            {
                rawValue = Registry64.LocalMachine.GetValue(@"SOFTWARE\Microsoft\Microsoft SQL Server",
                                                            "InstalledInstances");
            }
            catch (Win32Exception)
            {
                yield break;
            }

            if (!(rawValue is string[]))
            {
                yield return rawValue.ToString();
            }

            var instanceNames = (string[]) rawValue;
            foreach (var name in instanceNames)
            {
                if (name.Equals("MSSQLSERVER", StringComparison.InvariantCultureIgnoreCase))
                    yield return "localhost";
                else
                    yield return name;
            }
        }


        public static string GetMsSqlServerConnectionString(DbConnectionInfo connectionInfo)
        {
            return GetMsSqlServerConnectionString(connectionInfo.ServerName, connectionInfo.DatabaseName);
        }


        public static string GetMsSqlServerConnectionString(string serverName, string databaseName)
        {
            return String.Format("Server={0};initial catalog={1};Integrated Security=SSPI",
                                 serverName ?? DefaultSqlServerName,
                                 databaseName);
        }


        public static string SqliteConnectionString(string databaseName)
        {
            return String.Format("Data Source={0};Version=3;New=True;", databaseName);
        }

        #endregion


        public static implicit operator DbConnectionInfo(string databaseName)
        {
            return new DbConnectionInfo {DatabaseName = databaseName};
        }


        public static bool operator ==(DbConnectionInfo left, DbConnectionInfo right)
        {
            return Equals(left, right);
        }


        public static bool operator !=(DbConnectionInfo left, DbConnectionInfo right)
        {
            return !Equals(left, right);
        }
    }
}