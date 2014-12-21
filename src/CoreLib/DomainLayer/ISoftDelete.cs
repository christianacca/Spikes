using System;

namespace Eca.Commons.DomainLayer
{
    public interface ISoftDelete
    {
        bool Deleted { get; set; }
        DateTime? DeletedOn { get; set; }
        string DeletedBy { get; set; }
    }
}