using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Reflection;

namespace Eca.Commons
{
    public class SimpleFactory : IFactory
    {
        #region Member Variables

        private readonly Dictionary<Type, ICollection<Type>> _implementors = new Dictionary<Type, ICollection<Type>>();

        #endregion


        #region IFactory Members

        public virtual TRequested New<TRequested>()
        {
            return (TRequested) New(typeof (TRequested));
        }


        public virtual object New(Type requestedType)
        {
            Type implementor = GetImplementingTypesFor(requestedType).First();
            return CreateInstance<object>(implementor);
        }


        public virtual TRequested New<TRequested>(object constructorArgsAsAnnonymousType)
        {
            throw new NotImplementedException(
                "Need to use reflection to grab all the public properties values of constructorArgsAsAnnonymousType and create an array of object values to pass to the constructor");
        }


        public virtual IEnumerable<TRequested> NewAll<TRequested>()
        {
            return NewAll(typeof (TRequested)).Cast<TRequested>();
        }


        public virtual IEnumerable<object> NewAll(Type requestedType)
        {
            var result = GetImplementingTypesFor(requestedType)
                .Select(implementor => CreateInstance<object>(implementor))
                .ToList();
            return result;
        }


        public void Release(object instance)
        {
            //nothing to do
        }

        #endregion


        private TRequested CreateInstance<TRequested>(Type implementor)
        {
            var requestedInstance = (TRequested) ReflectionUtil.CreateInstance(implementor);
            return requestedInstance;
        }


        private IEnumerable<Type> GetImplementingTypesFor(Type requestedType)
        {
            ICollection<Type> implementor;
            _implementors.TryGetValue(requestedType, out implementor);
            if (implementor == null)
            {
                throw new ArgumentException("the type requested to be created is not implemented");
            }
            return implementor;
        }


        public virtual bool IsRegistered<TService>()
        {
            return _implementors.ContainsKey(typeof (TService));
        }


        public virtual void Register<TService, TImplementor>()
        {
            var serviceType = typeof (TService);
            if (!_implementors.ContainsKey(serviceType))
            {
                _implementors.Add(serviceType, new List<Type>());
            }
            _implementors[serviceType].Add(typeof (TImplementor));
        }
    }
}