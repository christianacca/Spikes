using System;
using System.Collections.Generic;
using Eca.Commons.Extensions;
using NValidate.Framework;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Untyped version of the class <see cref="ValidationProvider{T}"/>
    /// </summary>
    public abstract class ValidationProvider : IValidationProvider
    {
        #region Member Variables

        public const int HighestPriority = Int32.MinValue;


        private ICollection<string> _rulesets = new List<string>();

        #endregion


        #region IValidationProvider Members

        public bool Accepts(object candidate)
        {
            return Accepts(candidate, new ValidationCallContext());
        }


        public bool Accepts(object candidate, ValidationCallContext callContext)
        {
            Check.Require(() => Demand.The.Param(() => callContext).IsNotNull());

            if (ReferenceEquals(candidate, null) || !callContext.IsAtLeastOneRulesetInScope(Rulesets)) return false;

            return DoAccepts(candidate, callContext);
        }


        public virtual int Priority { get; set; }

        public IEnumerable<string> Rulesets
        {
            get { return _rulesets; }
            set { _rulesets = value.SafeToList(); }
        }


        public ValidationProviderResult Validate(object target)
        {
            return Validate(target, new ValidationCallContext());
        }


        public ValidationProviderResult Validate(object target, ValidationCallContext callContext)
        {
            if (!Accepts(target, callContext)) return ValidationProviderResult.NotExecuted;

            return DoValidate(target, callContext);
        }

        #endregion


        protected abstract bool DoAccepts(object candidate, ValidationCallContext callContext);


        protected abstract ValidationProviderResult DoValidate(object target, ValidationCallContext callContext);


        #region Overridden object methods

        public override string ToString()
        {
            return string.Format("{{ {0}: Priority = {1}, Rulesets = {2} }}",
                                 GetType().Name,
                                 Priority,
                                 _rulesets.Safe().Join(","));
        }

        #endregion
    }



    /// <summary>
    /// Convenience base class for implementing the <see cref="IValidationProvider{T}"/> interface. 
    /// </summary>
    /// <remarks>
    /// Inheriting from this class is the prefered way to author a validation provider that implements the
    /// <see cref="IValidationProvider{T}"/> interface as it affors a level of future
    /// proofing - if a new method or property is added to <see cref="IValidationProvider{T}"/> (or <see
    /// cref="IValidationProvider"/>), subclasses will automatically inherit any default implementation
    /// </remarks>
    /// <typeparam name="T">Type of the object to be validated</typeparam>
    public abstract class ValidationProvider<T> : ValidationProvider, IValidationProvider<T>, IValidationProvider
        where T : class
    {
        #region IValidationProvider Members

        bool IValidationProvider.Accepts(object candidate, ValidationCallContext callContext)
        {
            return Accepts(candidate as T, callContext);
        }


        bool IValidationProvider.Accepts(object candidate)
        {
            return Accepts(candidate as T);
        }


        ValidationProviderResult IValidationProvider.Validate(object target, ValidationCallContext callContext)
        {
            return Validate(target as T, callContext);
        }


        ValidationProviderResult IValidationProvider.Validate(object target)
        {
            return Validate(target as T);
        }

        #endregion


        #region IValidationProvider<T> Members

        public bool Accepts(T candidate, ValidationCallContext callContext)
        {
            return base.Accepts(candidate, callContext);
        }


        public bool Accepts(T candidate)
        {
            return Accepts(candidate, new ValidationCallContext());
        }


        public ValidationProviderResult Validate(T target)
        {
            return Validate(target, new ValidationCallContext());
        }


        public ValidationProviderResult Validate(T target, ValidationCallContext callContext)
        {
            return base.Validate(target, callContext);
        }

        #endregion


        protected override sealed bool DoAccepts(object candidate, ValidationCallContext callContext)
        {
            return DoAccepts(candidate as T, callContext);
        }


        /// <summary>
        /// This providers implementation of the <see cref="IValidationProvider{T}.Accepts(T,ValidationCallContext)"/> method
        /// </summary>
        /// <seealso cref="IValidationProvider{T}.Validate(T,ValidationCallContext)"/>
        protected abstract bool DoAccepts(T candidate, ValidationCallContext callContext);


        protected override sealed ValidationProviderResult DoValidate(object target, ValidationCallContext callContext)
        {
            return DoValidate(target as T, callContext);
        }


        /// <summary>
        /// This providers implementation of the <see cref="IValidationProvider{T}.Validate(T,ValidationCallContext)"/> method
        /// </summary>
        /// <seealso cref="IValidationProvider{T}.Accepts(T,ValidationCallContext)"/>
        protected abstract ValidationProviderResult DoValidate(T target, ValidationCallContext callContext);
    }
}