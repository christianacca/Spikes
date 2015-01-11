using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    /// A Migrator which delegates the work of running migrations to another implementation
    /// </summary>
    public class DelegatedMigrator: IDisposable
    {
        private readonly Action _disposeImpl;

        public DelegatedMigrator(Func<IEnumerable<string>> getPendingMigrations, Action<string> update, Action dispose = null)
        {
            _disposeImpl = dispose ?? (() =>
            {
                // noop
            });
            GetPendingMigrations = getPendingMigrations;
            Update = update;
        }

        /// <summary>
        /// The type name of the <see cref="DbMigrationsConfiguration"/> that defines the migrations to run
        /// </summary>
        public string ConfigurationTypeName { get; set; }

        /// <summary>
        /// Returns the names of the pending migrations outstanding against the database
        /// </summary>
        public Func<IEnumerable<string>> GetPendingMigrations { get; private set; }

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
        /// Runs the migrations against the <see cref="DbMigrationsConfiguration.TargetDatabase"/>
        /// </summary>
        public Action<string> Update { get; private set; }

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