using System;
using System.ComponentModel.DataAnnotations;

namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    /// <summary>
    /// The header information for a set of files stored in the external document db
    /// </summary>
    /// <remarks>
    /// A single file upload can result in multiple files being stored in the external
    /// database. These set of files will all share the same <see cref="MediaGroupId"/>
    /// </remarks>
    public abstract class FileHeader
    {
        protected FileHeader()
        {
            CreatedDate = DateTimeOffset.Now;
            ModifedDate = DateTimeOffset.Now;
        }

        public int Id { get; set; }

        /// <summary>
        /// The identifier for the set of files stored in the external document db
        /// </summary>
        [Range(1, Int32.MaxValue)]
        public int MediaGroupId { get; set; }

        [Required]
        public string Title { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifedDate { get; set; }

        /// <remarks>
        /// This associated entity should be treated as readonly
        /// </remarks>>
        public virtual FileContentInfo ContentInfo { get; set; }
    }
}