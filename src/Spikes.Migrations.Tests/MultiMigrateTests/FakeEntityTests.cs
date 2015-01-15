using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NUnit.Framework;
using Spike.Migrations.Model;
using Spikes.Migrations.BaseModel;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests.MultiMigrateTests
{
    [TestFixture]
    public class FakeEntityTests
    {
        private SpikesMigrationsDb _db;
        private DbConnection _conn ;
        private DbTransaction _transaction;

        [SetUp]
        public void Setup()
        {
            _conn = new SpikesMigrationsDb("SpikesMigrationsDb").Database.Connection;
            _conn.Open();
            _transaction = _conn.BeginTransaction();
            _db = CreateDb();
        }

        private SpikesMigrationsDb CreateDb()
        {
            var db = new SpikesMigrationsDb(_conn, contextOwnsConnection: false);
            db.Database.UseTransaction(_transaction);
            return db;
        }

        [TearDown]
        public void Teardown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _conn.Dispose();
            _db.Dispose();
        }

        [Test]
        public void CanFetchFakes()
        {
            List<FakeEntity> fakes = _db.FakeEntities.ToList();
            Assert.That(fakes, Is.Not.Empty);
        }
    }
}