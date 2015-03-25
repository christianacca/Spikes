namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    using System.Collections.Generic;

    public class FileHeader
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public ICollection<FileContent> Files { get; set; }
    }
}