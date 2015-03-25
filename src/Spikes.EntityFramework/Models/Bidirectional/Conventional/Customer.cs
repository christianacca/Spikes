namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    using System.Collections.Generic;

    public class Customer : EntityBase
    {
        public string Name { get; set; }

        public ICollection<CustomerFileHeader> Files { get; set; }
    }
}