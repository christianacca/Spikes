using System.Data.Common;
using System.Data.Entity;
using Spikes.EntityFramework.Models.Bidirectional.Conventional;
using Spikes.EntityFramework.Models.Bidirectional.NonStandard;

namespace Spikes.EntityFramework
{
    using Spikes.EntityFramework.Models.Unidirectional.Conventional;

    public class SpikesDbContext : DbContext
    {
        public SpikesDbContext()
        {
        }

        public SpikesDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public SpikesDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderNonStd> OrdersNonStd { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FileHeader> FileHeaders { get; set; }
        public DbSet<CustomerFileHeader> CustomerFileHeaders { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileContentInfo>().HasKey(f => f.FilterHeaderId)
                .HasRequired(f => f.FileHeader).WithOptional(fh => fh.ContentInfo);

            // Primary Key property doesn't match EF convention - expected 'Id' or 'OrderNonStdId', but is 'MyId'

            modelBuilder.Entity<OrderNonStd>().HasKey(x => x.MyId);

            // Foreign Key property 'PrimaryOrderId' doesn't match EF conventions, that is the name is expected to
            // by the name of the navigation property plus 'Id' suffix ie 'OrderId'
            modelBuilder.Entity<OrderLineNonStd>()
                        .HasRequired(ol => ol.Order)
                        .WithMany(o => o.Lines)
                        .HasForeignKey(x => x.PrimaryOrderId);
        }
    }
}