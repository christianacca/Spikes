using System;
using System.Data;
using Eca.Commons.Dates;
using NHibernate;
using NHibernate.SqlTypes;

namespace Eca.Commons.Data.NHibernate.UserTypes
{
    public class DateUserType : ImmutableUserType
    {
        #region Properties

        public override Type ReturnedType
        {
            get { return typeof (Date); }
        }

        public override SqlType[] SqlTypes
        {
            get { return sqlTypes; }
        }

        #endregion


        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var valueFromDb = (DateTime?) NHibernateUtil.Date.NullSafeGet(rs, names[0]);
            return Date.From(valueFromDb);
        }


        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value != null)
                NHibernateUtil.Date.NullSafeSet(cmd, ((Date) value).ToDateTime(), index);
            else
                NHibernateUtil.Date.NullSafeSet(cmd, null, index);
        }


        #region Class Members

        private static readonly SqlType[] sqlTypes
            = {
                  SqlTypeFactory.Date
              };

        #endregion
    }
}