using System.Collections.Generic;

namespace Change.This.Now.Extensions
{
    public static class TwoDimensionalArrayExtensions
    {
        #region Class Members

        public static IEnumerable<T> RowValues<T>(this T[,] source, int row)
        {
            for (int column = 0; column <= source.GetUpperBound(1); column++)
            {
                yield return source[row, column];
            }
        }

        #endregion
    }
}