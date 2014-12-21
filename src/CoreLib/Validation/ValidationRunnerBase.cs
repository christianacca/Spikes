using System.Linq;
using Castle.Core.Logging;
using Eca.Commons.Extensions;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Convenient base class to implement the <see cref="IValidationRunner"/> interface and to log validation activity
    /// </summary>
    public abstract class ValidationRunnerBase : IValidationRunner
    {
        #region Member Variables

        private ILogger _logger = NullLogger.Instance;
        protected ValidationPerformanceThresholds _performanceThresholds = ValidationPerformanceThresholds.Default;
        protected bool _stopOnFirstBrokenRules = true;

        #endregion


        #region Properties

        public virtual ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #endregion


        #region IValidationRunner Members

        public ValidationPerformanceThresholds PerformanceThresholds
        {
            get { return _performanceThresholds; }
            set { _performanceThresholds = value ?? ValidationPerformanceThresholds.None; }
        }

        public virtual bool StopOnFirstBrokenRules
        {
            get { return _stopOnFirstBrokenRules; }
            set { _stopOnFirstBrokenRules = value; }
        }


        public virtual BrokenRules Validate<T>(T target)
        {
            return Validate(target, null);
        }


        public abstract BrokenRules Validate<T>(T target, ValidationCallContext callContext);

        #endregion


        protected void LogBrokenRules(object provider, BrokenRules brokenRules)
        {
            if (brokenRules == null || brokenRules.IsValid) return;

            Logger.InfoFormat("Validator '{0}' identified '{1}' broken rule(s)",
                              provider,
                              brokenRules.Count());
            brokenRules.ForEach((b, i) => Logger.InfoFormat("Broken rule '{0}': {1}", i, b));
            return;
        }


        protected void LogValidationFinished(double totalDuration)
        {
            if (PerformanceThresholds.TotalInMilliseconds > 0 &&
                PerformanceThresholds.TotalInMilliseconds < totalDuration)
            {
                Logger.WarnFormat(
                    "Performance of all validations exceeds threshold; threshold: '{0}ms', actual: '{1}ms'",
                    PerformanceThresholds.TotalInMilliseconds,
                    totalDuration);
            }
            Logger.InfoFormat("Finishined validation in '{0}ms'", totalDuration);
        }


        protected void LogValidationRequested<T>(T target, ValidationCallContext callContext)
        {
            Logger.InfoFormat("Validation requested for: {0}", target);
            Logger.InfoFormat("Validation call context supplied: {0}", callContext);
            Logger.InfoFormat("Validation runner 'StopOnFirstBrokenRules': {0}", StopOnFirstBrokenRules);
        }
    }
}