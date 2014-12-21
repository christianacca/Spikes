using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Caching.Statistics;
using MiscUtil.Collections.Extensions;

namespace Eca.Commons.Caching
{
    /// <summary>
    /// A cache for those situations where all you need is an inmemory cache and you are happy to control when
    /// items are expired by explicitly calling <see cref="Flush"/>
    /// </summary>
    /// <remarks>
    /// See also EntLibCache in Commons.EntLibCaching for more feature rich cache implementation
    /// </remarks>
    public class SimpleInmemoryCache : ICache
    {
        #region Member Variables

        private readonly CacheStatistics _emptyCacheStatistics = new CacheStatistics(Enumerable.Empty<CacheStatistic>());
        private readonly IDictionary<string, object> _inmemoryCache = new Dictionary<string, object>();

        private string _instanceName;

        #endregion


        #region ICache Members

        public void Add(string key, object value)
        {
            //It really makes little sense to cache nothing
            if (ReferenceEquals(null, value)) return;

            _inmemoryCache.Add(key, value);
        }


        public string CacheName
        {
            get { return "SimpleInmemoryCache"; }
        }


        public bool Contains(string key)
        {
            return _inmemoryCache.ContainsKey(key);
        }


        public int Count
        {
            get { return _inmemoryCache.Count; }
        }


        public void Flush()
        {
            _inmemoryCache.Clear();
        }


        public T GetData<T>(string key)
        {
            return (T) _inmemoryCache[key];
        }


        public T GetOrAdd<T>(string key, Func<T> constructor)
        {
            return (T) _inmemoryCache.GetOrCreate(key, () => constructor());
        }


        public string InstanceName
        {
            get { return _instanceName ?? CacheName; }
            set { _instanceName = value; }
        }


        public void Remove(string key)
        {
            _inmemoryCache.Remove(key);
        }


        public CacheStatistics Statistics
        {
            get { return _emptyCacheStatistics; }
        }

        #endregion


        #region Class Members

        public static IStoppableCache CreateStoppableCache()
        {
            return new StoppableCache(new SimpleInmemoryCache());
        }

        #endregion
    }
}