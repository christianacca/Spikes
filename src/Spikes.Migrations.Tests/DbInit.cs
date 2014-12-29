using System.Data.Entity;
using System.Linq;
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
            // as a balance between performance and test isolation/reliability throwing away the database before the start of each *test run*
            Database.SetInitializer<SpikesMigrationsDb>(new SpikesMultiMigrateDbToLastestVersion {DropDatabase = true});

            // trigger initialisation before any tests run
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            new SpikesMigrationsDb().Assets.Any();
        }
    }
}