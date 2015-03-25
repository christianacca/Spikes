namespace Spikes.EntityFramework.Tests
{
    using System.Data.Common;
    using System.Linq;

    using NUnit.Framework;

    using Spikes.EntityFramework.Models.Bidirectional.Conventional;

    [TestFixture]
    public class GeneralTests : DbIntegrationTestBase<SpikesDbContext>
    {
        protected override DbConnection CreateConnection()
        {
            return new SpikesDbContext().Database.Connection;
        }

        protected override SpikesDbContext CreateDbContextFromConnection(DbConnection cnn)
        {
            return new SpikesDbContext(cnn, contextOwnsConnection: false);
        }

        [Test]
        public void CanClearLocalCache()
        {
            using (var db = CreateDbContext())
            {
                db.Orders.Add(new Order());
                Assert.That(db.Orders.Local.Count(), Is.EqualTo(1), "checking assumptions");

                db.Orders.Local.Clear();
                Assert.That(db.Orders.Local.Count(), Is.EqualTo(0));
            }
        }

        [Test]
        public void QueriesWillNotReturnAttachedButNotYetPersistedEntities()
        {
            using (var db = CreateDbContext())
            {
                db.Orders.Add(new Order());
                Assert.That(db.Orders.ToList(), Is.Empty);
            }
        }
    }
}