using System.Collections.Generic;

namespace Eca.Commons.Validation
{
    public class ValidationProviderResult
    {
        #region Properties

        public BrokenRules BrokenRules { get; set; }
        public bool ObjectWasValidated { get; set; }

        #endregion


        #region Class Members

        public static ValidationProviderResult NotExecuted
        {
            get { return new ValidationProviderResult {ObjectWasValidated = false, BrokenRules = BrokenRules.None}; }
        }

        public static ValidationProviderResult Pass
        {
            get { return new ValidationProviderResult {ObjectWasValidated = true, BrokenRules = BrokenRules.None}; }
        }


        public static ValidationProviderResult Fail(IEnumerable<BrokenRule> brokenRules)
        {
            return new ValidationProviderResult {ObjectWasValidated = true, BrokenRules = new BrokenRules(brokenRules)};
        }


        public static ValidationProviderResult Fail(BrokenRule brokenRule)
        {
            return Fail(new[] {brokenRule});
        }

        #endregion
    }
}