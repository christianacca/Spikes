using System.Data.Entity;
using NUnit.Framework;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests.TestTemplates
{
    [SetUpFixture]
    public class ExampleDbSetup
    {
        /// <summary>
        /// Drop and create the database just before the tests in this test suite execute
        /// </summary>
        /// <remarks>
        /// <para>
        /// Test suite == unit tests that share the same namespace as this class
        /// </para>
        /// <para>
        /// Note: this setup will run once per test suite execution run ie NOT once per test
        /// </para>
        /// </remarks>
        [SetUp]
        public void Setup()
        {
            Database.SetInitializer<SpikesMigrationsDb>(new SpikesMultiMigrateDbToLastestVersion());

            using (var db = new SpikesMigrationsDb())
            {
                if (db.Database.Exists())
                {
                    db.Database.Delete();
                }
                db.Database.Initialize(false);
            }
        }
    }
}