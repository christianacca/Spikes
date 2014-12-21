using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Validation;
using NHibernate.Validator.Engine;

namespace Eca.Commons.Data.NHibernate.Validation
{
    public class NHibernateValidationRunner : ValidationRunnerBase
    {
        #region Constructors

        public NHibernateValidationRunner(ValidatorEngine engine)
        {
            Engine = engine;
        }

        #endregion


        #region Properties

        private ValidatorEngine Engine { get; set; }

        #endregion


        private InvalidValue[] GetInvalidValues<T>(T target, ValidationCallContext callContext)
        {
            InvalidValue[] result = null;
            IEnumerable<string> tagNames = GetTags(callContext);
            double duration = With.PerformanceCounter(() => {
                result = tagNames != null
                             ? Engine.Validate(target, tagNames.ToArray())
                             : Engine.Validate(target);
            })*1000;

            LogValidationFinished(duration);
            return result;
        }


        private IEnumerable<string> GetTags(ValidationCallContext callContext)
        {
            if (callContext.NamedRulesets.Count() == 0)
            {
                //match all rules - ie tagged and untagged
                return null;
            }

            IEnumerable<string> tagNames =
                callContext.NamedRulesets.Select(name => name == ValidationCallContext.NullSelector ? null : name);
            if (tagNames.SequenceEqual(new string[] {null}))
            {
                //match untagged rules only
                return tagNames;
            }

            //match untagged rules and rules tagged with names matching ValidatorGroup names
            return tagNames.Union(new string[] {null});
        }


        public override BrokenRules Validate<T>(T target, ValidationCallContext callContext)
        {
            callContext = callContext ?? ValidationCallContext.AllRules;
            LogValidationRequested(target, callContext);

            IEnumerable<InvalidValue> invalidValues = GetInvalidValues(target, callContext);
            BrokenRules brokenRules = invalidValues.ToBrokenRules();
            LogBrokenRules(this, brokenRules);
            return brokenRules;
        }
    }
}