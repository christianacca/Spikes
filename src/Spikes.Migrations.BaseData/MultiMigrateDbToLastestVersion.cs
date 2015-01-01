using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Spikes.Migrations.BaseData
{
    /// <summary>
    ///     A database initialiser that will upgrade the database to the latest version as determined
    ///     by one or more <see cref="DbMigrationsConfiguration" />
    /// </summary>
    public class MultiMigrateDbToLastestVersion : IDatabaseInitializer<DbContext>
    {
        private readonly IEnumerable<DbMigrationsConfiguration> _configurations;

        /// <remarks>
        ///     The migrations defined by each <see cref="DbMigrationsConfiguration" /> will be executed fully
        ///     before the next <see cref="DbMigrationsConfiguration" /> is selected to run, therefore
        ///     the order of <paramref name="configurations" /> list is critical
        /// </remarks>
        /// <param name="configurations">Defines the migrations to run</param>
        public MultiMigrateDbToLastestVersion(IEnumerable<DbMigrationsConfiguration> configurations)
        {
            _configurations = configurations ?? Enumerable.Empty<DbMigrationsConfiguration>();
            Logger = NullMigrationsLogger.Instance;
            FailOnMissingCurrentMigration = true;
        }

        /// <summary>
        ///     When set to <c>true</c>, any existing database will be dropped first before migrations are run
        /// </summary>
        /// <remarks>
        ///     The default value is <c>false</c>
        /// </remarks>
        public bool DropDatabase { get; set; }

        /// <summary>
        ///     When <c>true</c> (the default), fail the upgrade if the current code model is different to model stored in 
        ///     the latest migration
        /// </summary>
        /// <remarks>
        ///     The default behaviour of failing the migration is consistent with
        ///     <see cref="MigrateDatabaseToLatestVersion{TContext,TMigrationsConfiguration}" />
        /// </remarks>
        public bool FailOnMissingCurrentMigration { get; set; }

        /// <summary>
        ///     If set to <c>true</c> the initializer is run using the connection information from the context that
        ///     triggered initialization. Otherwise, the connection information will be taken from a context constructed
        ///     using the default constructor or registered factory if applicable.
        /// </summary>
        /// <remarks>
        ///     The default value is <c>false</c>
        /// </remarks>
        public bool UseSuppliedContext { get; set; }

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
            if (DropDatabase && context.Database.Exists())
            {
                Logger.Verbose("Existing database detected and DropDatabase is set to 'true'.. Dropping database");
                context.Database.Delete();
                Logger.Info("Existing database dropped");
            }

            var migrations =
                (from m in _configurations.Select(c => CreateMigrator(c, context))
                    let pendingMigration = m.GetPendingMigrations().LastOrDefault()
                    select
                        new
                        {
                            migrator = m,
                            pendingMigration,
                            willRun = !SkipSeedWithNoPendingMigrations || pendingMigration != null
                        }
                    ).ToList();

            const string logMsg = "Configuration for {0} has no pending migrations... skipping the Seed method";
            var skippedMigrators = migrations.Where(m => !m.willRun).Select(m => m.migrator).ToList();
            skippedMigrators.ForEach(m => Logger.Verbose(string.Format(logMsg, m.Configuration.ContextKey)));

            var migrationsToRun = migrations.Where(m => m.willRun).ToList();
            if (FailOnMissingCurrentMigration)
            {
                migrationsToRun
                    .Select(m => new MigratorLoggingDecorator(m.migrator, Logger)).ToList()
                    .ForEach(migrator => migrator.Update());
            }
            else
            {
                var namedMigrations =
                    (from m in migrationsToRun
                        let latestMigration = m.pendingMigration ?? m.migrator.GetDatabaseMigrations().FirstOrDefault()
                        select new {migrator = new MigratorLoggingDecorator(m.migrator, Logger), latestMigration}
                        ).ToList();
                namedMigrations.ForEach(x => x.migrator.Update(x.latestMigration));
            }

            bool wasPendingMigrations = migrations.Any(m => m.pendingMigration != null);
            AdditionalSeed(context, wasPendingMigrations);
            context.SaveChanges();
        }

        private DbMigrator CreateMigrator(DbMigrationsConfiguration c, DbContext context)
        {
            if (!UseSuppliedContext)
            {
                return new DbMigrator(c);
            }

            var mt = typeof (DbMigrator);
            const BindingFlags instanceCtor = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var ctorArgs = new object[] {c, context};
            return
                (DbMigrator) mt.Assembly.CreateInstance(mt.FullName, true, instanceCtor, null, ctorArgs, null, null);
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
        /// <param name="wasPendingMigrations"><c>true</c> whether there were pending migrations that have now run</param>
        public virtual void AdditionalSeed(DbContext context, bool wasPendingMigrations)
        {
            // override in subclass
        }
    }
}