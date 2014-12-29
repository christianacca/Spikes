using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using Spikes.Migrations.BaseModel;

namespace Spikes.Migrations.BaseData
{
    public class SpikesMigrationsBaseDb : DbContext
    {
        public SpikesMigrationsBaseDb()
        {
        }

        public SpikesMigrationsBaseDb(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<LookupItem> LookupItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // set default schema
            var baseModelAssembly = typeof (LookupItem).Assembly;
            modelBuilder.Types()
                .Where(t => t.Assembly == baseModelAssembly)
                .Configure(c => c.ToTable(new EnglishPluralizationService().Pluralize(c.ClrType.Name), "Base"));
            var userRoleType = typeof (UserRole);
            modelBuilder.Types()
                .Where(t => userRoleType.IsAssignableFrom(t))
                .Configure(c => c.ToTable(new EnglishPluralizationService().Pluralize(userRoleType.Name), "Base"));
            modelBuilder.HasDefaultSchema("Base");
        }
    }
}