using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using Spikes.EntityFramework.Models.Bidirectional.Conventional;
using Spikes.EntityFramework.Models.Bidirectional.ExternalDb;

namespace Spikes.EntityFramework.Tests
{
    [TestFixture]
    public class BidirectionalTests : MultiDbIntegrationTestBase
    {
        protected override DbConnection CreateConnection<T>()
        {
            Type dbType = typeof (T);
            if (dbType == typeof (SpikesDbContext))
            {
                return new SpikesDbContext().Database.Connection;
            }
            if (dbType == typeof (SpikesExternalDbContext))
            {
                return new SpikesExternalDbContext().Database.Connection;
            }
            throw new InvalidOperationException("DbContext not recognised");
        }

        protected override T CreateDbContextFromConnection<T>(DbConnection cnn)
        {
            Type dbType = typeof (T);
            if (dbType == typeof (SpikesDbContext))
            {
                return new SpikesDbContext(cnn, contextOwnsConnection: false) as T;
            }
            if (dbType == typeof (SpikesExternalDbContext))
            {
                return new SpikesExternalDbContext(cnn, contextOwnsConnection: false) as T;
            }
            throw new InvalidOperationException("DbContext not recognised");
        }

        [Test]
        public void CanAddAndFetch()
        {
            using (var db = CreateDbContext<SpikesDbContext>())
            {
                db.Orders.Add(new Order
                {
                    Lines =
                    {
                        new OrderLine {Units = 5},
                        new OrderLine {Units = 10},
                    }
                });
                db.SaveChanges();
            }

            using (var db = CreateDbContext<SpikesDbContext>())
            {
                Assert.That(db.Orders.Count(), Is.EqualTo(1));
                Assert.That(db.Orders.Include(o => o.Lines).Single().Lines.Count, Is.EqualTo(2));
            }
        }


        [Test]
        public void CanAddCustomerWithFileHeader()
        {
            using (var db = CreateDbContext<SpikesDbContext>())
            {
                var c = new Customer
                {
                    Name = "First Customer",
                    Files =
                    {
                        new CustomerFileHeader {Title = "Jan-01 Billing", MediaGroupId = 1}
                    }
                };
                db.Customers.Add(c);
                db.SaveChanges();
            }

            using (var db = CreateDbContext<SpikesDbContext>())
            {
                Assert.That(db.Customers.Count(), Is.EqualTo(1));
                Assert.That(db.FileHeaders.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void CanAddFileContentToExternalDb()
        {
            using (var db = CreateDbContext<SpikesExternalDbContext>())
            {
                var mediaGroup = new MediaGroup();
                db.FileContents.AddRange(new[]
                {
                    new FileContent
                    {
                        FileOwnerType = "Customer",
                        ContentType = "application/image",
                        ImageSize = ImageSize.Thumbnail,
                        MediaGroup = mediaGroup
                    },
                    new FileContent
                    {
                        FileOwnerType = "Customer",
                        ContentType = "application/image",
                        ImageSize = ImageSize.Full,
                        MediaGroup = mediaGroup
                    }
                });
                db.SaveChanges();
            }

            using (var db = CreateDbContext<SpikesExternalDbContext>())
            {
                Assert.That(db.FileContents.Count(), Is.EqualTo(2));
                Assert.That(db.FileContents.Select(fc => fc.MediaGroupId).Distinct().Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void CanAddCustomerWithFileHeader_WithLinkToExternalFileContent()
        {
            var fileContent = new FileContent
            {
                FileOwnerType = "Customer",
                ContentType = "application/image",
                ImageSize = ImageSize.Full,
                MediaGroup = new MediaGroup()
            };
            using (var db = CreateDbContext<SpikesExternalDbContext>())
            {
                db.FileContents.Add(fileContent);
                db.SaveChanges();
            }
            using (var db = CreateDbContext<SpikesDbContext>())
            {
                var c = new Customer
                {
                    Name = "First Customer",
                    Files =
                    {
                        new CustomerFileHeader {Title = "Jan-01 Billing", MediaGroupId = fileContent.MediaGroupId }
                    }
                };
                db.Customers.Add(c);
                db.SaveChanges();
            }

            using (var db = CreateDbContext<SpikesDbContext>())
            {
                Assert.That(db.Customers.Count(), Is.EqualTo(1));
                Assert.That(db.FileHeaders.OfType<CustomerFileHeader>().Count(), Is.EqualTo(1));
            }
        }
    }
}