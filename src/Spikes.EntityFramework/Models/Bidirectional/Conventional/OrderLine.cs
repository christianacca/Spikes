namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int Units { get; set; }
        /// <summary>
        /// Note: in terms of EF mapping conventions, it doesn't matter what the name of the nav.property is, just
        /// that the corrosponding fk property is named the same plus 'Id' as a suffix
        /// </summary>
        public Order MyOrder { get; set; }
        public int MyOrderId { get; set; }
    }
}