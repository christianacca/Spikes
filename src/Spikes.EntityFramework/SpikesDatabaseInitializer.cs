using System.Data.Entity;

namespace Spikes.EntityFramework
{
    public class SpikesDatabaseInitializer : DropCreateDatabaseIfModelChanges<SpikesDbContext>
    {
    }
}