using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using Spike.Migrations.Model;
using Spikes.Migrations.BaseModel;
using Spikes.Migrations.Data;

namespace Spikes.Migrations.Tests.MultiMigrateTests
{
    [TestFixture]
    public class PostDbCreateTests
    {
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
                var initializer = new SpikesMultiMigrateDbToLastestVersion();
                initializer.InitializeDatabase(db);
            }
        }

        [Test]
        public void CanFetchFakes()
        {
            using (var db = new SpikesMigrationsDb("SpikesMigrationsDb"))
            {
                List<FakeEntity> fakes = db.FakeEntities.ToList();
                Assert.That(fakes, Is.Not.Empty);
            }
        }

        [Test]
        public void CanInsertSubclassedInstances()
        {
            // when
            using (var db = new SpikesMigrationsDb("SpikesMigrationsDb"))
            {
                var dataRole = new DataUserRole { DataRoleProp = "Data 1", Name = "DataRole1", Key = 1 };
                var featureRole = new FeatureUserRole { FeatureRoleProp = "Feature 1", Name = "FeatureRole1" };
                db.UserRoles.AddRange(new UserRole[] { dataRole, featureRole });
                db.SaveChanges();
            }

            // then
            using (var db = new SpikesMigrationsDb("SpikesMigrationsDb"))
            {
                Assert.That(db.UserRoles.ToList().Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void CanLoadPolymorphicNavigationProperty()
        {
            // given
            using (var db = new SpikesMigrationsDb("SpikesMigrationsDb"))
            {
                var dataRole = new DataUserRole { DataRoleProp = "Data 1", Name = "DataRole1", Key = 2 };
                db.UserRoles.Add(dataRole);
                var asset = new Asset { RequiredUserRole = dataRole, Reference = "Asset1", Title = "Asset 1" };
                db.Assets.Add(asset);
                db.SaveChanges();
            }

            // when, then
            using (var db = new SpikesMigrationsDb("SpikesMigrationsDb"))
            {
                var loadedAsset = db.Assets.First();
                Assert.That(loadedAsset.RequiredUserRole, Is.InstanceOf<DataUserRole>());
            }
        }
    }
}