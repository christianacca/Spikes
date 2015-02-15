using System.Collections.Generic;
using System.Data.Entity.Migrations;
using CcAcca.EntityFramework.Migrations;
using BaseConfiguration = Spikes.Migrations.BaseDataMigrations.Migrations.Configuration;
using MainNonAutoConfiguration = Spikes.Migrations.DataMigrations.Migrations.Configuration;
using MainAutoConfiguration = Spikes.Migrations.DataMigrations.AutoMigrations.AutoConfiguration;

namespace Spikes.Migrations.Tests
{
    public class SpikesMultiMigrateDbToLastestVersion : MultiMigrateDbToLastestVersion
    {
        public SpikesMultiMigrateDbToLastestVersion(IEnumerable<DbMigrationsConfiguration> configurations = null, string connectionStringName = null)
            : base(configurations ?? new DbMigrationsConfiguration[]
            {
                new BaseConfiguration(),
                new MainNonAutoConfiguration()
            }, connectionStringName ?? "SpikesMigrationsDb")
        {
            SkippedMigrations = new[] { "201501032326177_Rename LookupItem pk" };
        }

        public static SpikesMultiMigrateDbToLastestVersion UsingCodeBasedMigrations(string connectionStringName = null)
        {
            var configs = new DbMigrationsConfiguration[] {new BaseConfiguration(), new MainNonAutoConfiguration()};
            return new SpikesMultiMigrateDbToLastestVersion(configs, connectionStringName);
        }

        public static SpikesMultiMigrateDbToLastestVersion UsingAutoMigrations(string connectionStringName = null)
        {
            var configs = new DbMigrationsConfiguration[] {new BaseConfiguration(), new MainAutoConfiguration()};
            return new SpikesMultiMigrateDbToLastestVersion(configs, connectionStringName);
        }
    }
}