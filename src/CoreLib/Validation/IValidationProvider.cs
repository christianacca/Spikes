using System.Collections.Generic;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Untyped version of the interface <see cref="IValidationProvider{T}"/>.
    /// </summary>
    /// <remarks>
    /// Use this interface when you need to reference a validator provider(s) with a generic-free reference
    /// </remarks>
    public interface IValidationProvider : IValidationProviderBase
    {
        bool Accepts(object candidate);
        bool Accepts(object candidate, ValidationCallContext callContext);
        ValidationProviderResult Validate(object target);
        ValidationProviderResult Validate(object target, ValidationCallContext callContext);
    }



    /// <summary>
    /// Base interface for all validator providers
    /// </summary>
    public interface IValidationProviderBase
    {
        /// <summary>
        /// Determines the priority order when executing validation. The smaller the number the higher the priority.
        /// The provider with the highest priorty will be executed before other provider.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// The list of Rulesets this validator has validation rules for. If empty, this will be interpretted as this validator
        /// accepting all requested rulesets.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This list can be used as a coursely grained means of determining whether this validator will accept a target 
        /// object for validation.
        /// </para>
        /// <para>
        /// For finer grained conditions, you should test the object in the implementation of Accepts.
        /// </para>
        /// <para>
        /// The rulesets specified here are intended to passed to <see cref="ValidationCallContext.IsAtLeastOneRulesetInScope"/>
        /// </para>
        /// </remarks>
        /// <seealso cref="ValidationCallContext.RuleScope"/>
        IEnumerable<string> Rulesets { get; }
    }



    /// <summary>
    /// The interface that defines the members that a validator provider must implement
    /// </summary>
    /// <remarks>
    /// <para>
    /// While implementing this interface is quite easy to do, it is <b>strongly</b> advised that your class does
    /// <b>not</b> implement the interface directly. Instead have your class inherit from <see
    /// cref="ValidationProvider{T}"/> or FluentValidationProvider.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of the object to be validated</typeparam>
    public interface IValidationProvider<T> : IValidationProviderBase
    {
        /// <summary>
        /// Tests whether <paramref name="candidate"/> should be validated by this provider
        /// </summary>
        /// <param name="candidate">The object that is a candidate for validation</param>
        /// <param name="callContext">Contextual information that determines what/if validation should be perform</param>
        /// <returns></returns>
        bool Accepts(T candidate, ValidationCallContext callContext);


        /// <summary>
        /// Tests whether <paramref name="candidate"/> should be validated by this provider
        /// </summary>
        /// <param name="candidate">The object that is a candidate for validation</param>
        /// <returns></returns>
        bool Accepts(T candidate);


        /// <summary>
        /// Validates <paramref name="target"/> against a set of rules, and returns a <see
        /// cref="ValidationProviderResult"/> to indicate the outcome of that validation.
        /// </summary>
        /// <remarks>
        /// Note to implementor: its important that the first thing this method does is call <see cref="Accepts(T,ValidationCallContext)"/>. Only
        /// when <see cref="Accepts(T,ValidationCallContext)"/> returns true should the validation of <paramref name="target"/> proceed. If
        /// <paramref name="target"/> is not accepted for validation then this method should return <see cref="ValidationProviderResult.Pass"/>
        /// </remarks>
        ValidationProviderResult Validate(T target);


        /// <summary>
        /// Validates <paramref name="target"/> against a set of rules, and returns a <see
        /// cref="ValidationProviderResult"/> to indicate the outcome of that validation.
        /// </summary>
        /// <remarks>
        /// Note to implementor: its important that the first thing this method does is call <see cref="Accepts(T,ValidationCallContext)"/>. Only
        /// when <see cref="Accepts(T,ValidationCallContext)"/> returns true should the validation of <paramref name="target"/> proceed. If
        /// <paramref name="target"/> is not accepted for validation then this method should return <see cref="ValidationProviderResult.Pass"/>
        /// </remarks>
        ValidationProviderResult Validate(T target, ValidationCallContext callContext);
    }
}