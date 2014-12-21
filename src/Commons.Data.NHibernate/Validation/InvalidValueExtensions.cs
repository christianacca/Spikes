using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;
using Eca.Commons.Validation;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Exceptions;

namespace Eca.Commons.Data.NHibernate.Validation
{
    public static class InvalidValueExtensions
    {
        #region Class Members

        public static Exception ConvertToBrokenRulesException(this InvalidStateException invalidStateProblems)
        {
            BrokenRules brokenRules = invalidStateProblems.GetInvalidValues().ToBrokenRules();
            return new BrokenRulesException("Problem writing changes to database. See BrokenRules", brokenRules);
        }


        private static BrokenRule ToBrokenRule(this InvalidValue iv)
        {
            return new BrokenRule(iv.PropertyPath, iv.Message, RuleSeverity.Error)
                       {Source = iv.RootEntity.SafeGetType()};
        }


        public static BrokenRules ToBrokenRules(this IEnumerable<InvalidValue> source)
        {
            if (source.Safe().Count() == 0) return BrokenRules.None;

            return new BrokenRules(source.Select(iv => iv.ToBrokenRule()));
        }

        #endregion
    }
}