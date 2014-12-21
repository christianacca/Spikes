using System;
using System.Collections.Generic;
using System.Linq;

namespace Eca.Commons.Extensions
{
    public static class HierarchySort
    {
        #region Class Members

        /// <summary>
        /// Return a double-linked list of nodes sorted depth first. See http://weblogs.asp.net/okloeten/archive/2006/07/09/Hierarchical-Linq-Queries.aspx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">the items to be sorted into nodes</param>
        /// <param name="startWith">A predicate that will identify the item from <paramref name="source"/> that is the root of the hierarchy</param>
        /// <param name="connectBy">A delegate that will accept a reference to two nodes from the <paramref name="source"/> and should return true if the two nodes are connected</param>
        public static IEnumerable<Node<T>> ByDepthFirstHierarchy<T>(this IEnumerable<T> source,
                                                                    Func<T, bool> startWith,
                                                                    Func<T, T, bool> connectBy)
        {
            return source.ByDepthFirstHierarchy(startWith, connectBy, null);
        }


        private static IEnumerable<Node<T>> ByDepthFirstHierarchy<T>(this IEnumerable<T> source,
                                                                     Func<T, bool> startWith,
                                                                     Func<T, T, bool> connectBy,
                                                                     Node<T> parent)
        {
            int level = (parent == null ? 0 : parent.Level + 1);
            if (source == null) throw new ArgumentNullException("source");
            if (startWith == null) throw new ArgumentNullException("startWith");
            if (connectBy == null) throw new ArgumentNullException("connectBy");

            foreach (T value in from item in source where startWith(item) select item)
            {
                var newNode = new Node<T> {Level = level, Parent = parent, Item = value};
                yield return newNode;
                foreach (
                    Node<T> subNode in
                        source.ByDepthFirstHierarchy(possibleSub => connectBy(value, possibleSub), connectBy, newNode))
                {
                    yield return subNode;
                }
            }
        }

        #endregion


        [SkipFormatting]
        public class Node<T>
        {
            public T Item;
            public int Level;
            public Node<T> Parent;

            internal Node() {}
        }
    }
}