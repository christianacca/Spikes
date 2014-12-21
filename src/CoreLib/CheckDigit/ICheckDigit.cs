namespace Eca.Commons.CheckDigit
{
    /// <summary>
    /// <b>Check Digit</b> calculation and validation.
    /// </summary>
    public interface ICheckDigit
    {
        /// <summary>
        /// Calculate the <i>Check Digit</i> for a code.
        /// </summary>
        /// <param name="value">The code to calculate the Check Digit for.</param>
        /// <returns>The calculated Check Digit</returns>
        string CalculateDigit(string value);


        /// <summary>
        /// Validates <paramref name="value"/> whose <em>last digit</em> is a check digit computed from
        /// the remaining <paramref name="value"/>
        /// </summary>
        /// <param name="value">Value to validate</param>
        bool IsValid(string value);
    }
}