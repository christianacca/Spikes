using System;
using System.Collections;
using System.Linq;

namespace Eca.Commons
{
    /// <summary>
    /// Compares two collection to determine whether they are equal.
    /// </summary>
    /// <remarks>
    /// A collection will be considered equal to another collection when each
    /// item can be matched to a exactly one item in the other collection. Two
    /// items will considered a matching pair if they are objects of the same
    /// class (ie <see cref="object.GetType"/>
    ///   returns the same <see cref="Type"/>) and by default if
    /// <see cref="Object.Equals(object, object)"/> returns true with the two
    /// items as arguments. To override the default <see
    /// cref="Object.Equals(object, object)"/> used in the comparison, supply a
    /// <see cref="IEqualityComparer"/> as a constructor parameter
    /// </remarks>
    public class CollectionComparer : IEqualityComparer
    {
        #region Member Variables

        private readonly Func<object, object, bool> _compare;
        private readonly IEqualityComparer _elementComparer;

        #endregion


        #region Constructors

        public CollectionComparer() : this(new SimpleComparer()) {}


        public CollectionComparer(IEqualityComparer elementComparer)
        {
            _elementComparer = elementComparer;
            _compare = (x, y) => _elementComparer.Equals(x, y);
        }


        public CollectionComparer(Func<object, object, bool> compare)
        {
            _compare = compare;
        }

        #endregion


        #region IEqualityComparer Members

        public new bool Equals(object x, object y)
        {
            return Compare((IEnumerable) x, (IEnumerable) y) == 0;
        }


        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        #endregion


        private bool BelongToSameInheritanceHierarchy(object lhs, object rhs)
        {
            Type lhsType = lhs.GetType();
            Type rhsType = rhs.GetType();
            return lhsType.IsAssignableFrom(rhsType) || rhsType.IsAssignableFrom(lhsType);
        }


        public int Compare(IEnumerable list1, IEnumerable list2)
        {
            if (list1 == null && list2 == null) return 0;
            if (list1 == null || list2 == null) return -1;

            return CompareItems(list1, list2);
        }


        private int CompareItems(IEnumerable list1, IEnumerable list2)
        {
            if (CountOfItems(list1) != CountOfItems(list2)) return -1;

            foreach (object o in list1)
            {
                int countOfItemInList1 = CountOfItemsMatching(o, list1);
                int countOfItemInList2 = CountOfItemsMatching(o, list2);
                if (countOfItemInList1 != countOfItemInList2) return -1;
            }

            return 0;
        }


        private int CountOfItems(IEnumerable list)
        {
            return list.Cast<object>().Count();
        }


        private int CountOfItemsMatching(object itemToFind, IEnumerable listToSearch)
        {
            int count = 0;
            foreach (object item in listToSearch)
            {
                if (BelongToSameInheritanceHierarchy(item, itemToFind) && _compare(itemToFind, item))
                    count++;
            }
            return count;
        }
    }
}