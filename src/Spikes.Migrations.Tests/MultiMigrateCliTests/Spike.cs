using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Design;
using System.IO;
using System.Linq;
using CcAcca.EntityFramework.Migrations;
using NUnit.Framework;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests.MultiMigrateCliTests
{
    [TestFixture]
    public class Spike
    {
        private static List<IDisposable> _trash = new List<IDisposable>();

        [SetUp]
        public void Setup()
        {
            var db = new SpikesMigrationsDb("SpikesMigrationsDb");
            if (db.Database.Exists())
            {
                db.Database.Delete();
            }
        }

        [TearDown]
        public void Teardown()
        {
            _trash.ForEach(d => d.Dispose());
        }

        [Test]
        public void CanInstantiateEfTooling()
        {
            ToolingFacade facade = CreateToolingFacadeForBaseData();
            Assert.That(facade, Is.Not.Null);
        }

        [Test]
        public void CanListPendingMigrations()
        {
            var facade = CreateToolingFacadeForBaseData();
            List<string> migrations = facade.GetPendingMigrations().ToList();
            Assert.That(migrations, Is.Not.Empty);
            migrations.ForEach(Console.WriteLine);
        }

        [Test]
        public void CanMigrateDb()
        {
            var facade = CreateToolingFacadeForBaseData();
            facade.Update(null, true);
        }

        [Test]
        public void CanMigrateDbPiecemeal()
        {
            var facade = CreateToolingFacadeForBaseData();
            List<string> migrations = facade.GetPendingMigrations().ToList();
            migrations.ForEach(m => facade.Update(m, true));
        }

        
        [Test]
        public void CanMigrateDbFromMultipleConfigurations()
        {
            var bdf = CreateToolingFacadeForBaseData();
            var bdm = new DelegatedMigrator(bdf.GetPendingMigrations, migration => bdf.Update(migration, true));
            var mdf = CreateToolingFacadeForMainData();
            var mdm = new DelegatedMigrator(mdf.GetPendingMigrations, migration => mdf.Update(migration, true))
            {
                IsAutoMigrationsEnabled = true
            };
            var migrationRunner = new MultiMigrateDbToLastestVsRunner(new[] { bdm, mdm })
            {
                SkippedMigrations = new[] { "201501032326177_Rename LookupItem pk" }
            };
            migrationRunner.Run();
        }


        private static ToolingFacade CreateToolingFacadeForBaseData()
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var facade = new ToolingFacade(
                migrationsAssemblyName: "Spikes.Migrations.BaseDataMigrations",
                contextAssemblyName: null,
                configurationTypeName: "Spikes.Migrations.BaseDataMigrations.Migrations.Configuration",
                workingDirectory: workingDirectory,
                configurationFilePath:
                    Path.Combine(workingDirectory, "Spikes.Migrations.Tests.dll.config"),
                dataDirectory: null,
                connectionStringInfo: new DbConnectionInfo("SpikesMigrationsDb")
                );
            _trash.Add(facade);
            return facade;
        }
        private static ToolingFacade CreateToolingFacadeForMainData()
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ToolingFacade facade = new ToolingFacade(
                migrationsAssemblyName: "Spikes.Migrations.DataMigrations",
                contextAssemblyName: null,
                configurationTypeName: "Configuration",
                workingDirectory: workingDirectory,
                configurationFilePath:
                    Path.Combine(workingDirectory, "Spikes.Migrations.Tests.dll.config"),
                dataDirectory: null,
                connectionStringInfo: new DbConnectionInfo("SpikesMigrationsDb")
                );
            _trash.Add(facade);
            return facade;
        }
    }
}