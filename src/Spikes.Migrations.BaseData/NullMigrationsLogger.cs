using System.Data.Entity.Migrations.Infrastructure;

namespace Spikes.Migrations.BaseData
{
    /// <summary>
    /// An logger that ignores log messages
    /// </summary>
    public class NullMigrationsLogger : MigrationsLogger
    {
        public static NullMigrationsLogger Instance = new NullMigrationsLogger();

        public override void Info(string message)
        {
        }

        public override void Warning(string message)
        {
        }

        public override void Verbose(string message)
        {
        }
    }
}