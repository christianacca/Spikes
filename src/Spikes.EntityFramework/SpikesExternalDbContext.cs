using System.Data.Common;
using System.Data.Entity;
using Spikes.EntityFramework.Models.Bidirectional.ExternalDb;

namespace Spikes.EntityFramework
{
    public class SpikesExternalDbContext : DbContext
    {
        public SpikesExternalDbContext()
        {
        }

        public SpikesExternalDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<FileContent> FileContents { get; set; }
    }
}