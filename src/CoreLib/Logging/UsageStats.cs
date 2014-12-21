using Castle.Core.Logging;

namespace Eca.Commons.Logging
{
    /// <summary>
    /// Logs usage stats for objects
    /// </summary>
    /// <remarks>
    /// By default <see cref="DefaultUsageStatsLogger"/> will be used to implement this logging
    /// </remarks>
    public class UsageStats
    {
        #region Class Members

        private static IUsageStatsLogger _defaultLogger;

        private static IUsageStatsLogger DefaultLogger
        {
            get { return _defaultLogger ?? (_defaultLogger = new DefaultUsageStatsLogger()); }
        }


        public static void Log(object obj)
        {
            DefaultLogger.Log(obj);
        }


        public static void Log(object obj, LoggerLevel level)
        {
            DefaultLogger.Log(obj, level);
        }

        #endregion
    }
}