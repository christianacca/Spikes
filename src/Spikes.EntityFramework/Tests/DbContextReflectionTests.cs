using System;
using System.Collections.Generic;
using System.Reflection;
using Spikes.EntityFramework.Models.Bidirectional.NonStandard;
using Spikes.EntityFramework.Models.Unidirectional.Conventional;

namespace Spikes.EntityFramework.Tests
{
    using System.Data.Common;
    using System.Linq;

    using NUnit.Framework;

    using Models.Bidirectional.Conventional;

    [TestFixture]
    public class DbContextReflectionTests : DbIntegrationTestBase<SpikesDbContext>
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
        public void CanFindOneToManyNavigationProperties()
        {
            using (var db = CreateDbContext())
            {
                IEnumerable<PropertyInfo> properties = db.ManyToOne(typeof(OrderLine));
                Assert.That(properties.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void CanFindTableColumnProperties()
        {
            using (var db = CreateDbContext())
            {
                IEnumerable<PropertyInfo> properties = db.GetTableColumns(typeof(CustomerFileHeader));
                Assert.That(properties.Count(), Is.EqualTo(8));
            }
        }

        [Test]
        public void CanFindOneToManyDependentProperties()
        {
            using (var db = CreateDbContext())
            {
                Type orderLineType = typeof (OrderLine);
                Dictionary<PropertyInfo, IEnumerable<PropertyInfo>> cols = db.GetForeignKeyColumns(orderLineType);

                Assert.That(cols.Keys, Is.EquivalentTo(new[] { orderLineType.GetProperty("MyOrder") }));
                Assert.That(cols.First().Value, Is.EquivalentTo(new[] { orderLineType.GetProperty("MyOrderId") }));
            }
        }


        [Test]
        public void CanGetClrEntityTypes()
        {
            using (var db = CreateDbContext())
            {
                var mappedTypes = db.GetClrEntityTypes().ToList();
                var expectedTypes = new[]
                {
                    typeof(Customer),
                    typeof(CustomerFileHeader),
                    typeof(FileContentInfo),
                    typeof(FileHeader),
                    typeof(LookupItem),
                    typeof(Order),
                    typeof(OrderFileHeader),
                    typeof(OrderLine),
                    typeof(OrderLineNonStd),
                    typeof(OrderNonStd),
                    typeof(Invoice),
                    typeof(InvoiceLine)
                };
                var extraTypes = mappedTypes.Except(expectedTypes);
                var missingTypes = expectedTypes.Except(mappedTypes);
                Assert.That(extraTypes, Is.Empty);
                Assert.That(missingTypes, Is.Empty);
            }
        }

        [Test]
        public void CanGetMappedTableNames()
        {
            using (var db = CreateDbContext())
            {
                var tableMappings = db.GetTableMappings();
                var expected = new Dictionary<Type, string>
                {
                    { typeof(Customer), "dbo.Customers" },
                    { typeof(CustomerFileHeader), "dbo.FileHeaders" },
                    { typeof(FileContentInfo), "dbo.FileContentInfoes" },
                    { typeof(FileHeader), "dbo.FileHeaders" },
                    { typeof(LookupItem), "dbo.LookupItems" },
                    { typeof(Order), "dbo.Orders" },
                    { typeof(OrderFileHeader), "dbo.FileHeaders" },
                    { typeof(OrderLine), "dbo.OrderLines" },
                    { typeof(OrderLineNonStd), "dbo.OrderLineNonStds" },
                    { typeof(OrderNonStd), "dbo.OrderNonStds" },
                    { typeof(Invoice), "dbo.Invoices" },
                    { typeof(InvoiceLine), "dbo.InvoiceLines"}
                };
                Assert.That(tableMappings.Keys, Is.EquivalentTo(expected.Keys));
                Assert.That(tableMappings.Values, Is.EquivalentTo(expected.Values));
                Assert.That(tableMappings, Is.EquivalentTo(expected));
            }
        }
        
        [Test]
        public void CanGetMappedTableName()
        {
            using (var db = CreateDbContext())
            {
                Assert.That(db.GetTableName(typeof(CustomerFileHeader)), Is.EqualTo("dbo.FileHeaders"));
                Assert.That(db.GetTableName(typeof(FileHeader)), Is.EqualTo("dbo.FileHeaders"));
            }
        }
    }
}