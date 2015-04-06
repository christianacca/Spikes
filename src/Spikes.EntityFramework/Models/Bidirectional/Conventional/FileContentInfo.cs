namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    /// <summary>
    /// Represents an object that "stands in" / is a proxy for the real file
    /// stored in the external database
    /// </summary>
    public class FileContentInfo
    {
        public virtual FileHeader FileHeader { get; set; }
        public int FilterHeaderId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; }
    }
}