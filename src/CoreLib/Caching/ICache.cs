using System;
using Eca.Commons.Caching.Statistics;

namespace Eca.Commons.Caching
{
    /// <summary>
    /// Defines the main beahiour of a cache
    /// </summary>
    /// <remarks>
    /// For additional caching functionality that can be added to a cache, see concreate implementations of <see
    /// cref="CacheDecorator"/> such as
    /// <see cref="StatisticsCache"/>
    /// </remarks>
    public interface ICache
    {
        string CacheName { get; }
        int Count { get; }

        /// <summary>
        /// Unless specified, will be the name of the cache as specified by <see cref="CacheName"/>
        /// </summary>
        /// <remarks>
        /// There may be more than one instance that is sharing the same underlying cache, and hence a cache instance
        /// distriminates itself from other caches by its having a separate instance name.
        /// <para>
        /// The most common reason for this is wanting to have multiple cache instances with different policies, all
        /// sharing the same cache store
        /// </para>
        /// </remarks>
        string InstanceName { get; set; }

        CacheStatistics Statistics { get; }
        void Add(string key, object value);
        bool Contains(string key);
        void Flush();


        /// <summary>
        /// Returns the existing object in the cache associated with <paramref name="key"/>. If the 
        /// <paramref name="key"/> does not exist, the <paramref name="constructor"/> delegate will be executed 
        /// to create an instance of the object. This newly created object will be added to the cache and returned.
        /// </summary>
        /// <typeparam name="T">The type of the object that is cached</typeparam>
        /// <param name="key">The unique key that is associated with the object stored in cache</param>
        /// <param name="constructor">A delegate reponsible for creating the missing object</param>
        /// <returns></returns>
        T GetOrAdd<T>(string key, Func<T> constructor);


        /// <summary>
        /// Returns the object in the cache associated with <paramref name="key"/>. If the <paramref name="key"/>
        /// does not exist, the default value for <typeparamref name="T"/> will be returned.
        /// </summary>
        T GetData<T>(string key);


        void Remove(string key);
    }
}