using System.Data.Entity.Migrations.Infrastructure;
using log4net;

namespace Spikes.Migrations.BaseData
{
    public class Log4NetMigrationsLogger : MigrationsLogger
    {
        private readonly ILog _logger;

        public Log4NetMigrationsLogger()
        {
            _logger = LogManager.GetLogger(typeof (Log4NetMigrationsLogger));
        }

        public override void Info(string message)
        {
            _logger.Info(message);
        }

        public override void Warning(string message)
        {
            _logger.Warn(message);
        }

        public override void Verbose(string message)
        {
            _logger.Debug(message);
        }
    }
}