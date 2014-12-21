using System;
using Castle.Core.Logging;

namespace Eca.Commons.Logging
{
    public static class LoggerExtensionMethods
    {
        #region Class Members

        /// <summary>
        /// <paramref name="message"/> will only be executed if the log <paramref name="level"/> requested is enabled.
        /// </summary>
        /// <remarks>
        /// This method is a shorthand way of expressing the following pattern: 
        /// <c>if (logger.IsWarnEnabled) logger.Warn(message());</c> 
        /// </remarks>
        public static void LogIfLevelIsEnabled(this ILogger logger, LoggerLevel level, Func<string> message)
        {
            switch (level)
            {
                case LoggerLevel.Off:
                    break;
                case LoggerLevel.Fatal:
                    if (logger.IsFatalEnabled) ExceptionSafe.ExecuteAndIgnore<Exception>(() => logger.Fatal(message()));
                    break;
                case LoggerLevel.Error:
                    if (logger.IsErrorEnabled) ExceptionSafe.ExecuteAndIgnore<Exception>(() => logger.Error(message()));
                    break;
                case LoggerLevel.Warn:
                    if (logger.IsWarnEnabled) ExceptionSafe.ExecuteAndIgnore<Exception>(() => logger.Warn(message()));
                    break;
                case LoggerLevel.Info:
                    if (logger.IsInfoEnabled) ExceptionSafe.ExecuteAndIgnore<Exception>(() => logger.Info(message()));
                    break;
                case LoggerLevel.Debug:
                    if (logger.IsDebugEnabled) ExceptionSafe.ExecuteAndIgnore<Exception>(() => logger.Debug(message()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }

        #endregion
    }
}