using System;
using Eca.Commons;

namespace Eca.Spikes.NHibernate
{
    public abstract class EntityBase
    {
        #region Member Variables

        private const int UnsavedConcurrencyId = -1;


        protected int _concurrencyId = UnsavedConcurrencyId;
        private readonly Guid _id = GuidGenerator.Generate();

        #endregion


        #region Properties

        public virtual int ConcurrencyId
        {
            get { return _concurrencyId; }
        }

        public virtual Guid Id
        {
            get { return _id; }
        }

        public virtual bool IsNew
        {
            get { return _concurrencyId == UnsavedConcurrencyId; }
        }

        #endregion
    }
}