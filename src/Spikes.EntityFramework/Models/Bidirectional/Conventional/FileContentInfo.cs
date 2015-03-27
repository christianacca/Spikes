namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class FileContentInfo
    {
        public FileHeader FileHeader { get; set; }
        public int FilterHeaderId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; }
    }
}