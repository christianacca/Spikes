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
        private Dictionary<DelegatedMigrator, IEnumerable<MigrationInfo>> _allMigrations;

        public MultiMigrateDbToLastestVsRunner(IEnumerable<DelegatedMigrator> migrators)
        {
            _migrators = migrators;
            _allMigrations = new Dictionary<DelegatedMigrator, IEnumerable<MigrationInfo>>();
            Logger = NullMigrationsLogger.Instance;
        }

        public IEnumerable<string> SkippedMigrations { get; set; }
        /// <summary>
        /// TODO: currently not in use - either remove or support
        /// </summary>
        public bool SkipSeedWithNoPendingMigrations { get; set; }
        public MigrationsLogger Logger { get; set; }


        private IEnumerable<MigrationInfo> GetPendingMigrations(DelegatedMigrator migrator)
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

        private IEnumerable<MigrationInfo> GetAllMigrations(DelegatedMigrator migrator)
        {
            var parser = MigrationInfo.CreateParser(null);
            List<MigrationInfo> applied = migrator.GetDatabaseMigrations().Select(s => parser.Parse(s)).ToList();
            var all = applied.Union(GetPendingMigrations(migrator))
                .OrderBy(m => m.CreatedOn)
                .ThenBy(m => migrator.Priority)
                .ToList();
            return all;
        }

        public bool Run()
        {
            // note: currently the seed method for each DbMigrationsConfiguration is being run multiple times
            // todo: work out some way to tell DbMigrationsConfiguration.Seed that it should only run once

            var migrations = (
                from migrator in _migrators
                from migration in GetPendingMigrations(migrator)
                select new { migrator, migration }
                ).ToList();

            var orderedMigrations = migrations
                .OrderBy(m => m.migration.CreatedOn).ThenBy(m => m.migrator.Priority)
                .ToList();

            var batchedMigrations = orderedMigrations
                .SliceWhen((prev, current) => prev != null && prev.migrator != current.migrator || current.migration.IsSkipped)
                .ToList();

            if (migrations.Any(m => m.migration.IsSkipped))
            {
                _allMigrations = _migrators.ToDictionary(m => m, GetAllMigrations);
            }

            batchedMigrations.ForEach(batch =>
            {
                batch = batch.ToList();
                DelegatedMigrator migator = batch.First().migrator;
                MigrationInfo lastMigration = batch.Last().migration;
                if (lastMigration.IsSkipped)
                {
                    var previousMigration =
                        _allMigrations[migator].TakeWhile(m => m.CreatedOn < lastMigration.CreatedOn).LastOrDefault();
                    migator.InsertMigrationHistory(lastMigration.FullName, previousMigration != null ? previousMigration.FullName : null);
                }
                else
                {
                    migator.Update(lastMigration.FullName);
                }
            });

            bool migrationsRun = migrations.Any(m => !m.migration.IsSkipped);
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