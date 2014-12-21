using System;

namespace Eca.Commons.CheckDigit
{
    /// <summary>
    /// For more information cf. http://en.wikipedia.org/wiki/Verhoeff_algorithm/ 
    /// Dihedral Group stuff: http://en.wikipedia.org/wiki/Dihedral_group
    /// Dihedral Group order 10: http://mathworld.wolfram.com/DihedralGroupD5.html
    /// </summary>
    /// <remarks>
    /// Source for this code: http://en.wikibooks.org/wiki/Algorithm_Implementation/Checksums/Verhoeff_Algorithm#C.23
    /// <para>
    /// Modifications:
    /// <list type="bullet">
    /// <item>Changed class name and implemented interface</item>
    /// <item>Reformatted and renamed code to house standards</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class VerhoeffCheckDigit : ICheckDigit
    {
        #region ICheckDigit Members

        /// <summary>
        /// For a given number generates a Verhoeff digit
        /// Append this check digit to num
        /// </summary>
        public string CalculateDigit(string value)
        {
            int c = 0;
            int[] myArray = StringToReversedIntArray(value);

            for (int i = 0; i < myArray.Length; i++)
            {
                c = d[c, p[((i + 1)%8), myArray[i]]];
            }

            return inv[c].ToString();
        }


        /// <summary>
        /// Validates that an entered number is Verhoeff compliant.
        /// NB: Make sure the check digit is the last one!
        /// </summary>
        public bool IsValid(string value)
        {
            int c = 0;
            int[] myArray = StringToReversedIntArray(value);

            for (int i = 0; i < myArray.Length; i++)
            {
                c = d[c, p[(i%8), myArray[i]]];
            }

            return c == 0;
        }

        #endregion


        #region Class Members

        /// <summary>
        /// The multiplication table
        /// </summary>
        private static readonly int[,] d = new[,]
                                               {
                                                   {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                                                   {1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
                                                   {2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
                                                   {3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
                                                   {4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
                                                   {5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
                                                   {6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
                                                   {7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
                                                   {8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
                                                   {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
                                               };

        /// <summary>
        /// The inverse table
        /// </summary>
        private static readonly int[] inv = {0, 4, 3, 2, 1, 5, 6, 7, 8, 9};

        /// <summary>
        /// The permutation table
        /// </summary>
        private static readonly int[,] p = new[,]
                                               {
                                                   {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                                                   {1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
                                                   {5, 8, 0, 3, 7, 9, 6, 1, 4, 2},
                                                   {8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
                                                   {9, 4, 5, 3, 1, 2, 6, 8, 7, 0},
                                                   {4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
                                                   {2, 7, 9, 3, 8, 0, 6, 4, 1, 5},
                                                   {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
                                               };


        /// <summary>
        /// Converts a string to a reversed integer array.
        /// </summary>
        /// <param name="num"></param>
        /// <returns>Reversed integer array</returns>
        private static int[] StringToReversedIntArray(string num)
        {
            var myArray = new int[num.Length];

            for (int i = 0; i < num.Length; i++)
            {
                myArray[i] = int.Parse(num.Substring(i, 1));
            }

            Array.Reverse(myArray);

            return myArray;
        }

        #endregion
    }
}