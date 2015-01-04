using System.Data.Entity;
using NUnit.Framework;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests.MultiMigrateTests
{
    [SetUpFixture]
    public class DbInit
    {
        [SetUp]
        public void Setup()
        {
            Database.SetInitializer<SpikesMigrationsDb>(new SpikesMultiMigrateDbToLastestVersion
            {
                ConnectionStringName = "SpikesMigrationsDb"
            });

            var db = new SpikesMigrationsDb("SpikesMigrationsDb");
/*
            if (db.Database.Exists())
            {
                db.Database.Delete();
            }
*/
            db.Database.Initialize(false);
        }
    }
}