using System.Data.Entity.Migrations;
using Spikes.Migrations.BaseData;
using Spikes.Migrations.BaseDataMigrations.Migrations;

namespace Spikes.Migrations.Tests
{
    public class SpikesMultiMigrateDbToLastestVersion : MultiMigrateDbToLastestVersion
    {
        public SpikesMultiMigrateDbToLastestVersion()
            : base(new DbMigrationsConfiguration[]
            {
                new Configuration(),
                new DataMigrations.Migrations.Configuration()
            })
        {
        }
    }
}