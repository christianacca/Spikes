using System;
using System.ComponentModel.DataAnnotations;

namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public abstract class FileHeader
    {
        protected FileHeader()
        {
            CreatedDate = DateTimeOffset.Now;
            ModifedDate = DateTimeOffset.Now;
        }

        public int Id { get; set; }
        [Range(1, Int32.MaxValue)]
        public int MediaGroupId { get; set; }
        [Required]
        public string Title { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifedDate { get; set; }

        public FileContentInfo ContentInfo { get; protected set; }
    }
}