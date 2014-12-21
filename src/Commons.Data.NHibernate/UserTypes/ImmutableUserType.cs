using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace Eca.Commons.Data.NHibernate.UserTypes
{
    public abstract class ImmutableUserType : IUserType
    {
        #region IUserType Members

        public object Assemble(object cached, object owner)
        {
            return DeepCopy(cached);
        }


        public object DeepCopy(object value)
        {
            return value;
        }


        public object Disassemble(object value)
        {
            return DeepCopy(value);
        }


        bool IUserType.Equals(object x, object y)
        {
            return ObjectUtils.Equals(x, y);
        }


        public int GetHashCode(object x)
        {
            return x != null ? x.GetHashCode() : 0;
        }


        public bool IsMutable
        {
            get { return false; }
        }

        public abstract object NullSafeGet(IDataReader rs, string[] names, object owner);
        public abstract void NullSafeSet(IDbCommand cmd, object value, int index);


        public object Replace(object original, object target, object owner)
        {
            return DeepCopy(original);
        }


        public abstract Type ReturnedType { get; }
        public abstract SqlType[] SqlTypes { get; }

        #endregion
    }
}