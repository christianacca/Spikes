using System.Diagnostics;
using System.Linq;

namespace Eca.Commons.Logging
{
    public static class DotNetLoggerFactory
    {
        #region Class Members

        static DotNetLoggerFactory()
        {
            DefaultMessageOutputOptions = TraceOptions.None;
        }


        /// <summary>
        /// Determines whether such things as the date and time when the message is logged should be output as part of the message.
        /// By default this is set to <see cref="TraceOptions.None"/>
        /// </summary>
        public static TraceOptions DefaultMessageOutputOptions { get; set; }


        /// <summary>
        /// Create a logger that will output trace messages to <see cref="OutputDebugStringTraceListener"/>
        /// </summary>
        /// <param name="loggerName">The name of the logger to create</param>
        /// <param name="logLevel">Determines the message level that should be output</param>
        /// <remarks>
        /// <para>
        /// <strong>GOTCHA:</strong> see the remarks section of <see cref="OutputDebugStringTraceListener"/> documentation
        /// </para>
        /// <para>
        /// To add additional trace listeners, add to the web/app.config a <see cref="TraceSource"/> that's named
        /// the same as the value supplied to the <paramref name="loggerName"/> parameter. For an example of how 
        /// to do this see: http://blogs.msdn.com/b/bclteam/archive/2005/03/15/396431.aspx
        /// </para>
        /// </remarks>
        public static TraceSource CreateHighPerformanceLogger(string loggerName,
                                                              SourceLevels logLevel)
        {
            var logger = new TraceSource(loggerName, logLevel);

            //remove default trace listener as this is not want we want
            logger.Listeners.Remove("Default");

            //add a minimal listener that will ensure that trace output will be sent to OutputDebugString which can then be viewed
            //using a tool such as DebugView from SysInternals
            if (!logger.Listeners.OfType<OutputDebugStringTraceListener>().Any())
            {
                var traceListener =
                    new OutputDebugStringTraceListener
                        {
                            TraceOutputOptions = DefaultMessageOutputOptions
                        };
                logger.Listeners.Add(traceListener);
            }

            return logger;
        }

        #endregion
    }
}