using Castle.Core.Logging;

namespace Eca.Commons.Logging
{
    /// <summary>
    /// Use this class to register a <see cref="ILoggerFactory"/>. Every other
    /// class that needs to output hand-crafted log entries will lookup the
    /// logger factory from this registry in order to create a <see cref="ILogger"/>
    /// </summary>
    public class LogFactoryRegistry
    {
        #region Class Members

        private static ILoggerFactory _factory = new NullLogFactory();

        public static ILoggerFactory Factory
        {
            get { return _factory; }
        }


        public static void SetFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        #endregion
    }
}