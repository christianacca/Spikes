using System;

namespace Eca.Commons.DomainLayer
{
    public abstract class EntityBase<T> : IEntity<T>
    {
        #region IEntity<T> Members

        public virtual void DisableValidation()
        {
            //no-op
        }


        public virtual void EnableValidation()
        {
            //no-op
        }


        public abstract T Id { get; }


        public virtual bool IsNew
        {
            get
            {
                if (ReferenceEquals(Id, null)) return true;

                return Id.Equals(default(T));
            }
        }

        public virtual bool IsNull
        {
            get { return false; }
        }

        public bool IsValidationEnabled
        {
            get { return true; }
        }

        #endregion
    }
}