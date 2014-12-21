using Castle.Core.Logging;

namespace Eca.Commons.Logging
{
    public interface IUsageStatsLogger
    {
        void Log(object obj);
        void Log(object obj, LoggerLevel level);
    }
}