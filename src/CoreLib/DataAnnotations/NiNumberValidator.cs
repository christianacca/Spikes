using System.ComponentModel.DataAnnotations;

namespace Eca.Commons.DataAnnotations
{
    /// <summary>
    /// Provides the regular expressions and error message that can supplied to a <see cref="RegularExpressionAttribute"/>
    /// </summary>
    public class NiNumberValidator
    {
        #region Member Variables

        public const string ErrorMsg = "NI Number appears not to be valid";

        public const string RegEx =
            @"^[A-Za-z]{2}[0-9]{6}[A-Za-z]{1}$";

        #endregion
    }
}