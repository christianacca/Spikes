using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using Eca.Commons.Data.SelectQueryBuilder;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Data
{
    /// <summary>
    /// Helper class to provide simple data access, when we want to access the ADO.Net library directly 
    /// without setting a depedency on other libraries such as EnterpriseLibrary
    /// </summary>
    public static class SimpleDataAccess
    {
        #region Class Members

        private static T CreateAdoObject<T>(Func<T> factoryMethod) where T : class
        {
            T dbCommand = factoryMethod();
            if (dbCommand == null)
            {
                throw new AssertionException(
                    String.Format("Expected to be able to create a {0} object from the DbProviderFactory supplied",
                                  typeof (T).Name));
            }
            return dbCommand;
        }


        private static DbCommand CreateCommand(DbConnection openConnection,
                                               Action<DbCommand> commandConfigurator)
        {
            var dbCommand = openConnection.CreateCommand();
            commandConfigurator(dbCommand);
            return dbCommand;
        }


        /// <summary>
        /// Execute an <see cref="SqlCommand"/>, configured using <paramref name="commandConfigurator"/>, against the database identified by 
        /// <paramref name="connectionString"/>, returning the resulting DataSet returned by the database.
        /// </summary>
        /// <seealso cref="ExecuteDataSet(System.Action{System.Data.Common.DbCommand},string,System.Data.Common.DbProviderFactory)"/>
        public static DataSet ExecuteDataSet(Action<DbCommand> commandConfigurator, string connectionString)
        {
            return ExecuteDataSet(commandConfigurator, connectionString, null);
        }


        /// <summary>
        /// Execute an ADO command, configured using <paramref name="commandConfigurator"/>, against the database identified by 
        /// <paramref name="connectionString"/>, returning the resulting DataSet returned by the database.
        /// </summary>
        /// <param name="commandConfigurator">
        /// A delegate that is reponsible for performing the neccessary configuration of the <see cref="DbCommand"/> that will be executed against 
        /// the database. At a minimum you will need to set the <see cref="DbCommand.CommandType"/> and <see cref="DbCommand.CommandText"/>
        /// </param>
        /// <param name="connectionString">The connection string that identifies the database</param>
        /// <param name="adoFactory">
        /// The ADO factory that should be used to create the ADO objects necessary for accessing the database. 
        /// Defaults to <see cref="SqlClientFactory"/> if not supplied.
        /// </param>
        public static DataSet ExecuteDataSet(Action<DbCommand> commandConfigurator,
                                             string connectionString,
                                             DbProviderFactory adoFactory)
        {
#if !SILVERLIGHT

            Check.Require(() => Demand.The.Param(() => commandConfigurator).IsNotNull(),
                          () => Demand.The.Param(() => connectionString).IsNotNull());
#else
            if (commandConfigurator == null) throw new PreconditionException("commandConfigurator is mandatory");
            if (connectionString == null) throw new PreconditionException("connectionString is mandatory");
#endif

            if (adoFactory == null) adoFactory = SqlClientFactory.Instance;

            using (DbConnection cnn = OpenConnection(adoFactory, connectionString))
            using (DbCommand dbCommand = CreateCommand(cnn, commandConfigurator))
// ReSharper disable RedundantTypeArgumentsOfMethod
            using (var adapter = CreateAdoObject<DbDataAdapter>(adoFactory.CreateDataAdapter))
// ReSharper restore RedundantTypeArgumentsOfMethod
            {
                adapter.SelectCommand = dbCommand;
                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                return dataSet;
            }
        }


        /// <summary>
        /// Execute an <see cref="SqlCommand"/>, configured using <paramref name="commandConfigurator"/>, against the database identified by 
        /// <paramref name="connectionString"/>, returning the resulting DataTable returned by the database.
        /// </summary>
        /// <seealso cref="ExecuteDataSet(System.Action{System.Data.Common.DbCommand},string,System.Data.Common.DbProviderFactory)"/>
        public static DataTable ExecuteDataTable(Action<DbCommand> commandConfigurator, string connectionString)
        {
            return ExecuteDataTable(commandConfigurator, connectionString, null);
        }


        /// <summary>
        /// Execute an ADO command, configured using <paramref name="commandConfigurator"/>, against the database identified by 
        /// <paramref name="connectionString"/>, returning the resulting DataTable returned by the database.
        /// </summary>
        /// <seealso cref="ExecuteDataSet(System.Action{System.Data.Common.DbCommand},string,System.Data.Common.DbProviderFactory)"/>
        public static DataTable ExecuteDataTable(Action<DbCommand> commandConfigurator,
                                                 string connectionString,
                                                 DbProviderFactory adoFactory)
        {
            return ExecuteDataSet(commandConfigurator, connectionString, adoFactory).Tables[0];
        }


        public static void ExecuteEveryBatchInScript(string script, string connectionString)
        {
            foreach (string batch in GetBatchesIn(script))
            {
                string sql = batch;
                ExecuteNonQuery(cmd => {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                },
                                connectionString);
            }
        }


        /// <summary>
        /// Uses <see cref="TransactionScope"/> to execute <paramref name="actionToExecute"/> within a transaction
        /// with <see cref="TransactionScopeOption.Required"/> semantics
        /// </summary>
        /// <param name="actionToExecute">the action to execute</param>
        public static void ExecuteInsideTransaction(Action actionToExecute)
        {
            using (var txn = new TransactionScope(TransactionScopeOption.Required))
            {
                actionToExecute();
                txn.Complete();
            }
        }


        /// <summary>
        /// Execute an ADO command, configured using <paramref name="commandConfigurator"/>, against the database identified by 
        /// <paramref name="connectionString"/>
        /// </summary>
        /// <seealso cref="ExecuteNonQuery(System.Action{System.Data.Common.DbCommand},string,System.Data.Common.DbProviderFactory)"/>
        public static void ExecuteNonQuery(Action<DbCommand> commandConfigurator,
                                           string connectionString)
        {
            ExecuteNonQuery(commandConfigurator, connectionString, null);
        }


        /// <summary>
        /// Execute an ADO command, configured using <paramref name="commandConfigurator"/>, against the database identified by 
        /// <paramref name="connectionString"/>
        /// </summary>
        /// <param name="commandConfigurator">
        /// A delegate that is reponsible for performing the neccessary configuration of the <see cref="DbCommand"/> that will be executed against 
        /// the database. At a minimum you will need to set the <see cref="DbCommand.CommandType"/> and <see cref="DbCommand.CommandText"/>
        /// </param>
        /// <param name="connectionString">The connection string that identifies the database</param>
        /// <param name="adoFactory">
        /// The ADO factory that should be used to create the ADO objects necessary for accessing the database. 
        /// Defaults to <see cref="SqlClientFactory"/> if not supplied.
        /// </param>
        public static void ExecuteNonQuery(Action<DbCommand> commandConfigurator,
                                           string connectionString,
                                           DbProviderFactory adoFactory)
        {
#if !SILVERLIGHT

            Check.Require(() => Demand.The.Param(() => commandConfigurator).IsNotNull(),
                          () => Demand.The.Param(() => connectionString).IsNotNull());
#else
            if (commandConfigurator == null) throw new PreconditionException("commandConfigurator is mandatory");
            if (connectionString == null) throw new PreconditionException("connectionString is mandatory");
#endif

            if (adoFactory == null) adoFactory = SqlClientFactory.Instance;

            using (DbConnection cnn = OpenConnection(adoFactory, connectionString))
            using (DbCommand dbCommand = CreateCommand(cnn, commandConfigurator))
            {
                dbCommand.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Execute the <paramref name="sql"/> against the database identified by <paramref name="connectionString"/>
        /// </summary>
        public static void ExecuteSql(string sql, string connectionString)
        {
            ExecuteNonQuery(cmd => {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
            },
                            connectionString);
        }


        /// <summary>
        /// Execute the <paramref name="sql"/> against the already open connection. Note: the <paramref name="openConnection"/> supplied will not be closed,
        /// this is the responsibility of the caller
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <param name="openConnection">An already open connection</param>
        public static void ExecuteSql(string sql, IDbConnection openConnection)
        {
            using (IDbCommand command = openConnection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// See <see cref="WhereStatement.FormatSqlValue"/>
        /// </summary>
        public static string FormatSqlValue(object value)
        {
            return WhereStatement.FormatSqlValue(value);
        }


        /// <summary>
        /// Splits the script on every GO command
        /// </summary>
        public static string[] GetBatchesIn(string script)
        {
            var regex = new Regex(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Split(script).Where(s => !String.IsNullOrEmpty(s)).ToArray();
        }


        private static DbConnection OpenConnection(DbProviderFactory adoFactory, string connectionString)
        {
// ReSharper disable RedundantTypeArgumentsOfMethod
            var cnn = CreateAdoObject<DbConnection>(adoFactory.CreateConnection);
// ReSharper restore RedundantTypeArgumentsOfMethod
            cnn.ConnectionString = connectionString;
            cnn.Open();
            return cnn;
        }

        #endregion
    }
}