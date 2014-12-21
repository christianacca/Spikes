using System;

namespace Eca.Commons.Caching
{
    public class StoppableCache : CacheDecorator, IStoppableCache
    {
        #region Constructors

        public StoppableCache(ICache cache) : base(cache)
        {
            IsEnabled = true;
        }

        #endregion


        #region IStoppableCache Members

        public override void Add(string key, object value)
        {
            if (!IsEnabled) return;
            base.Add(key, value);
        }


        public override T GetData<T>(string key)
        {
            if (!IsEnabled) return default(T);
            return base.GetData<T>(key);
        }


        public override T GetOrAdd<T>(string key, Func<T> constructor)
        {
            if (!IsEnabled) return constructor();
            return base.GetOrAdd(key, constructor);
        }


        public bool IsEnabled { get; set; }


        public override void Remove(string key)
        {
            if (!IsEnabled) return;
            base.Remove(key);
        }

        #endregion
    }
}