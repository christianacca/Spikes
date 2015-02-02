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

        public SpikesMultiMigrateDbToLastestVersion()
            : this(new DbMigrationsConfiguration[]
            {
                new BaseConfiguration(),
                new MainNonAutoConfiguration()
            })
        {
        }

        public SpikesMultiMigrateDbToLastestVersion(IEnumerable<DbMigrationsConfiguration> configurations)
            : base(configurations, "SpikesMigrationsDb")
        {
            SkippedMigrations = new[] { "201501032326177_Rename LookupItem pk" };
        }

        public static SpikesMultiMigrateDbToLastestVersion UsingCodeBasedMigrations
        {
            get
            {
                var configs = new DbMigrationsConfiguration[] { new BaseConfiguration(), new MainNonAutoConfiguration() };
                return new SpikesMultiMigrateDbToLastestVersion(configs);
            }
        }
        public static SpikesMultiMigrateDbToLastestVersion UsingAutoMigrations
        {
            get
            {
                var configs = new DbMigrationsConfiguration[] { new BaseConfiguration(), new MainAutoConfiguration() };
                return new SpikesMultiMigrateDbToLastestVersion(configs);
            }
        }
    }
}