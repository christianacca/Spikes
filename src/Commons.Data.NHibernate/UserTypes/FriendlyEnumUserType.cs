using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;

namespace Eca.Commons.Data.NHibernate.UserTypes
{
    public class FriendlyEnumUserType<T> : ImmutableUserType
    {
        #region Properties

        public override Type ReturnedType
        {
            get { return typeof (T); }
        }

        public override SqlType[] SqlTypes
        {
            get { return sqlTypes; }
        }

        #endregion


        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var valueFromDb = (string) NHibernateUtil.String.NullSafeGet(rs, names[0]);
            if (string.IsNullOrEmpty(valueFromDb)) return default(T);

            string concatenatedEnumName = valueFromDb.Replace(" ", "");
            return EnhancedConvertor.ChangeType(concatenatedEnumName, typeof (T));
        }


        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value != null)
                NHibernateUtil.String.NullSafeSet(cmd, value.ToString().SplitUpperCaseToString(), index);
            else
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
        }


        #region Class Members

        private static readonly SqlType[] sqlTypes
            = {
                  SqlTypeFactory.GetString(255)
              };

        #endregion
    }
}