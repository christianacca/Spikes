using System;
using System.Collections.Generic;
using Eca.Commons.Extensions;

namespace Eca.Commons.Caching.Statistics
{
    public class CacheStatistics : ICacheActivityRecorder
    {
        #region Member Variables

        private readonly IDictionary<string, CacheStatistic> _statistics = new Dictionary<string, CacheStatistic>();

        #endregion


        #region Constructors

        public CacheStatistics(IEnumerable<CacheStatistic> statistics)
        {
            statistics.ForEach(statistic => _statistics.Add(statistic.Name, statistic));
        }

        #endregion


        #region ICacheActivityRecorder Members

        void ICacheActivityRecorder.ContainsCalled()
        {
            Record(s => s.ContainsCalled());
        }


        void ICacheActivityRecorder.FlushCalled()
        {
            Record(s => s.FlushCalled());
        }


        void ICacheActivityRecorder.ItemAdded(string key)
        {
            Record(s => s.ItemAdded(key));
        }


        void ICacheActivityRecorder.ItemRetrieved(string key)
        {
            Record(s => s.ItemRetrieved(key));
        }

        #endregion


        public T GetValue<T>(string statiticsName)
        {
            return (T) _statistics[statiticsName].CurrentValue;
        }


        public bool HasStatictic(string statisticName)
        {
            return _statistics.ContainsKey(statisticName);
        }


        private void Record(Action<CacheStatistic> methodCalled)
        {
            _statistics.Values.ForEach(methodCalled);
        }


        public T SafeGetValue<T>(string statiticsName)
        {
            if (!_statistics.ContainsKey(statiticsName)) return default(T);

            return (T) _statistics[statiticsName].CurrentValue;
        }


        #region Class Members

        static CacheStatistics()
        {
            Empty = new CacheStatistics(new List<CacheStatistic>());
        }


        public static CacheStatistics Empty { get; private set; }

        #endregion
    }



    public class CacheStatisticsKeys
    {
        #region Member Variables

        public const string LastItemAdd = "LastItemAddStatistic";
        public const string LastItemRetrieve = "LastItemRetrieveStatistic";
        public const string LastUse = "LastUseStatistic";

        #endregion
    }
}