using System.ComponentModel.DataAnnotations;

namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class CustomerFileHeader : FileHeader
    {
        [Required]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        public LookupItem CustomerFileType { get; set; }
        public int? CustomerFileTypeId { get; set; }
    }
}