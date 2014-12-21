using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Eca.Commons.Caching.Statistics;
using NValidate.Framework;

namespace Eca.Commons.Caching
{
    public class CacheAdministator
    {
        #region Member Variables

        private readonly ICollection<ICache> _allCaches = new List<ICache>();

        #endregion


        #region Properties

        public virtual IEnumerable<CacheInfo> AllCacheInfos
        {
            get
            {
                return
                    StoppableCaches.Select(
                        x =>
                        new CacheInfo
                            {
                                CacheName = x.CacheName,
                                InstanceName = x.InstanceName,
                                IsEnabled = x.IsEnabled,
                                ItemCount = x.Count,
                                LastAdd = x.Statistics.SafeGetValue<DateTime>(CacheStatisticsKeys.LastItemAdd),
                                LastRetrieve = x.Statistics.SafeGetValue<DateTime>(CacheStatisticsKeys.LastItemRetrieve),
                                LastUse = x.Statistics.SafeGetValue<DateTime>(CacheStatisticsKeys.LastUse)
                            })
                        .OrderBy(info => info.CacheName)
                        .ThenByDescending(info => info.LastUse);
            }
        }

        public virtual IEnumerable<ICache> AllCaches
        {
            get { return _allCaches; }
        }


        private IEnumerable<IStoppableCache> StoppableCaches
        {
            get { return _allCaches.OfType<IStoppableCache>(); }
        }

        #endregion


        public virtual void ChangeEnableStatusFor(IEnumerable<CacheInfo> cacheInfos)
        {
            Check.Require(() => Demand.The.Param(() => cacheInfos).IsNotNull());

            var cachesToUpdate = from info in cacheInfos
                                 join cache in StoppableCaches on info.InstanceName equals cache.InstanceName
                                 select new {info.IsEnabled, Cache = cache};

            foreach (var item in cachesToUpdate)
            {
                item.Cache.IsEnabled = item.IsEnabled;
            }
        }


        public virtual bool Contains(ICache cache)
        {
            return _allCaches.Contains(cache);
        }


        public virtual void DisableAllStoppableCaches()
        {
            StoppableCaches.ForEach(cache => cache.IsEnabled = false);
        }


        public virtual void EnableAllStoppableCaches()
        {
            StoppableCaches.ForEach(cache => cache.IsEnabled = false);
        }


        public virtual void FlushAll()
        {
            _allCaches.ForEach(cache => cache.Flush());
        }


        public virtual void Register(ICache cache)
        {
            Check.Require(() => Demand.The.Param(() => cache).IsNotNull());

            //don't add special case null instances
            if (cache is INullMarker) return;

            if (_allCaches.Contains(cache)) return;

            _allCaches.Add(cache);
        }


        public virtual void Unregister(ICache cache)
        {
            if (!_allCaches.Contains(cache)) return;

            _allCaches.Remove(cache);
        }


        #region Class Members

        static CacheAdministator()
        {
            Instance = new CacheAdministator();
        }


        public static CacheAdministator Instance { get; set; }

        #endregion
    }
}