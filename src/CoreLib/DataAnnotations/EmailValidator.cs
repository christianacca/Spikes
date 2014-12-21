using System.ComponentModel.DataAnnotations;

namespace Eca.Commons.DataAnnotations
{
    /// <summary>
    /// Provides the regular expressions and error message that can supplied to a <see cref="RegularExpressionAttribute"/>
    /// </summary>
    public class EmailValidator
    {
        #region Member Variables

        public const string ErrorMsg = "Email appears not to be valid";

        public const string RegEx =
            @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";

        #endregion
    }
}