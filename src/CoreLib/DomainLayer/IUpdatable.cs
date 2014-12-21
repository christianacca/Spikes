using System;

namespace Eca.Commons.DomainLayer
{
    public interface IUpdatable
    {
        string UpdatedBy { get; }
        DateTime? UpdatedOn { get; }
        void SetUpdatedBy(string value);
        void SetUpdatedOn(DateTime value);
    }
}