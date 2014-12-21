using System;

namespace Eca.Commons.DomainLayer
{
    public interface ICreatable
    {
        string CreatedBy { get; }
        DateTime? CreatedOn { get; }
        void SetCreatedBy(string value);
        void SetCreatedOn(DateTime value);
    }
}