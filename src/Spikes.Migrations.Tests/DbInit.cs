using System.Data.Entity;
using NUnit.Framework;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests
{
    [SetUpFixture]
    public class DbInit
    {
        [SetUp]
        public void Setup()
        {
            // note 1: 
            // as a balance between performance and test isolation/reliability throwing away the database 
            // before the start of each *test run*
            // note 2:
            // setting UseSuppliedContext to true will result in dbBuilderCtx in being used to build
            // the database - this maybe problematic and is causing us to HAVE to set 
            // FailOnMissingCurrentMigration to true
            Database.SetInitializer<SpikesMigrationsDb>(new SpikesMultiMigrateDbToLastestVersion
            {
                DropDatabase = true,
                UseSuppliedContext = true,
                FailOnMissingCurrentMigration = false
            });

            var dbBuilderCtx = new SpikesMigrationsDb();
            dbBuilderCtx.Database.Initialize(false);
        }
    }
}