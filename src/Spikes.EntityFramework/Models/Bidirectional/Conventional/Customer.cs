using System.Collections.Generic;

namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class Customer
    {
        public Customer()
        {
            Files = new List<CustomerFileHeader>();
        }

        public string Name { get; set; }
        public int Id { get; set; }

        public ICollection<CustomerFileHeader> Files { get; set; }
    }
}