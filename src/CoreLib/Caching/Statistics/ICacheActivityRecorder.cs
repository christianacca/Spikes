namespace Eca.Commons.Caching.Statistics
{
    /// <summary>
    /// Interface used internally to record when methods on <see cref="ICache"/> have been called
    /// </summary>
    internal interface ICacheActivityRecorder
    {
        void ContainsCalled();
        void FlushCalled();
        void ItemRetrieved(string key);
        void ItemAdded(string key);
    }
}