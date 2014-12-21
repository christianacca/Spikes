using System;
using System.Collections.Generic;
using Eca.Commons.Extensions;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Collection of <see cref="IValidationRunner"/>'s that allows you to treat that collection
    /// as a single <see cref="IValidationRunner"/>
    /// </summary>
    public class CompositeValidationRunner : ValidationRunnerBase
    {
        #region Member Variables

        private readonly ICollection<IValidationRunner> _runners;

        #endregion


        #region Constructors

        public CompositeValidationRunner(IEnumerable<IValidationRunner> runners)
        {
            _runners = runners.SafeToList();
        }

        #endregion


        public override BrokenRules Validate<T>(T target, ValidationCallContext callContext)
        {
            BrokenRules result = BrokenRules.None;
            foreach (var runner in _runners)
            {
                result = BrokenRules.Merge(new[] {result, runner.Validate(target, callContext)});
                if (StopOnFirstBrokenRules) break;
            }
            return result;
        }
    }
}