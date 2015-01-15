using System.Data.Entity.Migrations;
using CcAcca.EntityFramework.Migrations;
using Spikes.Migrations.BaseDataMigrations.Migrations;

namespace Spikes.Migrations.Tests
{
    public class SpikesMultiMigrateDbToLastestVersion : MultiMigrateDbToLastestVersion
    {

        public SpikesMultiMigrateDbToLastestVersion() : this("SpikesMigrationsDb") {}

        public SpikesMultiMigrateDbToLastestVersion(string connectionStringName)
            : base(new DbMigrationsConfiguration[]
            {
                new Configuration(),
                new DataMigrations.Migrations.Configuration()
            }, connectionStringName)
        {
            SkippedMigrations = new[] {"201501032326177_Rename LookupItem pk"};
        }
    }
}