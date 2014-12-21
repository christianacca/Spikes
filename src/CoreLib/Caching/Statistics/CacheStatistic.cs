using System;

namespace Eca.Commons.Caching.Statistics
{
    public abstract class CacheStatistic : ICacheActivityRecorder
    {
        #region Member Variables

        private readonly string _name;

        #endregion


        #region Constructors

        protected CacheStatistic(string name)
        {
            _name = name;
        }

        #endregion


        #region Properties

        public abstract object CurrentValue { get; }

        public virtual string Name
        {
            get { return _name; }
        }

        #endregion


        #region ICacheActivityRecorder Members

        public virtual void ContainsCalled() {}
        public virtual void FlushCalled() {}
        public virtual void ItemAdded(string key) {}
        public virtual void ItemRetrieved(string key) {}

        #endregion
    }



    /// <summary>
    /// Convenience base class 
    /// </summary>
    [SkipFormatting]
    public abstract class AccessTimeCacheStatisticBase : CacheStatistic
    {
        protected DateTime _when;

        protected AccessTimeCacheStatisticBase(string name) : base(name) {}

        public override object CurrentValue
        {
            get { return _when; }
        }
    }



    /// <summary>
    /// Records when was the last time the cache was used to add, retrieve or search for an item in the cache
    /// </summary>
    [SkipFormatting]
    public class LastUseCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastUseCacheStatistic() : base(CacheStatisticsKeys.LastUse) {}


        public override string Name
        {
            get { return CacheStatisticsKeys.LastUse; }
        }


        public override void ContainsCalled()
        {
            _when = DateTime.Now;
        }


        public override void FlushCalled()
        {
            _when = DateTime.Now;
        }


        public override void ItemAdded(string key)
        {
            _when = DateTime.Now;
        }


        public override void ItemRetrieved(string key)
        {
            _when = DateTime.Now;
        }
    }



    /// <summary>
    /// Records when was the last time the cache was used to add an item to the cache
    /// </summary>
    [SkipFormatting]
    public class LastItemAddCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastItemAddCacheStatistic() : base(CacheStatisticsKeys.LastItemAdd) {}

        public override string Name
        {
            get { return CacheStatisticsKeys.LastItemAdd; }
        }


        public override void ItemAdded(string key)
        {
            _when = DateTime.Now;
        }
    }



    /// <summary>
    /// Records when was the last time the cache was used to retrieve an item from the cache
    /// </summary>
    [SkipFormatting]
    public class LastItemRetrieveCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastItemRetrieveCacheStatistic() : base(CacheStatisticsKeys.LastItemRetrieve) {}


        public override string Name
        {
            get { return CacheStatisticsKeys.LastItemRetrieve; }
        }


        public override void ItemRetrieved(string key)
        {
            _when = DateTime.Now;
        }
    }
}