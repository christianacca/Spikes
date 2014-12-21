using System;
using System.ComponentModel.DataAnnotations;

namespace Eca.Commons.DomainLayer
{
    [Serializable]
    public abstract class AuditableObjectBase : IAuditable
    {
        #region Member Variables

        private string _createdBy = string.Empty;
        private DateTime? _createdOn;
        private string _updatedBy = string.Empty;
        private DateTime? _updatedOn;

        #endregion


        #region IAuditable Members

        [StringLength(256)]
        public virtual string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public virtual DateTime? CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }


        public virtual void SetCreatedBy(string value)
        {
            _createdBy = value;
        }


        public virtual void SetCreatedOn(DateTime value)
        {
            _createdOn = value;
        }


        public virtual void SetUpdatedBy(string value)
        {
            _updatedBy = value;
        }


        public virtual void SetUpdatedOn(DateTime value)
        {
            _updatedOn = value;
        }

        [StringLength(256)]
        public virtual string UpdatedBy
        {
            get { return _updatedBy; }
            set { _updatedBy = value; }
        }

        public virtual DateTime? UpdatedOn
        {
            get { return _updatedOn; }
            set { _updatedOn = value; }
        }

        #endregion
    }
}