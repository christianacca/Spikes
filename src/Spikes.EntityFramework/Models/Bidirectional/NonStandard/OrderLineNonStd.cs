namespace Spikes.EntityFramework.Models.Bidirectional.NonStandard
{
    public class OrderLineNonStd
    {
        public int Id { get; set; }
        public int Units { get; set; }
        public OrderNonStd Order { get; set; }
        public int PrimaryOrderId { get; set; }
    }
}