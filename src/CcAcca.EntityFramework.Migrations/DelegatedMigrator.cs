using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Migrations;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    /// A Migrator which delegates the work of running migrations to another implementation
    /// </summary>
    public class DelegatedMigrator: IDisposable
    {
        private readonly Func<IEnumerable<string>> _getPendingMigrationsImpl;
        private readonly Func<IEnumerable<string>> _getGetDatabaseMigrationsImpl;
        private readonly Action<string> _updateImpl;
        private readonly DbConnection _connection;
        private readonly Action _disposeImpl;
        private readonly MigrationTblInsertScripter _migrationTblInsertScripter;

        public DelegatedMigrator(Func<IEnumerable<string>> getPendingMigrationsImpl, Func<IEnumerable<string>> getGetDatabaseMigrationsImpl, Action<string> updateImpl, Func<string, string, string> scriptUpdateImpl, DbConnection connection, Action dispose = null)
        {
            _getPendingMigrationsImpl = getPendingMigrationsImpl;
            _getGetDatabaseMigrationsImpl = getGetDatabaseMigrationsImpl;
            _updateImpl = updateImpl;
            _migrationTblInsertScripter = new MigrationTblInsertScripter(scriptUpdateImpl);
            _connection = connection;
            _disposeImpl = dispose ?? (() =>
            {
                // noop
            });
        }

        /// <summary>
        /// The type name of the <see cref="DbMigrationsConfiguration"/> that defines the migrations to run
        /// </summary>
        public string ConfigurationTypeName { get; set; }

        /// <summary>
        /// Returns the names of the migrations that have been applied to the target database
        /// </summary>
        public IEnumerable<string> GetDatabaseMigrations()
        {
            return _getGetDatabaseMigrationsImpl();
        }

        /// <summary>
        /// Returns the names of the pending migrations outstanding against the database
        /// </summary>
        public IEnumerable<string> GetPendingMigrations()
        {
            return _getPendingMigrationsImpl();
        }

        /// <summary>
        /// Derived from <see cref="DbMigrationsConfiguration.AutomaticMigrationsEnabled"/>
        /// </summary>
        public bool IsAutoMigrationsEnabled { get; set; }

        /// <summary>
        /// Determines the order in which a migration from this migrator should run when there
        /// is a tie on the <see cref="MigrationInfo.CreatedOn"/> date of a collection of
        /// migrations
        /// </summary>
        public int Priority { get; set; }


        /// <summary>
        ///     Insert only the snapshot of the model into the migration history table
        /// </summary>
        /// <param name="previousMigrationName">
        ///     Previous migration required in order to create an sql script of just the target
        ///     migration
        /// </param>
        /// <param name="migrationName">The target migration whose model snapshot is to be inserted</param>
        public void InsertMigrationHistory(string previousMigrationName, string migrationName)
        {
            bool mustClose = _connection.State != ConnectionState.Open;
            try
            {
                _connection.Open();

                DbCommand cmd = _connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = _migrationTblInsertScripter.GetMigrationHistoryInsertSql(previousMigrationName, migrationName);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (mustClose) _connection.Close();
            }
        }

        /// <summary>
        /// Runs the migrations against the <see cref="DbMigrationsConfiguration.TargetDatabase"/>
        /// </summary>
        public void Update(string targetMigration)
        {
            _updateImpl(targetMigration);
        }


        public override string ToString()
        {
            return String.Format("ConfigurationTypeName: {0}, Priority: {1}", ConfigurationTypeName, Priority);
        }

        public void Dispose()
        {
            _disposeImpl();
        }
    }
}