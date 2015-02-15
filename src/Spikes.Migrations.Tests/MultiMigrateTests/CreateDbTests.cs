using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using NUnit.Framework;
using Spikes.Migrations.BaseDataMigrations.Migrations;
using Spikes.Migrations.Data;
using Spikes.Migrations.DataMigrations.AutoMigrations;

namespace Spikes.Migrations.Tests.MultiMigrateTests
{
    [TestFixture]
    public class CreateDbTests
    {
        private readonly List<IDisposable> _trash = new List<IDisposable>();

        [SetUp]
        public void Setup()
        {
            Database.SetInitializer(new NullDatabaseInitializer<SpikesMigrationsDb>());

            using (var db = new SpikesMigrationsDb())
            {
                if (db.Database.Exists())
                {
                    db.Database.Delete();
                }
            }
        }

        [TearDown]
        public void Teardown()
        {
            _trash.ForEach(d => d.Dispose());
        }

        private SpikesMigrationsDb CreateDbContext(string connectionStringName = null)
        {
            var context = new SpikesMigrationsDb(connectionStringName);
            _trash.Add(context);
            return context;
        }

        [Test]
        public void CanUpgradeToLatestVs_CodeBasedMigrations()
        {
            SpikesMigrationsDb db = CreateDbContext();
            var initializer = SpikesMultiMigrateDbToLastestVersion.UsingCodeBasedMigrations();
            initializer.InitializeDatabase(db);
        }

        [Test]
        public void CanUpgradeToLatestVs_AutodMigrations()
        {
            SpikesMigrationsDb db = CreateDbContext();
            var initializer = SpikesMultiMigrateDbToLastestVersion.UsingAutoMigrations();
            initializer.InitializeDatabase(db);
        }

        [Test]
        public void CanGetDatabaseMigrations()
        {
            // given
            SpikesMigrationsDb db = CreateDbContext();
            var initializer = SpikesMultiMigrateDbToLastestVersion.UsingAutoMigrations();
            initializer.InitializeDatabase(db);

            // when, then
            var migrator = new DbMigrator(new AutoConfiguration());
            IEnumerable<string> migrations = migrator.GetDatabaseMigrations();
            Assert.That(migrations, Is.Not.Empty);
        }

        [Test]
        public void CanScriptMigration()
        {
            var migrator = new MigratorScriptingDecorator(new DbMigrator(new AutoConfiguration()));
            string sql = migrator.ScriptUpdate("201501032325042_Merge BaseModel3", "201501110901388_Add CustomUserRole");
            Console.Out.WriteLine(sql);
        }

        [Test]
        public void CanScriptMigrationFromBaseData()
        {
            // note: notice that there is a bug in the ef that causes schema name for insert into MigrationsHistory table to be incorrect
            // (bug reported here: https://entityframework.codeplex.com/workitem/1871)
            var migrator = new MigratorScriptingDecorator(new DbMigrator(new Configuration()));
            string sql = migrator.ScriptUpdate("201501032315036_Introduce by-directional association", "201501032326177_Rename LookupItem pk");
            Console.Out.WriteLine(sql);
        }
    }
}