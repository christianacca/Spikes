using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eca.Commons.Reflection;

namespace Eca.Commons.Validation
{
    public class OneToManyObjectToObjectMap<TDest>
    {
        #region Member Variables

        private readonly ICollection<ObjectToObjectMapping> _mappings = new List<ObjectToObjectMapping>();

        #endregion


        private void DoAdd(ObjectToObjectMapping newMapping)
        {
            Check.Require(!_mappings.Contains(newMapping),
                          string.Format("Cannot add duplicate mapping. Attempted: {0}", newMapping));
            _mappings.Add(newMapping);
        }


        /// <summary>
        /// Using the mapping defined between <paramref name="sourceType"/> and <typeparamref name="TDest"/> object graphs, 
        /// return the property chain of <paramref name="propertyOnSource"/> as it is located in the destination object graph.
        /// </summary>
        /// <param name="sourceType">The type that declares the <paramref name="propertyOnSource"/></param>
        /// <param name="propertyOnSource">The property to find within <typeparamref name="TDest"/></param>
        /// <returns></returns>
        public string FindSourcePropertyOnDestination(Type sourceType, string propertyOnSource)
        {
            var matchingRankedMappings =
                (from m in _mappings
                 where m.SourceType == sourceType
                 let matchScore = m.GetSourcePropertyChainMatching(propertyOnSource).Count()
                 select new {mapping = m, matchScore}
                 into matches
                 orderby matches.matchScore descending
                 select matches).ToList();

            foreach (var x in matchingRankedMappings)
            {
                string propertyChainToAddToDestinationProperty
                    = x.mapping.GetSourcePropertyChainToAddToDestination(propertyOnSource);
                bool match = x.mapping.DestinationPropertyType.HasPublicProperty(
                    propertyChainToAddToDestinationProperty, true);
                if (match)
                {
                    return PropertyNames.ConcatPropertyChain(x.mapping.DestinationPropertyName,
                                                             propertyChainToAddToDestinationProperty);
                }
            }
            return String.Empty;
        }


        public IEnumerable<ObjectToObjectMapping> GetMappingsForSource(Type sourceType)
        {
            return GetMappingsForSource(sourceType, String.Empty);
        }


        public IEnumerable<ObjectToObjectMapping> GetMappingsForSource(Type sourceType, string childObjectProperty)
        {
            return
                _mappings.Where(m => m.SourceType == sourceType && m.SourceProperyName == childObjectProperty).ToList();
        }


        public OneToManyObjectToObjectMap<TDest> MapFrom<TSource>(Expression<Func<TDest, object>> destinationExpression,
                                                                  Expression<Func<TSource, object>> sourceExpression)
        {
            var newMapping = new ObjectToObjectMapping(typeof (TDest),
                                                       typeof (TSource),
                                                       ReflectionUtil.GetPropertyChain(destinationExpression),
                                                       ReflectionUtil.GetPropertyChain(sourceExpression));
            DoAdd(newMapping);
            return this;
        }


        public OneToManyObjectToObjectMap<TDest> MapFrom<TSource>(Expression<Func<TDest, object>> destinationExpression,
                                                                  string sourceExpression)
        {
            var newMapping = new ObjectToObjectMapping(typeof (TDest),
                                                       typeof (TSource),
                                                       ReflectionUtil.GetPropertyChain(destinationExpression),
                                                       sourceExpression);
            DoAdd(newMapping);
            return this;
        }
    }



    public class ObjectToObjectMap
    {
        public OneToManyObjectToObjectMap<TDest> MapTo<TDest>()
        {
            return new OneToManyObjectToObjectMap<TDest>();
        }


        #region Class Members

        static ObjectToObjectMap()
        {
            Instance = new ObjectToObjectMap();
        }


        public static ObjectToObjectMap Instance { get; set; }

        #endregion
    }
}