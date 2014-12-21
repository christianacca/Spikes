namespace Eca.Commons.Caching
{
    /// <summary>
    /// Never actually caches items! Useful when you have a consumer expects to be handed an <see
    /// cref="IStoppableCache"/> instance but the context dictates that caching should not actually occur
    /// </summary>
    public class NullStoppableCache : CacheDecorator, IStoppableCache, INullMarker
    {
        #region Constructors

        private NullStoppableCache() : base(NullCache.Instance) {}

        #endregion


        #region IStoppableCache Members

        public bool IsEnabled
        {
            get { return false; }
            set
            {
                //no-op; }
            }
        }

        #endregion


        #region Class Members

        static NullStoppableCache()
        {
            Instance = new NullStoppableCache();
        }


        public static IStoppableCache Instance { get; set; }

        #endregion
    }
}