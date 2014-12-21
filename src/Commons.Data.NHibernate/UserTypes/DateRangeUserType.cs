using System;
using System.Collections.Generic;
using System.Data;
using Eca.Commons.Dates;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;

namespace Eca.Commons.Data.NHibernate.UserTypes
{
    public class DateRangeUserType : ImmutableCompositeUserType
    {
        #region Properties

        public override string[] PropertyNames
        {
            get { return new[] {"Start", "End"}; }
        }

        public override IType[] PropertyTypes
        {
            get
            {
                return new[]
                           {
                               new CustomType(typeof (DateUserType), new Dictionary<string, string>()),
                               new CustomType(typeof (DateUserType), new Dictionary<string, string>())
                           };
            }
        }

        public override Type ReturnedClass
        {
            get { return typeof (DateRange); }
        }

        #endregion


        public override object GetPropertyValue(object component, int property)
        {
            var range = (DateRange) component;
            if (property == 0)
                return range.Start;
            else
                return range.End;
        }


        public override object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            var start = (DateTime?) NHibernateUtil.Date.NullSafeGet(dr, names[0]);
            var end = (DateTime?) NHibernateUtil.Date.NullSafeGet(dr, names[1]);

            return DateRange.From(start, end);
        }


        public override void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.Date.NullSafeSet(cmd, null, index);
                NHibernateUtil.Date.NullSafeSet(cmd, null, index + 1);
            }
            else
            {
                var range = (DateRange) value;

                DateTime? start = range.Start;
                DateTime? end = range.End;
                NHibernateUtil.Date.NullSafeSet(cmd, start, index);
                NHibernateUtil.Date.NullSafeSet(cmd, end, index + 1);
            }
        }
    }
}