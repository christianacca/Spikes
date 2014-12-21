using System;
using Castle.Core.Logging;

namespace Eca.Commons.Logging
{
    /// <summary>
    /// Default implementation for usage stats logging - will log to the default infrastructure registered with
    /// <see cref="LogFactoryRegistry.Factory"/>
    /// </summary>
    /// <remarks>
    /// Will output to loggers named "UsageStats.Type" where 'Type' is the full type name of the object sent to <see cref="Log(object, LoggerLevel)"/>.
    /// Typically you will append listeners to these loggers as required by your application
    /// </remarks>
    public class DefaultUsageStatsLogger : IUsageStatsLogger
    {
        #region IUsageStatsLogger Members

        /// <summary>
        /// Outputs a <see cref="LoggerLevel.Info"/> level message with the full type name of <paramref name="obj"/> as the text
        /// </summary>
        public void Log(object obj)
        {
            Log(obj, LoggerLevel.Info);
        }


        /// <summary>
        /// Outputs a message with the full type name of <paramref name="obj"/> as the text, at the <paramref name="level"/> specified
        /// </summary>
        public void Log(object obj, LoggerLevel level)
        {
            Func<string> logMessage = () => obj.GetType().FullName;
            GetLoggerFor(obj).LogIfLevelIsEnabled(level, logMessage);
        }

        #endregion


        private ILogger GetLoggerFor(object obj)
        {
            string loggerName = "UsageStats." + obj.GetType().FullName;
            return LogFactoryRegistry.Factory.Create(loggerName);
        }
    }
}