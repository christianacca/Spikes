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

        public MigrationsLogger Logger { get; set; }


        private IEnumerable<MigrationInfo> GetPendingMigrations(DelegatedMigrator migrator)
        {
            var parser = MigrationInfo.CreateParser(SkippedMigrations);
            List<MigrationInfo> migrations = migrator.GetPendingMigrations().Select(parser.Parse).ToList();
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
            List<MigrationInfo> applied = migrator.GetDatabaseMigrations().Select(parser.Parse).ToList();
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
                    // Q: Why do we need to insert a skipped migration into the migration history table?
                    // A: the standard behaviour when calling migator.Update(lastMigration.FullName) is to execute
                    //    ALL migrations after the last inserted migration in the db and the migration identified
                    //    by lastMigration.FullName
                    //    This will result in running migration that was skipped in the previous batch
                    var previousMigration =
                        _allMigrations[migator].TakeWhile(m => m.CreatedOn < lastMigration.CreatedOn).LastOrDefault();
                    migator.InsertMigrationHistory(previousMigration != null ? previousMigration.FullName : null, lastMigration.FullName);
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