using System.Data.Entity.Migrations;
using Spikes.Migrations.BaseData.Migrations;

namespace Spikes.Migrations.Tests
{
    public class SpikesMultiMigrateDbToLastestVersion : MultiMigrateDbToLastestVersion
    {
        public SpikesMultiMigrateDbToLastestVersion(bool dropDatabase = false)
            : base(new DbMigrationsConfiguration[]
            {
                new Configuration(),
                new Data.Migrations.Configuration()
            })
        {
            DropDatabase = dropDatabase;
        }
    }
}