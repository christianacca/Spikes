using Eca.Commons.Extensions;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Summary information about a validator
    /// </summary>
    public class ValidatorInfo
    {
        #region Constructors

        public ValidatorInfo(string validatorName)
        {
            ValidatorName = validatorName;
        }

        #endregion


        #region Properties

        public string ValidatorName { get; set; }

        #endregion


        #region Overridden object methods

        public override string ToString()
        {
            return string.Format("{{ {0}: ValidatorName = {1} }}", GetType().Name, ValidatorName);
        }

        #endregion


        #region Class Members

        /// <summary>
        /// Create a <see cref="ValidatorInfo"/> for the supplied <paramref name="validator"/>
        /// </summary>
        public static ValidatorInfo For(IValidationProviderBase validator)
        {
            return new ValidatorInfo(validator.ClassName());
        }

        #endregion
    }
}