using System.Data.Entity.Migrations;

namespace Spikes.Migrations.DataMigrations.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<SpikesMigrationsDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SpikesMigrationsDb";
        }

        protected override void Seed(SpikesMigrationsDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
