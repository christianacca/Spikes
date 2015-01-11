using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    ///     A database initialiser that will upgrade the database to the latest version as determined
    ///     by one or more <see cref="DbMigrationsConfiguration" />
    /// </summary>
    public class MultiMigrateDbToLastestVersion : IDatabaseInitializer<DbContext>
    {
        private readonly IEnumerable<DbMigrationsConfiguration> _configurations;

        /// <remarks>
        ///     The order of <paramref name="configurations" /> should be least dependent model
        ///     first
        /// </remarks>
        /// <param name="configurations">
        ///     Defines the migrations to run. The order of <paramref name="configurations" />
        ///     should be least dependent model first
        /// </param>
        public MultiMigrateDbToLastestVersion(IEnumerable<DbMigrationsConfiguration> configurations)
        {
            _configurations = configurations ?? Enumerable.Empty<DbMigrationsConfiguration>();
            Logger = NullMigrationsLogger.Instance;
        }

        /// <summary>
        ///     Used to override the connection to the database to be migrated.
        /// </summary>
        /// <remarks>
        ///     If not supplied the connection information will be taken from a context,
        ///     as determined by the <see cref="DbConfiguration" />, constructed using the context's default constructor
        ///     or registered factory if applicable.
        /// </remarks>
        public string ConnectionStringName { get; set; }

        /// <summary>
        ///     Names of migrations that should be skipped
        /// </summary>
        /// <remarks>
        ///     This is useful when migrations from one configuration should be used in place of one or more migrations
        ///     defined in another configuration
        /// </remarks>
        public IEnumerable<string> SkippedMigrations { get; set; }

        /// <summary>
        ///     When <c>true</c>, the <see cref="DbMigrationsConfiguration{T}.Seed" /> method will be skipped for those
        ///     configurations that do not have any pending migrations to run against the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default value is <c>false</c>; ie the <see cref="DbMigrationsConfiguration{T}.Seed" /> method
        ///         will be run even if there were no pending migrations detected for that configuration.
        ///     </para>
        ///     <para>
        ///         The default behaviour of not skipping migrations is consistent with
        ///         <see cref="MigrateDatabaseToLatestVersion{TContext,TMigrationsConfiguration}" />
        ///     </para>
        /// </remarks>
        public bool SkipSeedWithNoPendingMigrations { get; set; }

        /// <summary>
        ///     Assign the logger that should output generated log messages
        /// </summary>
        /// <remarks>
        ///     By default this will be assigned a <see cref="NullMigrationsLogger" />
        /// </remarks>
        public MigrationsLogger Logger { get; set; }

        public virtual void InitializeDatabase(DbContext context)
        {
            bool migrationsRun = UpgradeDb();
            AdditionalSeed(context, migrationsRun);
            context.SaveChanges();
        }

        private bool UpgradeDb()
        {
            List<DelegatedMigrator> migrators = _configurations.Select(CreateMigrator).ToList();
            using (var runner = new MultiMigrateDbToLastestVsRunner(migrators)
            {
                SkippedMigrations = SkippedMigrations,
                Logger = Logger,
                SkipSeedWithNoPendingMigrations = SkipSeedWithNoPendingMigrations
            })
            {
                return runner.Run();
            }
        }

        private DelegatedMigrator CreateMigrator(DbMigrationsConfiguration c, int migratorPriority)
        {
            if (!String.IsNullOrEmpty(ConnectionStringName))
            {
                c.TargetDatabase = new DbConnectionInfo(ConnectionStringName);
            }
            var impl = new DbMigrator(c);
            return new DelegatedMigrator(impl.GetPendingMigrations, impl.Update)
            {
                IsAutoMigrationsEnabled = c.AutomaticMigrationsEnabled,
                ConfigurationTypeName = c.GetType().FullName, 
                Priority = migratorPriority
            };
        }

        /// <summary>
        ///     Override in subclass to provide additional seed data
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Seed data created by this method will be in addition to the data created by
        ///         <see cref="DbMigrationsConfiguration" />.<see cref="DbMigrationsConfiguration{T}.Seed" />
        ///         methods supplied to this instance.
        ///     </para>
        ///     <para>
        ///         <see cref="DbContext.SaveChanges" /> will be run after this method has finshed.
        ///     </para>
        ///     <para>
        ///         ** WARNING ** Seed data created by this method will not be run when upgrading the database
        ///         using Update-Database command in nuget package console or in an automated deploy scenarios
        ///         using migrate.exe
        ///     </para>
        /// </remarks>
        /// <param name="context">The database context that will used to save seed data</param>
        /// <param name="migrationsRun"><c>true</c> whether migrations were actually run</param>
        public virtual void AdditionalSeed(DbContext context, bool migrationsRun)
        {
            // override in subclass
        }
    }
}