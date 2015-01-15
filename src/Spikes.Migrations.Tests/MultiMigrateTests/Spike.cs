using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using CcAcca.EntityFramework.Migrations;
using NUnit.Framework;
using Spikes.Migrations.Data;
using Spikes.Migrations.DataMigrations.Migrations;

namespace Spikes.Migrations.Tests.MultiMigrateTests
{
    [TestFixture]
    public class Spike
    {
        private readonly List<IDisposable> _trash = new List<IDisposable>();

        [SetUp]
        public void Setup()
        {
            Database.SetInitializer(new NullDatabaseInitializer<SpikesMigrationsDb>());

            using (var db = new SpikesMigrationsDb("SpikesMigrationsDb"))
            {
                if (db.Database.Exists())
                {
                    db.Database.Delete();
                }
                db.Database.Initialize(false);
            }
        }

        [TearDown]
        public void Teardown()
        {
            _trash.ForEach(d => d.Dispose());
        }

        private SpikesMigrationsDb CreateDbContext()
        {
            var context = new SpikesMigrationsDb("SpikesMigrationsDb");
            _trash.Add(context);
            return context;
        }

        [Test]
        public void CanUpgradeToLatestVs()
        {
            SpikesMigrationsDb db = CreateDbContext();
            var initializer = new SpikesMultiMigrateDbToLastestVersion();
            initializer.InitializeDatabase(db);
        }

        [Test]
        public void CanGetDatabaseMigrations()
        {
            // given
            SpikesMigrationsDb db = CreateDbContext();
            var initializer = new SpikesMultiMigrateDbToLastestVersion();
            initializer.InitializeDatabase(db);

            // when, then
            var migrator = new DbMigrator(new Configuration());
            IEnumerable<string> migrations = migrator.GetDatabaseMigrations();
            Assert.That(migrations, Is.Not.Empty);
        }

        [Test]
        public void CanScriptMigration()
        {
            var migrator = new MigratorScriptingDecorator(new DbMigrator(new Configuration()));
            string sql = migrator.ScriptUpdate("201501032325042_Merge BaseModel3", "201501110901388_Add CustomUserRole");
            Console.Out.WriteLine(sql);
        }
    }
}