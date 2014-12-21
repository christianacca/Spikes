using System;
using System.Collections.Generic;

namespace Eca.Commons.Caching.Statistics
{
    public class StatisticsCache : CacheDecorator
    {
        #region Member Variables

        private readonly ICacheActivityRecorder _statistics;

        #endregion


        #region Constructors

        public StatisticsCache(IEnumerable<CacheStatistic> statistics, ICache cache) : base(cache)
        {
            _statistics = new CacheStatistics(statistics);
        }

        #endregion


        #region Properties

        public override CacheStatistics Statistics
        {
            get { return (CacheStatistics) _statistics; }
        }

        #endregion


        public override void Add(string key, object value)
        {
            _statistics.ItemAdded(key);
            base.Add(key, value);
        }


        public override bool Contains(string key)
        {
            _statistics.ContainsCalled();
            return base.Contains(key);
        }


        public override void Flush()
        {
            _statistics.FlushCalled();
            base.Flush();
        }


        public override T GetData<T>(string key)
        {
            _statistics.ItemRetrieved(key);
            return base.GetData<T>(key);
        }


        public override T GetOrAdd<T>(string key, Func<T> constructor)
        {
            if (!DecoratedCache.Contains(key)) _statistics.ItemAdded(key);
            _statistics.ItemRetrieved(key);
            return base.GetOrAdd(key, constructor);
        }
    }
}