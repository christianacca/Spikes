using System;
using System.ComponentModel.DataAnnotations;

namespace Spikes.Migrations.BaseModel
{
    [ReferenceData]
    public class LookupItem
    {
        [Key]
        public int Key { get; set; }

        [Required]
        [StringLength(100)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public Lookup Lookup { get; set; }
        public int? LookupId { get; set; }

//        public DateTimeOffset Created { get; set; }

//        public int ConcurrencyVs { get; set; }
    }
}