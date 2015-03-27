using System;
using System.Collections.Generic;
using System.Reflection;

namespace Spikes.EntityFramework.Tests
{
    using System.Data.Common;
    using System.Linq;

    using NUnit.Framework;

    using Models.Bidirectional.Conventional;

    [TestFixture]
    public class ExploratoryTests : DbIntegrationTestBase<SpikesDbContext>
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

        [Test]
        public void CanFindOneToManyNavigationProperties()
        {
            using (var db = CreateDbContext())
            {
                IEnumerable<PropertyInfo> properties = db.ManyToOne(typeof(OrderLine));
                Assert.That(properties.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void CanFindOneToManyDependentProperties()
        {
            using (var db = CreateDbContext())
            {
                foreach (KeyValuePair<PropertyInfo, IEnumerable<PropertyInfo>> navColumn in db.GetNavigationProperties(typeof(OrderLine)))
                {
                    Console.Out.WriteLine("Navigation Property = {0}", navColumn.Key.Name);
                    foreach (var depProp in navColumn.Value)
                    {
                        Console.Out.WriteLine("... Dependent Property = {0}", depProp.Name);
                    }
                }
            }
        }
    }
}