using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using Spikes.EntityFramework.Models.Bidirectional.Conventional;

namespace Spikes.EntityFramework.Tests
{
    [TestFixture]
    public class BidirectionalTests
    {
        private SpikesDbContext _db;
        private DbContextTransaction _transaction;

        [SetUp]
        public void Setup()
        {
            _db = new SpikesDbContext();
            _transaction = _db.Database.BeginTransaction();
        }

        [TearDown]
        public void Teardown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _db.Dispose();
        }

        [Test]
        public void CanInstantiate()
        {
            // see setup
        }

        
        
        [Test]
        public void CanClearLocalCache()
        {
            _db.Orders.Add(new Order());
            Assert.That(_db.Orders.Local.Count(), Is.EqualTo(1), "checking assumptions");

            _db.Orders.Local.Clear();
            Assert.That(_db.Orders.Local.Count(), Is.EqualTo(0));
        }

        [Test]
        public void QueriesWillNotReturnAttachedButNotPersistedEntities()
        {
            _db.Orders.Add(new Order());
            Assert.That(_db.Orders.ToList(), Is.Empty);
        }

        [Test]
        public void CanAddFetchAndDelete()
        {
            _db.Orders.Add(new Order
                {
                    Lines =
                        {
                            new OrderLine { Units = 5 },
                            new OrderLine { Units = 10 },
                        }
                });
            _db.SaveChanges();
            _db.Orders.Local.Clear();

            
            Assert.That(_db.Orders.Count(), Is.EqualTo(1));
        }

        
/*
        [Test]
        public void CanAddAndFetchOrderWithLines()
        {
            _db.Orders.Add(new Order
                {
                    OrderLines = 
                });
            _db.SaveChanges();
            Assert.That(_db.Orders.Count(), Is.EqualTo(1));
        }
*/

        
    }
}