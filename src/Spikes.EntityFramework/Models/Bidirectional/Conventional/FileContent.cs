namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class FileContent
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        public int FileHeaderId { get; set; }

        public FileHeader FileHeader { get; set; }
    }
}