namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class OrderFileHeader : FileHeader
    {
        public Order Order { get; set; }
        public int OrderId { get; set; }
    }
}