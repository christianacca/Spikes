using System.Diagnostics.CodeAnalysis;

namespace Spikes.Migrations.BaseData
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MigrationsConfig
    {
        public int? identitySeed { get; set; }
    }
}