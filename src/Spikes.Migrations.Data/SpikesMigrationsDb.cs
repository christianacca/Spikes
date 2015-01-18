using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Pluralization;
using Spike.Migrations.Model;
using Spikes.Migrations.BaseData;
using Spikes.Migrations.BaseModel;

namespace Spikes.Migrations.Data
{
    public class SpikesMigrationsDb : SpikesMigrationsBaseDb
    {
        public SpikesMigrationsDb()
        {
        }

        public SpikesMigrationsDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public SpikesMigrationsDb(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<Asset> Assets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set default schema...

            // we can't set a default schema that all tables should have because of a limitation of using automatic migrations
            modelBuilder.HasDefaultSchema(null);
            var mainModelAssembly = typeof(Asset).Assembly;
            modelBuilder.Types()
                .Where(t => t.Assembly == mainModelAssembly)
                .Configure(c => c.ToTable(new EnglishPluralizationService().Pluralize(c.ClrType.Name), "Main"));

            // note: this is a workaround to the standard way of mapping Table-per-hierarchy mapping for UserRole
            // we're doing this so that the single table get's created in the schema we want all tables
            var userRoleType = typeof(UserRole);
            modelBuilder.Types()
                .Where(t => t.Assembly == mainModelAssembly)
                .Where(userRoleType.IsAssignableFrom)
                .Configure(c => c.ToTable(new EnglishPluralizationService().Pluralize(userRoleType.Name), BaseSchemaName));
        }
    }
}