using System.Data.Entity.Migrations;
using CcAcca.EntityFramework.Migrations;
using BaseConfiguration = Spikes.Migrations.BaseDataMigrations.Migrations.Configuration;
using MainConfiguration = Spikes.Migrations.DataMigrations.AutoMigrations.AutoConfiguration;

namespace Spikes.Migrations.Tests
{
    public class SpikesMultiMigrateDbToLastestVersion : MultiMigrateDbToLastestVersion
    {

        public SpikesMultiMigrateDbToLastestVersion() : this("SpikesMigrationsDb") {}

        public SpikesMultiMigrateDbToLastestVersion(string connectionStringName)
            : base(new DbMigrationsConfiguration[]
            {
                new BaseConfiguration(),
                new MainConfiguration(), 
            }, connectionStringName)
        {
            SkippedMigrations = new[] {"201501032326177_Rename LookupItem pk"};
        }
    }
}