using System.Data.Common;
using System.Linq;
using NUnit.Framework;
using Spikes.EntityFramework.Models.Bidirectional.Conventional;

namespace Spikes.EntityFramework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Reflection;

    [TestFixture]
    public class BidirectionalTests : DbIntegrationTestBase<SpikesDbContext>
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
        public void CanAddAndFetch()
        {
            using (var db = CreateDbContext())
            {
                db.Orders.Add(new Order
                {
                    Lines =
                        {
                            new OrderLine { Units = 5 },
                            new OrderLine { Units = 10 },
                        }
                });
                db.SaveChanges();
            }

            using (var db = CreateDbContext())
            {
                Assert.That(db.Orders.Count(), Is.EqualTo(1));
                Assert.That(db.Orders.Include(o => o.Lines).Single().Lines.Count, Is.EqualTo(2));
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