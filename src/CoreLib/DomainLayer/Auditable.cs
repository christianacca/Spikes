using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Dates;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;

namespace Eca.Commons.DomainLayer
{
    public static class Auditable
    {
        #region Member Variables

        public const string NullUserName = "Machine Generated";

        #endregion


        #region Class Members

        public static readonly DateTime NullCreatedOn = new DateTime(1900, 01, 06);


        private static IEnumerable<string> _creatableAuditPropertyNames;
        private static IEnumerable<string> _updatableAuditPropertyNames;


        public static IEnumerable<string> AuditablePropertyNames
        {
            get { return CreatableAuditPropertyNames.Union(UpdatableAuditPropertyNames); }
        }

        public static IEnumerable<string> CreatableAuditPropertyNames
        {
            get
            {
                if (_creatableAuditPropertyNames == null)
                {
                    _creatableAuditPropertyNames = PropertyNames.For<ICreatable>(x => x.CreatedOn,
                                                                                 x => x.CreatedBy);
                }
                return _creatableAuditPropertyNames;
            }
        }

        public static IEnumerable<string> UpdatableAuditPropertyNames
        {
            get
            {
                if (_updatableAuditPropertyNames == null)
                {
                    _updatableAuditPropertyNames = PropertyNames.For<IAuditable>(x => x.UpdatedOn,
                                                                                 x => x.UpdatedBy);
                }
                return _updatableAuditPropertyNames;
            }
        }


        public static void SetAuditingPropertiesNotAlreadySet(IAuditable entity)
        {
            if (entity == null) return;

            if (String.IsNullOrEmpty(entity.CreatedBy)) entity.SetCreatedBy(User.Current.Username);
            if (String.IsNullOrEmpty(entity.UpdatedBy)) entity.SetUpdatedBy(User.Current.Username);

            DateTime now = Clock.Now;
            if (entity.CreatedOn.IsMissing()) entity.SetCreatedOn(now);
            if (entity.UpdatedOn.IsMissing()) entity.SetUpdatedOn(now);
        }


        public static void SetNullAuditingProperties(IAuditable entity)
        {
            entity.SetCreatedBy(NullUserName);
            entity.SetCreatedOn(NullCreatedOn);
            entity.SetUpdatedBy(NullUserName);
            entity.SetUpdatedOn(NullCreatedOn);
        }

        #endregion
    }
}