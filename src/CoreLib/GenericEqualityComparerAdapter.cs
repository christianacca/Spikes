using System;
using System.Collections;
using System.Collections.Generic;

namespace Eca.Commons
{
    [SkipFormatting]
    public class GenericEqualityComparerAdapter<T> : IEqualityComparer<T>
    {
        private readonly IEqualityComparer _implementation;


        public GenericEqualityComparerAdapter(IEqualityComparer implementation)
        {
            _implementation = implementation;
        }


        public bool Equals(T x, T y)
        {
            return _implementation.Equals(x, y);
        }


        public int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }
    }
}