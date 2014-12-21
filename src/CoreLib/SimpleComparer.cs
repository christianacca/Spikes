using System;
using System.Collections;

namespace Eca.Commons
{
    /// <summary>
    /// Compares two objects for equality using <see cref="Object.Equals(object, object)"/>.
    /// However, unlike <see cref="Object.Equals(object, object)"/>, will return true if both objects 
    /// supplied are <see langword="null" />
    /// </summary>
    public class SimpleComparer : IEqualityComparer
    {
        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null) return true;

            return Object.Equals(x, y);
        }


        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}