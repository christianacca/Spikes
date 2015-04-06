using System.Data.Entity;
using NUnit.Framework;

namespace Spikes.EntityFramework.Tests
{
    [SetUpFixture]
    public class DbSetup
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
            Database.SetInitializer(new DropCreateDatabaseAlways<SpikesDbContext>());
            Database.SetInitializer(new DropCreateDatabaseAlways<SpikesExternalDbContext>());
            using (var db = new SpikesExternalDbContext())
            {
                db.Database.Initialize(false);
            }
            using (var db = new SpikesDbContext())
            {
                db.Database.Initialize(false);
            }
        }
    }
}