using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using Spike.Migrations.Model;
using Spikes.Migrations.BaseModel;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests
{
    [TestFixture]
    public class InheritanceTests
    {
        private SpikesMigrationsDb _db;
        private DbConnection _conn ;
        private DbTransaction _transaction;

        [SetUp]
        public void Setup()
        {
            _conn = new SpikesMigrationsDb().Database.Connection;
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
        public void CanInsertSubclassedInstances()
        {
            var dataRole = new DataUserRole {DataRoleProp = "Data 1", Name = "DataRole1"};
            var featureRole = new FeatureUserRole {FeatureRoleProp = "Feature 1", Name = "FeatureRole1"};

            _db.UserRoles.AddRange(new UserRole[] {dataRole, featureRole});
            _db.SaveChanges();
        }

        [Test]
        public void CanLoadPolymorphicNavigationProperty()
        {
            // given
            var dataRole = new DataUserRole {DataRoleProp = "Data 1", Name = "DataRole1"};
            _db.UserRoles.Add(dataRole);
            var asset = new Asset {RequiredUserRole = dataRole, Reference = "Asset1", Title = "Asset 1"};
            _db.Assets.Add(asset);
            _db.SaveChanges();

            // when, then
            using (var db = CreateDb())
            {
                var loadedAsset = db.Assets.First();
                Assert.That(loadedAsset.RequiredUserRole, Is.InstanceOf<DataUserRole>());
            }
        }
    }
}