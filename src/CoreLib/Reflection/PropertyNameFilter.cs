using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eca.Commons.Extensions;

namespace Eca.Commons.Reflection
{
    /// <summary>
    /// Returns the properties implemented by a target object whose name match the property
    /// name filters
    /// </summary>
    public class PropertyNameFilter : IEquatable<PropertyNameFilter>
    {
        #region Member Variables

        private readonly HashSet<string> _excludes;
        private readonly HashSet<string> _includes;

        #endregion


        #region Constructors

        private PropertyNameFilter() : this(null, null) {}


        public PropertyNameFilter(IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            _includes = includes == null ? new HashSet<string>() : new HashSet<string>(includes);
            _excludes = excludes == null ? new HashSet<string>() : new HashSet<string>(excludes);
        }

        #endregion


        #region Properties

        public IEnumerable<string> Excludes
        {
            get { return _excludes; }
        }

        public IEnumerable<string> Includes
        {
            get { return _includes; }
        }

        #endregion


        #region IEquatable<PropertyNameFilter> Members

        public bool Equals(PropertyNameFilter obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return _excludes.SequenceEqual(obj._excludes) && _includes.SequenceEqual(obj._includes);
        }

        #endregion


        public IEnumerable<PropertyInfo> Apply(Type targetType)
        {
            if (Includes.Any())
                return GetInstanceProperiesFor(targetType, Includes.Except(Excludes));
            else
                return GetPublicInstancePropertiesFor(targetType).Except(GetExcludedProperties(targetType)).ToList();
        }


        public IEnumerable<PropertyInfo> Apply<T>()
        {
            return Apply(typeof (T));
        }


        public IEnumerable<PropertyInfo> Apply(object obj)
        {
            return Apply(ReflectionUtil.GetActualType(obj));
        }


        private HashSet<PropertyInfo> GetExcludedProperties(Type type)
        {
            return GetInstanceProperiesFor(type, _excludes);
        }


        public PropertyNameFilter Union(PropertyNameFilter filter)
        {
            return new PropertyNameFilter(filter.Includes.Union(Includes), filter.Excludes.Union(Excludes));
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyNameFilter)) return false;
            return Equals((PropertyNameFilter) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (_excludes.GetSequenceHashCode()*397) ^ _includes.GetSequenceHashCode();
            }
        }


        public override string ToString()
        {
            return string.Format("Excludes: {0}, Includes: {1}",
                                 _excludes.Join(",", "<Empty>"),
                                 _includes.Join(",", "<Empty>"));
        }

        #endregion


        #region Class Members

        public static PropertyNameFilter NoFilter = new PropertyNameFilter();


        private static HashSet<PropertyInfo> GetInstanceProperiesFor(Type type, IEnumerable<string> propertyNames)
        {
            return new HashSet<PropertyInfo>(type.GetProperties(propertyNames, ReflectionUtil.InstanceMembers));
        }


        private static HashSet<PropertyInfo> GetPublicInstancePropertiesFor(Type type)
        {
            return new HashSet<PropertyInfo>(type.GetProperties(ReflectionUtil.PublicInstanceMembers));
        }


        public static FilterBuilder ToExclude<T>(params Expression<Func<T, object>>[] accessorExpressions)
        {
            return new FilterBuilder().ToExclude(accessorExpressions);
        }


        public static FilterBuilder ToExclude(params string[] propertyNames)
        {
            return new FilterBuilder().ToExclude(propertyNames);
        }


        public static FilterBuilder ToExclude(IEnumerable<string> propertyNames)
        {
            return new FilterBuilder().ToExclude(propertyNames);
        }


        public static FilterBuilder ToInclude<T>(params Expression<Func<T, object>>[] accessorExpressions)
        {
            return new FilterBuilder().ToInclude(accessorExpressions);
        }


        public static FilterBuilder ToInclude(params string[] propertyNames)
        {
            return new FilterBuilder().ToInclude(propertyNames);
        }


        public static FilterBuilder ToInclude(IEnumerable<string> propertyNames)
        {
            return new FilterBuilder().ToInclude(propertyNames);
        }

        #endregion


        public class FilterBuilder
        {
            #region Constructors

            public FilterBuilder()
            {
                Excludes = new HashSet<string>();
                Includes = new HashSet<string>();
            }

            #endregion


            #region Properties

            private HashSet<string> Excludes { get; set; }
            private HashSet<string> Includes { get; set; }

            #endregion


            public PropertyNameFilter Build()
            {
                return new PropertyNameFilter(Includes, Excludes);
            }


            public FilterBuilder ToExclude(params string[] propertyNames)
            {
                Excludes.UnionWith(propertyNames);
                return this;
            }


            public FilterBuilder ToExclude(IEnumerable<string> propertyNames)
            {
                Excludes.UnionWith(propertyNames);
                return this;
            }


            public FilterBuilder ToExclude<T>(params Expression<Func<T, object>>[] accessorExpressions)
            {
                return ToExclude(PropertyNames.For(accessorExpressions));
            }


            public FilterBuilder ToInclude(params string[] propertyNames)
            {
                Includes.UnionWith(propertyNames);
                return this;
            }


            public FilterBuilder ToInclude(IEnumerable<string> propertyNames)
            {
                Includes.UnionWith(propertyNames);
                return this;
            }


            public FilterBuilder ToInclude<T>(params Expression<Func<T, object>>[] accessorExpressions)
            {
                return ToInclude(PropertyNames.For(accessorExpressions));
            }


            public static implicit operator PropertyNameFilter(FilterBuilder from)
            {
                return from.Build();
            }
        }
    }
}