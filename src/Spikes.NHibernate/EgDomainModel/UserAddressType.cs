/*
 * Created by: 
 * Created: 19 November 2006
 */

using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace Eca.Spikes.NHibernate
{
    public class UserAddressType : IUserType
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


        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var line1 = (string) NHibernateUtil.String.NullSafeGet(rs, names[0]);
            var line2 = (string) NHibernateUtil.String.NullSafeGet(rs, names[1]);
            var town = (string) NHibernateUtil.String.NullSafeGet(rs, names[2]);
            var county = (string) NHibernateUtil.String.NullSafeGet(rs, names[3]);
            var postCode = (string) NHibernateUtil.String.NullSafeGet(rs, names[4]);

            return
                (line1 == string.Empty && line2 == string.Empty && town == string.Empty && county == string.Empty &&
                 postCode == string.Empty)
                    ? UserAddress.NullObject
                    : new UserAddress(line1, line2, town, county, postCode);
        }


        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            UserAddress a = (value == null) ? UserAddress.NullObject : (UserAddress) value;

            NHibernateUtil.String.NullSafeSet(cmd, a.Line1, index);
            NHibernateUtil.String.NullSafeSet(cmd, a.Line2, index + 1);
            NHibernateUtil.String.NullSafeSet(cmd, a.Town, index + 2);
            NHibernateUtil.String.NullSafeSet(cmd, a.County, index + 3);
            NHibernateUtil.String.NullSafeSet(cmd, a.PostCode, index + 4);
        }


        public object Replace(object original, object target, object owner)
        {
            return DeepCopy(original);
        }


        public Type ReturnedType
        {
            get { return typeof (UserAddress); }
        }

        public SqlType[] SqlTypes
        {
            get { return sqlTypes; }
        }

        #endregion


        #region Class Members

        private static readonly SqlType[] sqlTypes
            = {
                  SqlTypeFactory.GetString(50),
                  SqlTypeFactory.GetString(50),
                  SqlTypeFactory.GetString(50),
                  SqlTypeFactory.GetString(50),
                  SqlTypeFactory.GetString(50)
              };

        #endregion
    }
}