using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Globalization;
using System.Linq;

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
        ///     The order of <paramref name="configurations" /> should be least dependent model
        ///     first
        /// </remarks>
        /// <param name="configurations">Defines the migrations to run</param>
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
            List<MigratorInfo> migrators = _configurations.Select(CreateMigratorInfo).ToList();

            var migrations = (
                from migrator in migrators
                from migration in GetMigrations(migrator.Impl).DefaultIfEmpty(MigrationInfo.Null)
                select new
                {
                    migrator,
                    migration,
                    willRun = !SkipSeedWithNoPendingMigrations || !migration.IsNull
                }).ToList();

            const string logMsg = "Configuration for {0} has no pending migrations... skipping the Seed method";
            List<DbMigrator> skippedMigrators = migrations.Where(m => !m.willRun).Select(m => m.migrator.Impl).ToList();
            skippedMigrators.ForEach(m => Logger.Verbose(string.Format(logMsg, m.Configuration.ContextKey)));

            var orderedMigrations = migrations
                .Where(m => m.willRun)
                .OrderBy(m => m.migration.CreatedOn).ThenBy(m => m.migrator.Priority)
                .ToList();
            var batchedMigrations = orderedMigrations
                .SliceWhen(
                    (prev, current) =>
                        prev == null ||
                        prev.migrator.Impl.Configuration.ContextKey != current.migrator.Impl.Configuration.ContextKey)
                .ToList();

            batchedMigrations.ForEach(batch =>
            {
                batch = batch.ToList();
                DbMigrator migator = batch.First().migrator.Impl;
                MigrationInfo lastMigration = batch.Last().migration;
                migator.Update(lastMigration.FullName);
            });

            AdditionalSeed(context, migrations.Any(m => m.willRun));
            context.SaveChanges();
        }

        private IEnumerable<MigrationInfo> GetMigrations(DbMigrator migrator)
        {
            List<MigrationInfo> migrations = migrator.GetPendingMigrations().Select(MigrationInfo.Parse).ToList();
            if (migrator.Configuration.AutomaticMigrationsEnabled)
            {
                return migrations.Union(new[] {MigrationInfo.Auto});
            }
            else
            {
                return migrations;
            }
        }

        private MigratorInfo CreateMigratorInfo(DbMigrationsConfiguration c, int migratorPriority)
        {
            if (!String.IsNullOrEmpty(ConnectionStringName))
            {
                c.TargetDatabase = new DbConnectionInfo(ConnectionStringName);
            }
            return new MigratorInfo {Impl = new DbMigrator(c), Priority = migratorPriority};
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

        private class MigrationInfo
        {
            public DateTime CreatedOn { get; set; }
            public string FullName { get; set; }
            public bool IsAuto { get; set; }
            public bool IsNull { get; set; }
            public string Name { get; set; }

            public static MigrationInfo Auto
            {
                get { return new MigrationInfo {IsAuto = true, CreatedOn = DateTime.MaxValue}; }
            }

            public static MigrationInfo Null
            {
                get { return new MigrationInfo {IsNull = true, CreatedOn = DateTime.MaxValue}; }
            }

            public override string ToString()
            {
                return string.Format("Name: {0}, CreatedOn: {1}, IsAuto: {2}, IsNull: {3}", Name, CreatedOn, IsAuto, IsNull);
            }

            public static MigrationInfo Parse(string rawName)
            {
                string[] nameParts = rawName.Split(new[] {"_"}, StringSplitOptions.None);
                return new MigrationInfo
                {
                    Name = nameParts[1],
                    FullName = rawName,
                    CreatedOn = DateTime.ParseExact(nameParts[0], "yyyyMMddHHmmssFFF", CultureInfo.InvariantCulture)
                };
            }
        }

        private class MigratorInfo
        {
            public DbMigrator Impl { get; set; }
            public int Priority { get; set; }

            public override string ToString()
            {
                return string.Format("ContextKey: {0}, Priority: {1}", Impl.Configuration.ContextKey, Priority);
            }
        }
    }
}