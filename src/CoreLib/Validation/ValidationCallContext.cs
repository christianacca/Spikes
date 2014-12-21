using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;
using NValidate.Framework;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Contextual information that determines what/if validation should be perform
    /// </summary>
    public class ValidationCallContext
    {
        #region Member Variables

        /// <summary>
        /// Special case selector that will selects rules that are not explictly associated with a Ruleset
        /// </summary>
        public const string NullSelector = "null";

        /// <summary>
        /// Special case selector that will select both rules explicitly associated with a Ruleset and those that are not
        /// </summary>
        public const string WildcardSelector = "*";

        private string _filtersetName = String.Empty;
        private string _rulesetName;

        #endregion


        #region Properties

        /// <summary>
        /// Don't make public as this want to be able change design
        /// </summary>
        private string FiltersetName
        {
            get { return _filtersetName; }
            set { _filtersetName = value.SafeTrim() ?? String.Empty; }
        }

        /// <summary>
        /// The name(s) of any <em>explictly</em> named Ruleset that was supplied when setting <see cref="RuleScope"/>
        /// </summary>
        /// <remarks>
        /// If a <see cref="RuleScope"/> was supplied with a <see cref="WildcardSelector"/>, an empty collection will be returned
        /// </remarks>
        public IEnumerable<string> NamedRulesets
        {
            get
            {
                if (RulesetName != WildcardSelector)
                {
                    yield return RulesetName;
                }
            }
        }


        /// <summary>
        /// Passed to a validator to indicate the scope of validation required. For example a value of 
        /// 'Membership' indicates that validators that respond to the ruleset 'Membership' should execute 
        /// their rules.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The string supplied has the following component parts: <code>ruleset:filterset</code>
        /// </para>
        /// <para>
        /// <em>ruleset</em> is the name of a ruleset. Each validator will have zero-to-many rulesets that they will
        /// respond to. <strong>Tip:</strong> A validator that has zero rulesets will respond to all ruleset names
        /// </para>
        /// <para>
        /// <em>:filterset</em> is optional. It is the name of a filter that a validator may be using to group together
        /// some of its rules. 
        /// </para>
        /// <para>
        /// Supplying a filterset would cause a validator to execute <em>all its unfiltered rules</em>,
        /// plus those rules it has associated with that filter. Not suppling a filterset would cause a
        /// validator to execute all its unfiltered rules, but skip rules it has associated with a filter.
        /// </para>
        /// <para>
        /// The <see cref="WildcardSelector"/> can be used to indicate that all validators should apply, and/or that
        /// all filtered rules should apply.
        /// </para>
        /// <example>
        /// <code>Memebership</code>all validators with the ruleset 'Membership' will apply plus all validators that are <em>not</em> associated with any ruleset. Those rules that a validator has associated with a filter will not run
        /// <code>Memebership:Save</code>all validators which have a ruleset of 'Memebership' will apply plus all validators that are <em>not</em> associated with any ruleset. Validators will execute their non-filtered rules and all the rules associated with a filter named 'Save'
        /// <code>*:Save</code>all validators apply. Validators will execute their non-filtered rules and all the rules associated with a filter named 'Save'
        /// <code>*</code>all validators will apply their non filtered rules
        /// <code>*:*</code>all validators will apply all of their rules
        /// </example>
        /// </remarks>
        /// <seealso cref="IValidationProviderBase.Rulesets"/>
        public string RuleScope
        {
            get
            {
                return String.IsNullOrEmpty(FiltersetName)
                           ? String.Format("{0}", RulesetName)
                           : String.Format("{0}:{1}", RulesetName, FiltersetName);
            }

            set
            {
                RulesetName = String.Empty;
                FiltersetName = String.Empty;

                var ruleScopeParts = value.Split(new[] {":"}, StringSplitOptions.None);
                RulesetName = ruleScopeParts[0];
                if (ruleScopeParts.Length > 1)
                {
                    FiltersetName = ruleScopeParts[1];
                }
            }
        }

        /// <summary>
        /// Don't make public as this want to be able change design
        /// </summary>
        private string RulesetName
        {
            get { return _rulesetName.IfNullOrEmpty(WildcardSelector); }
            set { _rulesetName = value.SafeTrim(); }
        }


        /// <summary>
        /// Determines whether the validator can choose to skip validations of the target object or any of the target
        /// object's properties based on whether or not the properties of the target have been touched (the property set called).
        /// </summary>
        /// <remarks>
        /// <para>When set to <c>false</c>, a validator must <em>always</em> perform its validation</para>
        /// <para>When set to <c>true</c>, a validator <em>can</em> choose to ignore this setting and go ahead and validate anyway</para>
        /// </remarks>
        public bool ValidatorsCanSkipWhenUntouched { get; set; }

        #endregion


        /// <summary>
        /// Compares the <paramref name="filtersets"/> against the 'filterset' component of the <see cref="RuleScope"/> string to determine a match
        /// </summary>
        public bool IsAtLeastOneFiltersetInScope(IEnumerable<string> filtersets)
        {
            return IsAtLeastOneSetInScope(filtersets, FiltersetName);
        }


        /// <summary>
        /// Compares the <paramref name="rulesets"/> against the 'ruleset' component of the <see cref="RuleScope"/> string to determine a match
        /// </summary>
        public bool IsAtLeastOneRulesetInScope(IEnumerable<string> rulesets)
        {
            return IsAtLeastOneSetInScope(rulesets, RulesetName);
        }


        private bool IsAtLeastOneSetInScope(IEnumerable<string> setNames, string requiredSetName)
        {
            Check.Require(() => Demand.The.Param(() => setNames).IsNotNull());

            //match all
            if (requiredSetName == WildcardSelector) return true;

            //match only when no explicit set
            if (requiredSetName == NullSelector) return !setNames.Any();

            //if we are here then: requiredSetName must be a string other than a wildcard of null selector...

            //a requiredSetName should always match an empty set; this is so that rules that are not associated with
            //a set will get selected
            if (!setNames.Any()) return true;

            return setNames.Select(rs => rs.Trim()).Contains(requiredSetName);
        }


        #region Overridden object methods

        public override string ToString()
        {
            return String.Format("{{ {0}: RuleScope = {1}, ValidatorsCanSkipWhenUntouched = {2} }}",
                                 GetType().Name,
                                 RuleScope,
                                 ValidatorsCanSkipWhenUntouched);
        }

        #endregion


        #region Class Members

        /// <summary>
        /// Defines a <see cref="RuleScope"/> of <em>"*:*"</em> and informs validators to always perform validations even 
        /// when the parts of the target object may be untouched
        /// </summary>
        public static ValidationCallContext AllRules
        {
            get { return new ValidationCallContext {ValidatorsCanSkipWhenUntouched = false, RuleScope = "*:*"}; }
        }

        /// <summary>
        /// Defines a <see cref="RuleScope"/> of <em>"*"</em> and informs validators thay they may skip validations of the 
        /// parts of the target object that may be untouched
        /// </summary>
        public static ValidationCallContext ForSave
        {
            get { return new ValidationCallContext {ValidatorsCanSkipWhenUntouched = true, RuleScope = "*"}; }
        }

        #endregion
    }
}