using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;
using NValidate.Framework;

namespace Eca.Commons.Validation
{
    public interface IBrokenRuleFormatter
    {
        string Format(IEnumerable<BrokenRule> rulesToFormat);
    }



    public class PlainTextFormatter : IBrokenRuleFormatter
    {
        #region Constructors

        public PlainTextFormatter()
        {
            HeaderText = string.Empty;
        }

        #endregion


        #region Properties

        public string HeaderText { get; set; }

        #endregion


        #region IBrokenRuleFormatter Members

        public string Format(IEnumerable<BrokenRule> rulesToFromat)
        {
            return HeaderText + rulesToFromat.Join(" | ", rule => rule.Message);
        }

        #endregion
    }



    /// <summary>
    /// General representation of broken rules
    /// </summary>
    [Serializable]
    public class BrokenRules : IEnumerable<BrokenRule>
    {
        #region Member Variables

        private readonly List<BrokenRule> _rules = new List<BrokenRule>();

        #endregion


        #region Constructors

        public BrokenRules(IEnumerable<BrokenRule> rules)
        {
            _rules.AddRange(rules);
        }

        #endregion


        #region Properties

        public virtual bool IsValid
        {
            get { return _rules.Count == 0; }
        }

        #endregion


        #region IEnumerable<BrokenRule> Members

        public IEnumerator<BrokenRule> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        public virtual void Add(BrokenRule rule)
        {
            _rules.Add(rule);
        }


        #region Overridden object methods

        public override string ToString()
        {
            return ToString(new PlainTextFormatter());
        }


        public string ToString(IBrokenRuleFormatter formatter)
        {
            return formatter.Format(this);
        }

        #endregion


        #region Class Members

        public static BrokenRules None
        {
            get { return new BrokenRules(Enumerable.Empty<BrokenRule>()); }
        }


        public static BrokenRules From(ICollection<Exception> exceptions, Type source)
        {
            if (exceptions == null) return None;

            Check.Require(() => Demand.The.Param(() => exceptions).DoesNotContain(null));

            var rules =
                (from ArgumentException exception in exceptions select BrokenRule.From(exception, source)).ToList();
            return new BrokenRules(rules);
        }


        public static BrokenRules Merge(IEnumerable<BrokenRules> sets)
        {
            return new BrokenRules(sets.SkipNulls().Where(x => !x.IsValid).SelectMany(rules => rules));
        }


        public static void TryAction<T>(Action action, out BrokenRules rulesBroken)
        {
            rulesBroken = None;
            using (new Check.DisableDotNetAssertsScope())
            {
                try
                {
                    action();
                }
                catch (PreconditionException ex)
                {
                    rulesBroken = From(ex.InnerExceptions, typeof (T));
                }
                catch (BrokenRulesException ex)
                {
                    rulesBroken = ex.BrokenRules;
                }
            }
        }


        public static T TryCreateObject<T>(Func<T> creator, out BrokenRules rulesBroken)
        {
            rulesBroken = None;
            using (new Check.DisableDotNetAssertsScope())
            {
                try
                {
                    return creator();
                }
                catch (PreconditionException ex)
                {
                    rulesBroken = From(ex.InnerExceptions, typeof (T));
                }
                catch (BrokenRulesException ex)
                {
                    rulesBroken = ex.BrokenRules;
                }
                return default(T);
            }
        }

        #endregion
    }



    public enum RuleSeverity
    {
        Error,
        Warning,
        Info
    }



    [Serializable]
    public class BrokenRule : IEquatable<BrokenRule>
    {
        #region Member Variables

        private readonly string _message;
        private readonly string _propertyName;
        private readonly RuleSeverity _severity;

        #endregion


        #region Constructors

        public BrokenRule(string message)
        {
            _message = message;
        }


        public BrokenRule(string propertyName, string message) : this(message)
        {
            _propertyName = propertyName;
        }


        public BrokenRule(string propertyName, string message, RuleSeverity severity)
        {
            _propertyName = propertyName ?? string.Empty;
            _message = message ?? string.Empty;
            _severity = severity;
        }


        /// <summary>
        /// Copy constructor
        /// </summary>
        protected BrokenRule(BrokenRule copy)
        {
            Check.Require(() => Demand.The.Param(() => copy).IsNotNull());
            _message = copy._message;
            _propertyName = copy._propertyName;
            _severity = copy._severity;
            Source = copy.Source;
            ValidatorInfo = copy.ValidatorInfo;
        }

        #endregion


        #region Properties

        public string Message
        {
            get { return _message ?? String.Empty; }
        }

        public string PropertyName
        {
            get { return _propertyName ?? String.Empty; }
        }

        public RuleSeverity Severity
        {
            get { return _severity; }
        }

        /// <summary>
        /// The source of the broken rule, normally a domain model entity
        /// </summary>
        public Type Source { get; set; }

        /// <summary>
        /// Information about the validator that generated this broken rule
        /// </summary>
        public ValidatorInfo ValidatorInfo { get; set; }

        #endregion


        #region IEquatable<BrokenRule> Members

        public bool Equals(BrokenRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._message, _message) && Equals(other._propertyName, _propertyName) &&
                   Equals(other._severity, _severity) && Equals(other.Source, Source) &&
                   Equals(other.ValidatorInfo, ValidatorInfo);
        }

        #endregion


        public virtual BrokenRule Clone()
        {
            return new BrokenRule(this);
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BrokenRule)) return false;
            return Equals((BrokenRule) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = _message.GetHashCode();
                result = (result*397) ^ _propertyName.GetHashCode();
                result = (result*397) ^ _severity.GetHashCode();
                result = (result*397) ^ (Source != null ? Source.GetHashCode() : 0);
                result = (result*397) ^ (ValidatorInfo != null ? ValidatorInfo.GetHashCode() : 0);
                return result;
            }
        }


        public override string ToString()
        {
            return string.Format("{{ {0}: Severity = {1}, PropertyName = {2}, Message = {3}, ValidatorInfo = {4} }}",
                                 GetType().Name,
                                 _severity,
                                 _propertyName,
                                 _message,
                                 TextHelper.SafeToString(() => ValidatorInfo));
        }

        #endregion


        #region Class Members

        public static BrokenRule From(ArgumentException sourceException)
        {
            return ExceptionToBrokenRuleTranslator.From(sourceException);
        }


        public static BrokenRule From(ArgumentException sourceException, Type source)
        {
            return ExceptionToBrokenRuleTranslator.From(sourceException, source);
        }

        #endregion
    }
}