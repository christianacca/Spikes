using System;

namespace Eca.Commons.DomainLayer
{
    [Serializable]
    public class ValueObjectBase : ICreatable
    {
        #region Member Variables

        private string _createdBy = String.Empty;
        private DateTime? _createdOn;

        #endregion


        #region ICreatable Members

        public virtual string CreatedBy
        {
            get { return _createdBy; }
        }

        public virtual DateTime? CreatedOn
        {
            get { return _createdOn; }
        }


        void ICreatable.SetCreatedBy(string value)
        {
            _createdBy = value;
        }


        void ICreatable.SetCreatedOn(DateTime value)
        {
            _createdOn = value;
        }

        #endregion
    }
}