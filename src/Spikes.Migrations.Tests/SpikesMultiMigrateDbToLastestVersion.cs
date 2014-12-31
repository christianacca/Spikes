using System.Data.Entity.Migrations;
using Spikes.Migrations.BaseDataMigrations.Migrations;

namespace Spikes.Migrations.Tests
{
    public class SpikesMultiMigrateDbToLastestVersion : MultiMigrateDbToLastestVersion
    {
        public SpikesMultiMigrateDbToLastestVersion(bool dropDatabase = false)
            : base(new DbMigrationsConfiguration[]
            {
                new Configuration(),
                new DataMigrations.Migrations.Configuration()
            })
        {
            DropDatabase = dropDatabase;
        }
    }
}