using System;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Exceptions;

namespace Eca.Commons.Data.NHibernate.Validation
{
    public static class FluentConfigurationExtensions
    {
        #region Class Members

        /// <summary>
        /// Register <see cref="InvalidValueExtensions.ConvertToBrokenRulesException"/> as the 
        /// <see cref="NHibernateSessionExtensions.FlushExceptionConvertor"/> to use for a 
        /// <see cref="InvalidStateException"/> exception that may be thrown when flushing changes
        /// to the database
        /// </summary>
        public static IFluentConfiguration RegisterInvalidValuesToBrokenRuleConvertor(
            this IFluentConfiguration configuration)
        {
            NHibernateSessionExtensions.FlushExceptionConvertor = exception => {
                var invalidStateProblems = exception as InvalidStateException;
                return invalidStateProblems == null ? exception : invalidStateProblems.ConvertToBrokenRulesException();
            };
            return configuration;
        }

        #endregion
    }
}