using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.DomainLayer;
using Eca.Commons.Extensions;
using MiscUtil.Collections.Extensions;

namespace Eca.Commons.Data
{
    /// <summary>
    /// Represents a dictionary of "entity id"-"entity instance", key-value pairs. Typically used as an in-memory cache
    /// that ensures that only one instance of an Entity is being maintained in memory
    /// </summary>
    public class IdentityMap
    {
        #region Member Variables

        private readonly IDictionary<Type, IDictionary<object, object>> _entityCaches =
            new Dictionary<Type, IDictionary<object, object>>();

        #endregion


        public bool ContainsKey<TEntity, TId>(TId id)
        {
            return GetEntityCache<TEntity>().ContainsKey(id);
        }


        private IDictionary<object, object> GetEntityCache<TEntity>()
        {
            return _entityCaches.GetOrCreate(typeof (TEntity), () => new Dictionary<object, object>());
        }


        /// <summary>
        /// Executes <paramref name="createMany"/> and for each instance thus returned by <paramref name="createMany"/>,
        /// either adds the instance to the map, or substitues the instace thus created with the instance already in the
        /// map
        /// </summary>
        public ICollection<TEntity> GetOrCreateMany<TEntity, TId>(Func<IEnumerable<TEntity>> createMany)
            where TEntity : class, IExposesId<TId>
        {
            IDictionary<object, object> entityCache = GetEntityCache<TEntity>();
            return createMany()
                .SkipNulls()
                .Select(entity => entityCache.GetOrCreate(entity.Id, entity))
                .Cast<TEntity>()
                .ToList();
        }


        /// <summary>
        /// Return an existing instance already added to the map using <paramref name="id"/> as the key, otherwise
        /// execute <paramref name="createOne"/> to create an instance of <typeparamref name="TEntity"/>, add it to the
        /// map using its id as key and return this instance just created
        /// </summary>
        /// <remarks>
        /// If <paramref name="createOne"/> returns null, this will not be added to the map
        /// </remarks>
        public virtual TEntity GetOrCreateOne<TEntity, TId>(TId id, Func<TId, TEntity> createOne)
            where TEntity : class, IExposesId<TId>
        {
            IDictionary<object, object> cache = GetEntityCache<TEntity>();
            var entity = (TEntity) cache.GetOrCreate(id, createOne(id));

            //make sure to remove nulls createOne has just added
            if (ReferenceEquals(entity, null))
            {
                cache.Remove(id);
            }
            return entity;
        }
    }
}