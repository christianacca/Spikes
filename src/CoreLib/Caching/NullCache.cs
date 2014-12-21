using System;
using System.Linq;
using Eca.Commons.Caching.Statistics;

namespace Eca.Commons.Caching
{
    /// <summary>
    /// Never actually caches items! Useful when you have a consumer expects to be handed an <see cref="ICache"/>
    /// instance but the context dictates that caching should not actually occur
    /// </summary>
    public class NullCache : ICache, INullMarker
    {
        #region Member Variables

        private readonly CacheStatistics _emptyCacheStatistics = new CacheStatistics(Enumerable.Empty<CacheStatistic>());

        #endregion


        #region Constructors

        private NullCache() {}

        #endregion


        #region ICache Members

        public void Add(string key, object value)
        {
            //no-op
        }


        public string CacheName
        {
            get { return "NullCache"; }
        }


        public bool Contains(string key)
        {
            return false;
        }


        public int Count
        {
            get { return 0; }
        }


        public void Flush()
        {
            //no-op
        }


        public T GetData<T>(string key)
        {
            return default(T);
        }


        public T GetOrAdd<T>(string key, Func<T> constructor)
        {
            return constructor();
        }


        public string InstanceName
        {
            get { return CacheName; }
            set { throw new NotSupportedException("NullCache cannot be renamed"); }
        }


        public void Remove(string key)
        {
            //no-op
        }


        public CacheStatistics Statistics
        {
            get { return _emptyCacheStatistics; }
        }

        #endregion


        #region Class Members

        static NullCache()
        {
            Instance = new NullCache();
        }


        public static ICache Instance { get; private set; }

        #endregion
    }
}