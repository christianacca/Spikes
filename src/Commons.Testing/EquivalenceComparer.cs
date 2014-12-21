using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;

namespace Eca.Commons.Testing
{
    /// <summary>
    /// Compares two object graphs for equaility using "by value" semantics
    /// </summary>
    /// <remarks>
    /// Compares two object graphs of arbitary depth. The two graphs will be
    /// considered equal if each node from one graph has a corrosponding node in
    /// the other graph with identical property values. A filter can be supplied
    /// that will determine which properties to specifically include / exclude
    /// when comparing nodes
    /// </remarks>
    public class EquivalenceComparer : IEqualityComparer, IEquatable<EquivalenceComparer>
    {
        #region Member Variables

        private readonly Stack<object> _leftStack;

        private readonly IDictionary<Type, EquivalenceComparer> _nestedComparers =
            new Dictionary<Type, EquivalenceComparer>();

        private Type _primaryTypeToCompare;

        private readonly PropertyNameFilter _propertyFilter;

        private readonly Stack<object> _rightStack;

        #endregion


        #region Constructors

        /// <summary>
        /// Internal so as to test. Do not use this constructor in production code
        /// </summary>
        internal EquivalenceComparer(PropertyNameFilter filter,
                                     IDictionary<Type, EquivalenceComparer> nestedComparers)
            : this(filter, nestedComparers, new Stack<object>(), new Stack<object>()) {}


        private EquivalenceComparer(PropertyNameFilter filter,
                                    IDictionary<Type, EquivalenceComparer> nestedComparers,
                                    Stack<object> leftStack,
                                    Stack<object> rightStack)
        {
            _leftStack = leftStack;
            _rightStack = rightStack;
            _propertyFilter = filter == null ? GlobalFilter : filter.Union(GlobalFilter);
            _nestedComparers = nestedComparers == null
                                   ? new Dictionary<Type, EquivalenceComparer>()
                                   : new Dictionary<Type, EquivalenceComparer>(nestedComparers);
        }

        #endregion


        #region Properties

        public Type PrimaryTypeToCompare
        {
            get { return _primaryTypeToCompare ?? typeof (Object); }
            internal set { _primaryTypeToCompare = value; }
        }

        public PropertyNameFilter PropertyFilter
        {
            get { return _propertyFilter; }
        }

        #endregion


        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
                return true;
                //TODO: this should not be necessary if PropertiesNotEqual handled nulls
            else if (x == null || y == null)
                return false;
            else
                return PropertiesNotEqual(x, y).Count() == 0;
        }


        /// <exception cref="NotImplementedException">GetHashCode needs to be carefully thought through to implement in a proper way.</exception>
        public int GetHashCode(object obj)
        {
            throw new NotImplementedException(
                "GetHashCode needs to be carefully thought through to implement in a proper way.");
        }

        #endregion


        #region IEquatable<EquivalenceComparer> Members

        public bool Equals(EquivalenceComparer obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return _nestedComparers.SequenceEqual(obj._nestedComparers) &&
                   Object.Equals(obj._propertyFilter, _propertyFilter);
        }

        #endregion


        /// <exception cref="ArgumentException">obj1 and obj2 do not share any common ancestor</exception>
        private Type ChooseTypeToReflect(Type left, Type right)
        {
            if (left.IsAssignableFrom(right)) return left;
            if (right.IsAssignableFrom(left)) return right;

            Type commonAncestorType = FindClosestCommonAncestorType(left, right);

            if (commonAncestorType == null)
                throw new ArgumentException("no common ancestor to compare");

            return commonAncestorType;
        }


        private Type FindClosestCommonAncestorType(Type left, Type right)
        {
            const bool ignoreObjectType = true;
            IEnumerable<Type> leftInheranceChain = GetInheritanceChain(left, ignoreObjectType);
            IEnumerable<Type> rightInheranceChain = GetInheritanceChain(right, ignoreObjectType);

            IEnumerable<Type> commonAncestors = leftInheranceChain.Intersect(rightInheranceChain);
            return commonAncestors.LastOrDefault();
        }


        private IEnumerable<Type> GetInheritanceChain(Type type, bool ignoreObjectType)
        {
            Type current = type;
            do
            {
                yield return current;
                if (ignoreObjectType && current.BaseType == typeof (Object))
                    current = null;
                else
                    current = current.BaseType;
            } while (current != null);
        }


        private IEnumerable<PropertyInfo> GetPropertiesToCompare(Type typeToReflect)
        {
            return _propertyFilter.Apply(typeToReflect);
        }


        private IEqualityComparer GetPropertyComparerFor(PropertyReference lhs, PropertyReference rhs)
        {
            if (lhs.IsCollectionType)
            {
                Type lhsElementType = lhs.GetCollectionElementType();
                Type rhsElementType = rhs.GetCollectionElementType();

                //The only reason GetCollectionElementType would return null is if the collection is empty;
                //if the collection is empty there will be nothing to compare, hence why its ok to create
                //a collection comparer that returns true for any element in the collection
                return lhsElementType == null && rhsElementType == null
                           ? new CollectionComparer((o1, o2) => true)
                           : new CollectionComparer(GetPropertyComparerFor(lhsElementType ?? rhsElementType));
            }
            else
            {
                return GetPropertyComparerFor(lhs.Type);
            }
        }


        private IEqualityComparer GetPropertyComparerFor(Type type)
        {
            if (ShouldUseEqualsMethodDefinedByType(type)) return _simpleEqualsComparer;

            if (_nestedComparers.ContainsKey(type)) return _nestedComparers[type];
            if (PrimaryTypeToCompare == type) return this;

            return new EquivalenceComparer(null, null, new Stack<object>(_leftStack), new Stack<object>(_rightStack));
        }


        public virtual IEnumerable<string> PropertiesNotEqual<T>(T obj1, T obj2)
        {
            Type typeToReflect = ChooseTypeToReflect(ReflectionUtil.GetActualType(obj1),
                                                     ReflectionUtil.GetActualType(obj2));
            return PropertiesNotEqual(obj1, obj2, typeToReflect);
        }


        public virtual IEnumerable<string> PropertiesNotEqual<T>(T obj1, T obj2, Type typeToReflect)
        {
            IEnumerable<PropertyInfo> properties = GetPropertiesToCompare(typeToReflect);
            return (from property in properties
                    let value1 = new PropertyReference(obj1, property)
                    let value2 = new PropertyReference(obj2, property)
                    where PropertyValuesAreNotEqual(value1, value2)
                    select property.Name).ToList();
        }


        private bool PropertyValuesAreNotEqual(PropertyReference left, PropertyReference right)
        {
            if (SameValuesFoundOnStacksOnSameDepth(_leftStack, left.Value, _rightStack, right.Value))
                return false;

            object leftValue = left.Value;
            object rightValue = right.Value;
            try
            {
                _leftStack.Push(leftValue);
                _rightStack.Push(rightValue);

                IEqualityComparer comparer = GetPropertyComparerFor(left, right);
                return !comparer.Equals(leftValue, rightValue);
            }
            finally
            {
                _leftStack.Pop();
                _rightStack.Pop();
            }
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as EquivalenceComparer);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (_nestedComparers.GetSequenceHashCode()*397) ^ _propertyFilter.GetHashCode();
            }
        }


        /// <summary>
        /// Register a type that should be compared using the type's own Equal's method.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default instances of the following types will be
        /// compared using the type's own Equal's method:
        /// <list type="bullet">
        /// <item>built-in types (eg int, string, datetime)</item>
        /// <item>nullable types</item>
        /// <item>Enumerations</item>
        /// <item>types decorated with the attribute <see cref="IsBuiltinTypeAttribute"/></item>
        /// </list>
        /// All other types will be compared for equivalence by examining the instance's individual public properties.
        /// </para>
        /// <para>
        /// The problem with a comparison that examines each individual property is a slow down in performance.
        /// </para>
        /// <para>
        /// To avoid this performance overhead, you can choose to register more types that should be compared
        /// using the type's own Equals method.
        /// </para>
        /// </remarks>
        public static void RegisterTypeToCompareUsingEqualsMethodDefinedByType(Type type)
        {
            _typesToComparUsingEqualsMethodDefinedByType[type] = true;
        }


        private bool ShouldUseEqualsMethodDefinedByType(Type type)
        {
            bool useEqualsMethodDefinedByType;
            bool typeLearnt = _typesToComparUsingEqualsMethodDefinedByType.TryGetValue(type,
                                                                                       out useEqualsMethodDefinedByType);

            if (!typeLearnt)
            {
                useEqualsMethodDefinedByType = type.IsPrimitive || ReflectionUtil.IsNullableType(type)
                                               || type == typeof (string)
                                               || type == typeof (DateTime) || type == typeof (decimal)
                                               || type.IsEnum ||
                                               type.GetCustomAttributes(typeof (IsBuiltinTypeAttribute), false).Length >
                                               0;
                _typesToComparUsingEqualsMethodDefinedByType.Add(type, useEqualsMethodDefinedByType);
            }
            return useEqualsMethodDefinedByType;
        }


        public override string ToString()
        {
            return string.Format("NestedComparers: {{ {0} }}, PropertyFilter: {{ {1} }}",
                                 _nestedComparers.Join(",", "<Empty>"),
                                 _propertyFilter);
        }


        public static void UnregisterTypeToCompareUsingEqualsMethodDefinedByType(Type type)
        {
            _typesToComparUsingEqualsMethodDefinedByType.Remove(type);
        }


        private static readonly IEqualityComparer _simpleEqualsComparer = new SimpleComparer();

        private static readonly IDictionary<Type, bool> _typesToComparUsingEqualsMethodDefinedByType =
            new Dictionary<Type, bool>();

        #endregion


        #region Class Members

        static EquivalenceComparer()
        {
            GlobalFilter = PropertyNameFilter.NoFilter;
        }


        /// <summary>
        /// Same as calling For&lt;object&gt;().
        /// </summary>
        /// <remarks>
        /// Prefer to call <see cref="For{T}"/> with the specific type you want to compare. Doing so will result in
        /// better comparisons when the target object being compared has properties typed as itself.
        /// </remarks>
        public static EquivalenceComparer Default
        {
            get { return new EquivalenceComparer(null, null); }
        }


        /// <summary>
        /// The property filter that all comparer's created should use when deciding which properties
        /// to include in comparison. This global filter will be combined with any property filter
        /// supplied specifically to the comparer being created.
        /// </summary>
        public static PropertyNameFilter GlobalFilter { get; set; }


        public static void ClearGlobalFilter()
        {
            GlobalFilter = PropertyNameFilter.NoFilter;
        }


        public static Builder<T> For<T>()
        {
            return new Builder<T>();
        }


        private static bool SameValuesFoundOnStacksOnSameDepth(Stack<object> leftStack,
                                                               object leftValue,
                                                               Stack<object> rightStack,
                                                               object rightValue)
        {
            int indexOfLeftObjectInLeftStack = leftStack.IndexOfSame(leftValue);
            int indexOfRightObjectInRightStack = rightStack.IndexOfSame(rightValue);

            //both left and right objects not found
            if (indexOfLeftObjectInLeftStack == -1 && indexOfRightObjectInRightStack == -1) return false;

            return indexOfLeftObjectInLeftStack == indexOfRightObjectInRightStack;
        }

        #endregion


        /// <summary>
        /// Implements a fluent interface for the construction of <see cref="EquivalenceComparer"/> objects
        /// </summary>
        public class Builder<T>
        {
            #region Member Variables

            private readonly Dictionary<Type, EquivalenceComparer> _nestedComparers =
                new Dictionary<Type, EquivalenceComparer>();

            private PropertyNameFilter _propertyFilter = PropertyNameFilter.NoFilter;

            #endregion


            public EquivalenceComparer Build()
            {
                return new EquivalenceComparer(_propertyFilter, _nestedComparers) {PrimaryTypeToCompare = typeof (T)};
            }


            public Builder<T> Excludes(params Expression<Func<T, object>>[] propertyNames)
            {
                _propertyFilter = _propertyFilter.Union(PropertyNameFilter.ToExclude(propertyNames));
                return this;
            }


            public Builder<T> Excludes(params string[] propertyNames)
            {
                return Excludes((IEnumerable<string>) propertyNames);
            }


            public Builder<T> Excludes(IEnumerable<string> propertyNames)
            {
                _propertyFilter = _propertyFilter.Union(PropertyNameFilter.ToExclude(propertyNames));
                return this;
            }


            public Builder<T> Includes(params Expression<Func<T, object>>[] propertyNames)
            {
                _propertyFilter = _propertyFilter.Union(PropertyNameFilter.ToInclude(propertyNames));
                return this;
            }


            public Builder<T> Includes(params string[] propertyNames)
            {
                return Includes((IEnumerable<string>) propertyNames);
            }


            public Builder<T> Includes(IEnumerable<string> propertyNames)
            {
                typeof (T).RequireProperties(propertyNames);
                _propertyFilter = _propertyFilter.Union(PropertyNameFilter.ToInclude(propertyNames));
                return this;
            }


            public Builder<T> With<TChild>(Builder<TChild> childBuilder)
            {
                _nestedComparers.Add(typeof (TChild), childBuilder.Build());
                return this;
            }


            public static implicit operator EquivalenceComparer(Builder<T> builder)
            {
                return builder.Build();
            }
        }
    }
}