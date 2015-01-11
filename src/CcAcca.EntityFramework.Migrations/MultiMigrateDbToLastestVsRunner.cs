using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    /// Runs the migrations defined by the supplied <see cref="DelegatedMigrator"/>s
    /// in order of migration creation date and then by <see cref="DelegatedMigrator.Priority"/>
    /// </summary>
    public class MultiMigrateDbToLastestVsRunner : IDisposable
    {
        private readonly IEnumerable<DelegatedMigrator> _migrators;

        public MultiMigrateDbToLastestVsRunner(IEnumerable<DelegatedMigrator> migrators)
        {
            _migrators = migrators;
            Logger = NullMigrationsLogger.Instance;
        }

        public IEnumerable<string> SkippedMigrations { get; set; }
        public bool SkipSeedWithNoPendingMigrations { get; set; }
        public MigrationsLogger Logger { get; set; }


        private IEnumerable<MigrationInfo> GetMigrations(DelegatedMigrator migrator)
        {
            var parser = MigrationInfo.CreateParser(SkippedMigrations);
            List<MigrationInfo> migrations = migrator.GetPendingMigrations().Select(s => parser.Parse(s)).ToList();
            if (migrator.IsAutoMigrationsEnabled)
            {
                return migrations.Union(new[] {MigrationInfo.Auto});
            }
            else
            {
                return migrations;
            }
        }

        public bool Run()
        {
            var migrations = (
                from migrator in _migrators
                from migration in GetMigrations(migrator).DefaultIfEmpty(MigrationInfo.Null)
                select new
                {
                    migrator,
                    migration,
                    willRun = (!SkipSeedWithNoPendingMigrations || !migration.IsNull) && !migration.IsSkipped
                }).ToList();

            const string logMsg = "'{0}' has no pending migrations... skipping the Seed method";
            List<string> skippedConfigs =
                migrations.Where(m => !m.willRun).Select(m => m.migrator.ConfigurationTypeName ?? "AnonymousConfig").ToList();
            skippedConfigs.ForEach(c => Logger.Verbose(string.Format(logMsg, c)));

            var orderedMigrations = migrations
                .Where(m => m.willRun)
                .OrderBy(m => m.migration.CreatedOn).ThenBy(m => m.migrator.Priority)
                .ToList();

            var batchedMigrations = orderedMigrations
                .SliceWhen((prev, current) => prev != null && prev.migrator != current.migrator)
                .ToList();

            batchedMigrations.ForEach(batch =>
            {
                batch = batch.ToList();
                DelegatedMigrator migator = batch.First().migrator;
                MigrationInfo lastMigration = batch.Last().migration;
                migator.Update(lastMigration.FullName);
            });

            bool migrationsRun = migrations.Any(m => m.willRun);
            return migrationsRun;
        }

        public void Dispose()
        {
            foreach (DelegatedMigrator migrator in _migrators)
            {
                migrator.Dispose();
            }
        }
    }
}