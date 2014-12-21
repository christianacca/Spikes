namespace Eca.Commons.Caching
{
    public interface IStoppableCache : ICache
    {
        bool IsEnabled { get; set; }
    }
}