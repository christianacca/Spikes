using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Spikes.Migrations.BaseData
{
    public class CustomDbConfiguration : DbConfiguration
    {
        public CustomDbConfiguration()
        {
            SetMigrationSqlGenerator(SqlProviderServices.ProviderInvariantName,
                () => new IdSeedSqlServerMigrationSqlGenerator());
        }
    }
}