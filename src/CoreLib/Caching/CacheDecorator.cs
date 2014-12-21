using System;
using Eca.Commons.Caching.Statistics;

namespace Eca.Commons.Caching
{
    /// <summary>
    /// Base class for creating concreate decorator classes that will add (decorate) additional caching
    /// behaviours to implementations of <see cref="ICache"/>
    /// </summary>
    public abstract class CacheDecorator : ICache
    {
        #region Constructors

        protected CacheDecorator(ICache decoratedCache)
        {
            DecoratedCache = decoratedCache;
        }

        #endregion


        #region Properties

        protected ICache DecoratedCache { get; private set; }

        #endregion


        #region ICache Members

        public virtual void Add(string key, object value)
        {
            DecoratedCache.Add(key, value);
        }


        public virtual string CacheName
        {
            get { return DecoratedCache.CacheName; }
        }


        public virtual bool Contains(string key)
        {
            return DecoratedCache.Contains(key);
        }


        public virtual int Count
        {
            get { return DecoratedCache.Count; }
        }


        public virtual void Flush()
        {
            DecoratedCache.Flush();
        }


        public virtual T GetData<T>(string key)
        {
            return DecoratedCache.GetData<T>(key);
        }


        public virtual T GetOrAdd<T>(string key, Func<T> constructor)
        {
            return DecoratedCache.GetOrAdd(key, constructor);
        }


        public string InstanceName
        {
            get { return DecoratedCache.InstanceName; }
            set { DecoratedCache.InstanceName = value; }
        }


        public virtual void Remove(string key)
        {
            DecoratedCache.Remove(key);
        }


        public virtual CacheStatistics Statistics
        {
            get { return DecoratedCache.Statistics; }
        }

        #endregion
    }
}