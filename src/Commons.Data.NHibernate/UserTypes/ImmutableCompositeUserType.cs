using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace Eca.Commons.Data.NHibernate.UserTypes
{
    public abstract class ImmutableCompositeUserType : ICompositeUserType
    {
        #region ICompositeUserType Members

        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return cached;
        }


        public object DeepCopy(object value)
        {
            return value;
        }


        public object Disassemble(object value, ISessionImplementor session)
        {
            return value;
        }


        bool ICompositeUserType.Equals(object x, object y)
        {
            return ObjectUtils.Equals(x, y);
        }


        public int GetHashCode(object x)
        {
            return x != null ? x.GetHashCode() : 0;
        }


        public abstract object GetPropertyValue(object component, int property);


        public bool IsMutable
        {
            get { return false; }
        }


        public abstract object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner);


        public abstract void NullSafeSet(IDbCommand cmd,
                                         object value,
                                         int index,
                                         bool[] settable,
                                         ISessionImplementor session);


        public abstract string[] PropertyNames { get; }
        public abstract IType[] PropertyTypes { get; }


        public object Replace(object original, object target, ISessionImplementor session, object owner)
        {
            return original;
        }


        public abstract Type ReturnedClass { get; }


        public void SetPropertyValue(object component, int property, object value)
        {
            string msg = string.Format("{0} is immutable", ReturnedClass.GetType().FullName);
            throw new InvalidOperationException(msg);
        }

        #endregion
    }
}