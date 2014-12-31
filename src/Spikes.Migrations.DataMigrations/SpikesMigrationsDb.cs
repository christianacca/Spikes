using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Spikes.Migrations.BaseModel;

namespace Spikes.Migrations.DataMigrations
{
    public class SpikesMigrationsDb : Data.SpikesMigrationsDb
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Types()
                .Configure(c =>
                {
                    var udfProperties =
                        c.ClrType.GetProperties().Where(p => p.GetCustomAttribute<UdfAttribute>() != null).ToList();
                    udfProperties.ForEach(p => c.Ignore(p));
                });
        }
    }
}