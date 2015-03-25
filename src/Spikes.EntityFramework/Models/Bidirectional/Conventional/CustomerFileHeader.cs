namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class CustomerFileHeader : EntityFileHeader
    {
        public Customer Customer { get; set; }

        public int CustomerId { get; set; }
    }
}