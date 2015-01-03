using System;
using System.ComponentModel.DataAnnotations;

namespace Spikes.Migrations.BaseModel
{
    [ReferenceData]
    public class LookupItem
    {
        public int Id { get; set; }

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