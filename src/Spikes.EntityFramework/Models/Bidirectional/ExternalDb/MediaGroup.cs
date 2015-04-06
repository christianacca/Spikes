namespace Spikes.EntityFramework.Models.Bidirectional.ExternalDb
{
    /// <summary>
    /// Groups together the set of files that were produced by a single
    /// file upload.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typically when an image is uploaded, multiple sizes of that same image
    /// will be generated and saved as seperate <see cref="FileContent"/>
    /// entries. The set of <see cref="FileContent"/> entries thus created will
    /// all share the same <see cref="MediaGroup"/> instance.
    /// </para>
    /// </remarks>
    public class MediaGroup
    {
        public int Id { get; set; }
    }
}