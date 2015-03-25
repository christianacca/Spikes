namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class EntityFileHeader
    {
        public int Id { get; set; }

        public FileHeader FileHeader { get; set; }

        public int FileHeaderId { get; set; }
    }
}