using System;
using System.ComponentModel.DataAnnotations;
using Spikes.Migrations.BaseModel;

namespace Spike.Migrations.Model
{
    public partial class Asset
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Reference { get; set; }

        public virtual LookupItem AssetType { get; set; }
        public int? AssetTypeId { get; set; }

        public virtual UserRole RequiredUserRole { get; set; }
        public int? RequiredUserRoleId { get; set; }

        public DateTimeOffset Created { get; set; }

/*
        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; }
*/
    }
}