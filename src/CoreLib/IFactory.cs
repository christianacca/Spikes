using System;
using System.Collections.Generic;

namespace Eca.Commons
{
    public interface IFactory
    {
        void Release(object instance);
        TRequested New<TRequested>();
        object New(Type requestedType);
        TRequested New<TRequested>(object constructorArgsAsAnnonymousType);
        IEnumerable<TRequested> NewAll<TRequested>();
        IEnumerable<object> NewAll(Type requestedType);
    }
}