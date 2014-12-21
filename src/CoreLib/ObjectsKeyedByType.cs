using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;
using MiscUtil.Collections.Extensions;
using NValidate.Framework;

namespace Eca.Commons
{
    public class ObjectsKeyedByType<TElement>
    {
        #region Member Variables

        private readonly IDictionary<Type, List<TElement>> _store = new Dictionary<Type, List<TElement>>();

        #endregion


        #region Constructors

        public ObjectsKeyedByType() {}


        public ObjectsKeyedByType(IEnumerable<KeyValuePair<Type, IEnumerable<TElement>>> objects)
        {
            _store = objects.ToDictionary(pair => pair.Key, x => x.Value.ToList());
        }

        #endregion


        public virtual void Add<TKey>(TElement element)
        {
            Check.Require(() => Demand.The.Param(() => element).IsNotOneOf(SelectMany()));

            ICollection<TElement> objects = _store.GetOrCreate(typeof (TKey),
                                                               new List<TElement>());
            objects.Add(element);
        }


        public virtual void Add<TKey>(IEnumerable<TElement> elements)
        {
            elements.ForEach(Add<TKey>);
        }


        public IEnumerable<TElement> Get<TKey>()
        {
            if (!_store.ContainsKey(typeof (TKey))) return Enumerable.Empty<TElement>();

            return _store[typeof (TKey)];
        }


        public IEnumerable<KeyValuePair<Type, IEnumerable<TElement>>> GetAll()
        {
            return _store.Select(pair => new KeyValuePair<Type, IEnumerable<TElement>>(pair.Key, pair.Value));
        }


        /// <summary>
        /// Removes the <paramref name="element"/> supplied, returning true if the element was found
        /// </summary>
        public virtual bool Remove<TKey>(TElement element)
        {
            if (Get<TKey>() == null) return false;

            List<TElement> elements = _store[typeof (TKey)];
            return elements.Remove(element);
        }


        public IEnumerable<TElement> SelectMany()
        {
            return _store.SelectMany(x => x.Value);
        }
    }
}