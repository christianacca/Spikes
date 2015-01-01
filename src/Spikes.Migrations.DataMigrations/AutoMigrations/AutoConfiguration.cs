using System.Data.Entity.Migrations;

namespace Spikes.Migrations.DataMigrations.AutoMigrations
{
    /// <summary>
    /// This configuration will include udf properties in the model and will use automatic migrations
    /// </summary>
    public sealed class AutoConfiguration : DbMigrationsConfiguration<Data.SpikesMigrationsDb>
    {
        public AutoConfiguration()
        {
            MigrationsDirectory = "AutoMigrations";
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Auto.SpikesMigrationsDb";
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
