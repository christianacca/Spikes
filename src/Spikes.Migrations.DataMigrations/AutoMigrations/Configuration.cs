using System.Data.Entity.Migrations;

namespace Spikes.Migrations.DataMigrations.AutoMigrations
{
    public sealed class Configuration : DbMigrationsConfiguration<Data.SpikesMigrationsDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Spikes.Migrations.Data.SpikesMigrationsDb";
        }

        protected override void Seed(Data.SpikesMigrationsDb context)
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
