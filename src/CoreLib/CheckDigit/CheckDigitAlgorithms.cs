namespace Eca.Commons.CheckDigit
{
    public static class CheckDigitAlgorithms
    {
        #region Class Members

        private static readonly VerhoeffCheckDigit _verhoeff = new VerhoeffCheckDigit();

        /// <remarks>
        /// Benefits / properties of the Verhoeff algorithm:
        /// <list type="bullet">
        /// <item>Calculates a single digit</item>
        /// <item>It catches more errors than most other schemes (schemes that add more than one check digit can catch more errors, at the expense of adding more digits)</item>
        /// <item>It only returns decimal digits (0-9), so does not require an alpha "keyboard" on a touch screen display </item>
        /// <item>It can be used on a number with any number of digits</item>
        /// </list>
        /// </remarks>>
        public static ICheckDigit Verhoeff
        {
            get { return _verhoeff; }
        }

        #endregion
    }
}