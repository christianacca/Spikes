using System;
using System.Diagnostics;
using Eca.Commons.Extensions;

namespace Eca.Commons.Logging
{
    public static class TraceSourceExtensions
    {
        #region Class Members

        private static string SafeSerialiseArgs(object[] args)
        {
            try
            {
                return String.Format("| args: {0}", args.Join(", "));
            }
            catch (Exception)
            {
                //yes we are catching all exceptions here, this is because tracing should not introduced exceptions when tracing is turned on
                return String.Empty;
            }
        }


        /// <summary>
        /// Output trace messages, signalling the start, end and exception occuring during the <paramref name="methodBody"/> being executed
        /// </summary>
        /// <param name="logger">The logger that will be used to output the messages</param>
        /// <param name="methodName">The name of the method whose code is being supplied as <paramref name="methodBody"/></param>
        /// <param name="methodBody">The code that should be traced</param>
        /// <param name="args">Any arguments that code inside <paramref name="methodBody"/> has received that you want to be include in the message signalling the start of the method being traced</param>
        /// <example>
        /// Typically you want to call this method as follows (assumes the method you want to be traced is InitialiseForWeb in a class named CrmProgram):
        /// <code>
        /// public static void InitialiseForWeb()
        /// {
        ///     DotNetLogger.TraceMethodCall("CrmProgram.InitialiseForWeb",
        ///                                     () => {
        ///                                         FlagAsInsideAppStart();
        ///                                         DoSharedWebAndDesktopInitialisation();
        ///                                         HandlingMultipleRoleProviders.ThrowIfNoProviderForRequest = false;
        ///                                     });
        /// }
        /// </code>
        /// </example>
        public static void TraceMethodCall(this TraceSource logger,
                                           string methodName,
                                           Action methodBody,
                                           params object[] args)
        {
            Func<object> action = () => {
                methodBody();
                return null;
            };
            logger.TraceMethodCall(methodName, action, args);
        }


        /// <seealso cref="TraceMethodCall"/>
        public static T TraceMethodCall<T>(this TraceSource logger,
                                           string methodName,
                                           Func<T> methodBody,
                                           params object[] args)
        {
            try
            {
                string message = String.Format("Begin: {0}", methodName);
                if (args.SafeCount() > 0)
                {
                    message += SafeSerialiseArgs(args);
                }
                logger.TraceInformation(message);
                try
                {
                    T result = methodBody();
                    logger.TraceInformation(String.Format("End: {0}", methodName));
                    return result;
                }
                catch (Exception e)
                {
                    logger.TraceEvent(TraceEventType.Error,
                                      0,
                                      String.Format("Exception: {0}; Details: {1}", methodName, e));
                    throw;
                }
            }
            finally
            {
                logger.Flush();
            }
        }

        #endregion
    }
}