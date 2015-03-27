using System;
using System.ComponentModel.DataAnnotations;

namespace Spikes.EntityFramework.Models.Bidirectional.ExternalDb
{
    public class FileContent
    {
        public FileContent()
        {
            CreatedDate = DateTimeOffset.Now;
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public ImageSize? ImageSize { get; set; }
        public int MediaGroupId { get; set; }
        [Required]
        public MediaGroup MediaGroup { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        [Required]
        public string FileOwnerType { get; set; }
    }
}