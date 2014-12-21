using System;
using Eca.Commons;

namespace Change.This.Now.Extensions
{
    public static class Int32Extensions
    {
        #region Class Members

        public static bool Between(this Int32 source, Int32 lower, Int32 upper)
        {
            return source >= lower && source <= upper;
        }


        public static bool Between(this decimal source, decimal lower, decimal upper)
        {
            return source >= lower && source <= upper;
        }


        public static string ToWords(this int source)
        {
            return new NumberToWordsConverter().NumberToWords(source);
        }

        #endregion
    }
}