using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;

namespace Eca.Commons.Validation
{
    public interface IValidationRunner
    {
        ValidationPerformanceThresholds PerformanceThresholds { get; set; }

        bool StopOnFirstBrokenRules { get; set; }


        /// <summary>
        /// Executes all validations for the <paramref name="target"/> to be validated
        /// </summary>
        /// <remarks>
        /// Equivalent of supplying <see cref="ValidationCallContext.AllRules"/> to <see cref="Validate{T}(T,Eca.Commons.Validation.ValidationCallContext)"/>
        /// </remarks>
        BrokenRules Validate<T>(T target);


        /// <summary>
        /// Executes the validations for the <paramref name="target"/> that are applicable given the <paramref name="callContext"/>
        /// </summary>
        BrokenRules Validate<T>(T target, ValidationCallContext callContext);
    }



    /// <summary>
    /// Executes all <see cref="ValidationProvider{T}"/> that are returned by the <see cref="IFactory"/> supplied
    /// </summary>
    /// <remarks>
    /// The chances are high that <see cref="IFactory"/> is a simple wrapper around an inversion of control container
    /// such as Castle Winsdor. Whichever validation providers have been registered with the container will be what <see
    /// cref="IFactory"/> will return as the set of providers that will execute the validation
    /// </remarks>
    public class ValidationRunner : ValidationRunnerBase
    {
        #region Constructors

        public ValidationRunner(IFactory factory)
        {
            ProviderFactory = factory;
        }

        #endregion


        #region Properties

        private IFactory ProviderFactory { get; set; }

        #endregion


        private ValidationProviderResult ExecuteValidation<T>(T target,
                                                              ValidationCallContext callContext,
                                                              IValidationProvider<T> validator,
                                                              out double duration)
        {
            ValidationProviderResult result = null;
            duration = With.PerformanceCounter(() => {
                result = validator.Validate(target, callContext);
            })*1000;
            Logger.DebugFormat("Validator '{0}' executed in '{1}ms'", validator, duration);
            if (PerformanceThresholds.IndividualValidatorInMilliseconds > 0 &&
                PerformanceThresholds.IndividualValidatorInMilliseconds < duration)
            {
                Logger.WarnFormat(
                    "Performance of validator '{0}' exceeds threshold; threshold: '{1}ms', actual: '{2}ms'",
                    validator,
                    PerformanceThresholds.IndividualValidatorInMilliseconds,
                    duration);
            }
            return result;
        }


        private ICollection<IValidationProvider<T>> GetValidationProviders<T>()
        {
            Type genericProviderType = typeof (IValidationProvider<T>);
            var providers = ProviderFactory.NewAll(genericProviderType);
            var orderedProviders = providers
                .OfType<IValidationProvider<T>>()
                .OrderBy(provider => provider.Priority).ToList();
            return orderedProviders;
        }


        protected void LogBrokenRules<T>(ValidationProviderResult result, IValidationProvider<T> provider)
        {
            if (!result.ObjectWasValidated)
            {
                Logger.InfoFormat("Validator '{0}' choose not to accept target", provider);
                return;
            }
            LogBrokenRules(provider, result.BrokenRules);
        }


        private void LogValidatorsAboutToBeApplied<T>(ICollection<IValidationProvider<T>> providers, T target)
        {
            if (providers.Count == 0)
            {
                Logger.WarnFormat("No validators registered for type '{0}'; no validation to be applied to '{1}'",
                                  typeof (T).Name,
                                  target);
                return;
            }

            Logger.InfoFormat("'{0}' validator(s) regiserted for type '{1}'", providers.Count, typeof (T).Name);
            providers.ForEach((v, i) => Logger.InfoFormat("Validator '{0}': {1}", i, v));
            Logger.Info("Starting validation");
        }


        public override BrokenRules Validate<T>(T target, ValidationCallContext callContext)
        {
            callContext = callContext ?? ValidationCallContext.AllRules;

            LogValidationRequested(target, callContext);
            ICollection<IValidationProvider<T>> providers = GetValidationProviders<T>();

            LogValidatorsAboutToBeApplied(providers, target);

            var results = new List<BrokenRules>();
            double totalDuration = 0d;
            foreach (IValidationProvider<T> provider in providers)
            {
                double duration;
                ValidationProviderResult result = ExecuteValidation(target, callContext, provider, out duration);
                totalDuration += duration;
                LogBrokenRules(result, provider);
                results.Add(result.BrokenRules);
                if (!result.BrokenRules.IsValid && StopOnFirstBrokenRules) break;
            }

            LogValidationFinished(totalDuration);
            return BrokenRules.Merge(results);
        }
    }
}