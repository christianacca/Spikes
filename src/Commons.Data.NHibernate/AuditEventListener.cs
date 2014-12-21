using System;
using Eca.Commons.Dates;
using Eca.Commons.DomainLayer;
using Eca.Commons.Extensions;
using NHibernate.Event;

namespace Eca.Commons.Data.NHibernate
{
    public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        #region Member Variables

        private const string PropertyNameCreatedBy = "CreatedBy";
        private const string PropertyNameCreatedOn = "CreatedOn";
        private const string PropertyNameUpdatedBy = "UpdatedBy";
        private const string PropertyNameUpdatedOn = "UpdatedOn";

        #endregion


        #region IPreInsertEventListener Members

        public bool OnPreInsert(PreInsertEvent e)
        {
            DateTime now = Clock.Now;
            string userName = User.Current.Username;
            if (typeof (ICreatable).IsAssignableFrom(e.Entity.GetType()))
                SetCreateAuditProperties(e.State, e.Persister.PropertyNames, (ICreatable) e.Entity, now, userName);
            if (typeof (IUpdatable).IsAssignableFrom(e.Entity.GetType()))
                SetUpdateAuditProperties(e.State, e.Persister.PropertyNames, (IUpdatable) e.Entity, now, userName);
            return false;
        }

        #endregion


        #region IPreUpdateEventListener Members

        public bool OnPreUpdate(PreUpdateEvent e)
        {
            DateTime now = Clock.Now;
            string userName = User.Current.Username;

            // fill in wholes in created on/by 
            // most likely when using a join mapping and the CreatedOn/By properties are mapped to the join table
            if (typeof (ICreatable).IsAssignableFrom(e.Entity.GetType()))
            {
                var listener = new DiscretionaryAuditEventListener();
                listener.SetCreateAuditProperties(e.State,
                                                  e.Persister.PropertyNames,
                                                  (ICreatable) e.Entity,
                                                  now,
                                                  userName);
            }

            if (typeof (IUpdatable).IsAssignableFrom(e.Entity.GetType()))
                SetUpdateAuditProperties(e.State,
                                         e.Persister.PropertyNames,
                                         (IUpdatable) e.Entity,
                                         now,
                                         userName);
            return false;
        }

        #endregion


        private void SetCreateAuditProperties(object[] state,
                                              string[] propertyNames,
                                              ICreatable entity,
                                              DateTime now,
                                              string userName)
        {
            var index = Array.FindIndex(propertyNames, n => n == PropertyNameCreatedBy);
            SetCreatedBy(state, index, userName, entity);

            index = Array.FindIndex(propertyNames, n => n == PropertyNameCreatedOn);
            SetCreatedOn(state, index, now, entity);
        }


        protected virtual void SetCreatedBy(object[] state, int index, string value, ICreatable entity)
        {
            SetPropertyBag(state, index, value, PropertyNameCreatedBy, entity);
            entity.SetCreatedBy(value);
        }


        protected virtual void SetCreatedOn(object[] state, int index, DateTime value, ICreatable entity)
        {
            SetPropertyBag(state, index, value, PropertyNameCreatedOn, entity);
            entity.SetCreatedOn(value);
        }


        private void SetPropertyBag<EntityT, PropertyT>(object[] state,
                                                        int index,
                                                        PropertyT propertyValue,
                                                        string propertyName,
                                                        EntityT entity)
        {
            try
            {
                state[index] = propertyValue;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(e.Message +
                                                   string.Format("{0}Make sure you have mapped a {1} property for {2}",
                                                                 Environment.NewLine,
                                                                 propertyName,
                                                                 entity.GetType().FullName));
            }
        }


        private void SetUpdateAuditProperties(object[] state,
                                              string[] propertyNames,
                                              IUpdatable entity,
                                              DateTime now,
                                              string userName)
        {
            var index = Array.FindIndex(propertyNames, n => n == PropertyNameUpdatedBy);
            SetUpdatedBy(state, index, userName, entity);

            index = Array.FindIndex(propertyNames, n => n == PropertyNameUpdatedOn);
            SetUpdatedOn(state, index, now, entity);
        }


        protected virtual void SetUpdatedBy(object[] state, int index, string value, IUpdatable entity)
        {
            SetPropertyBag(state, index, value, PropertyNameUpdatedBy, entity);
            entity.SetUpdatedBy(value);
        }


        protected virtual void SetUpdatedOn(object[] state, int index, DateTime value, IUpdatable entity)
        {
            SetPropertyBag(state, index, value, PropertyNameUpdatedOn, entity);
            entity.SetUpdatedOn(value);
        }
    }



    public class DiscretionaryAuditEventListener : AuditEventListener
    {
        protected override void SetCreatedBy(object[] state, int index, string value, ICreatable entity)
        {
            if (String.IsNullOrEmpty(entity.CreatedBy))
            {
                base.SetCreatedBy(state, index, value, entity);
            }
        }


        protected override void SetCreatedOn(object[] state, int index, DateTime value, ICreatable entity)
        {
            if (entity.CreatedOn.IsMissing())
            {
                base.SetCreatedOn(state, index, value, entity);
            }
        }
    }
}