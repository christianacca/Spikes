using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Maps <see cref="BrokenRule"/> from one object to another
    /// </summary>
    public class BrokenRuleObjectToObjectMapper
    {
        /// <seealso cref="Map{T}(System.Collections.Generic.IEnumerable{BrokenRule},OneToManyObjectToObjectMap{T})"/>
        public BrokenRules Map<TDest>(BrokenRules brokenRules, OneToManyObjectToObjectMap<TDest> mappings)
        {
            return new BrokenRules(Map(brokenRules.AsEnumerable(), mappings));
        }


        /// <summary>
        /// Attempts to map the <see cref="BrokenRule"/>(s) supplied, to the properties of <typeparamref
        /// name="TDest"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="BrokenRule.PropertyName"/> of a <see cref="BrokenRule"/>, associates the rule with a failing
        /// property on the <see cref="BrokenRule.Source"/> object. This property name is the key peice of information
        /// used by many frameworks for associating an error message with an input element on the screen.
        /// </para>
        /// <para>
        /// The problem is, the object used to carry the data to the screen is often not the same as the <see
        /// cref="BrokenRule.Source"/>, and often the property names do not match up.
        /// </para>
        /// <para>
        /// This method attempts to adjust the <see cref="BrokenRule.PropertyName"/> so that it uses the name of the
        /// property on <typeparamref name="TDest"/> to which the <see cref="BrokenRule"/> has been mapped.
        /// </para>
        /// </remarks>
        /// <param name="brokenRules">The broken rules that are to be mapped to properties declared by <typeparamref name="TDest"/></param>
        /// <param name="mappings">Defines the mapping between <see cref="BrokenRule.PropertyName"/> and <typeparamref name="TDest"/></param>
        public ICollection<BrokenRule> Map<TDest>(IEnumerable<BrokenRule> brokenRules,
                                                  OneToManyObjectToObjectMap<TDest> mappings)
        {
            return brokenRules.Select(r => MapRule(r, mappings)).ToList();
        }


        private BrokenRule MapRule<TDest>(BrokenRule rule, OneToManyObjectToObjectMap<TDest> mappings)
        {
            return
                new BrokenRule(
                    mappings.FindSourcePropertyOnDestination(rule.Source, rule.PropertyName)
                        .IfNullOrEmpty(rule.PropertyName),
                    rule.Message,
                    rule.Severity) {Source = rule.Source, ValidatorInfo = rule.ValidatorInfo};
        }


        #region Class Members

        static BrokenRuleObjectToObjectMapper()
        {
            Instance = new BrokenRuleObjectToObjectMapper();
        }


        public static BrokenRuleObjectToObjectMapper Instance { get; private set; }

        #endregion
    }
}