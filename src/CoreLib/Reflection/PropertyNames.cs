using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eca.Commons.Extensions;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Reflection
{
    public class PropertyNames
    {
        #region Class Members

        /// <summary>
        /// Concatenates two property call chaings by appending <paramref name="propertyChainToAppend"/> onto the end of <paramref name="startingPropertyChain"/>
        /// </summary>
        public static string ConcatPropertyChain(string startingPropertyChain, string propertyChainToAppend)
        {
            if (String.IsNullOrEmpty(startingPropertyChain)) return propertyChainToAppend;
            if (String.IsNullOrEmpty(propertyChainToAppend)) return startingPropertyChain;

            if (propertyChainToAppend.StartsWith("["))
            {
                return String.Format("{0}{1}", startingPropertyChain, propertyChainToAppend);
            }
            else
            {
                return String.Format("{0}.{1}", startingPropertyChain, propertyChainToAppend);
            }
        }


        /// <summary>
        /// Concatenates the property call chaings into a period '.' separated string
        /// </summary>
        public static string ConcatPropertyChain(IEnumerable<string> propertyChain)
        {
            return IntersperseWithSeperators(propertyChain).Concat();
        }


        public static IEnumerable<string> For<T>(params Expression<Func<T, object>>[] accessorExpressions)
        {
            return accessorExpressions.Select(expr => ReflectionUtil.GetAccessor(expr).Name);
        }


        /// <summary>
        /// Return the names of the properties in the property chain that is common to both 
        /// <paramref name="leftPropertyName"/> and <paramref name="rightPropertyName"/>
        /// </summary>
        public static IEnumerable<string> GetMatchingPropertyChain(string leftPropertyName, string rightPropertyName)
        {
            string[] leftChain = GetPropertyChain(leftPropertyName);
            string[] rightChain = GetPropertyChain(rightPropertyName);

            IEnumerable<string> matchingChain
                = leftChain.TakeWhile((p, i) => (i < rightChain.Length && p.Equals(rightChain[i])))
                    .ToList();
            return matchingChain;
        }


        /// <summary>
        /// Splits <paramref name="propertyName"/> into the individual properties that make up its call chain
        /// </summary>
        public static string[] GetPropertyChain(string propertyName)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => propertyName).IsNotNull());
#endif
            string[] calls = propertyName.Split(new[] {"."},
                                                StringSplitOptions.RemoveEmptyEntries);

            calls = SplitCallChainOnCollectionIndexAccess(calls).ToArray();
            return calls;
        }


        /// <summary>
        /// Returns the property chain that leads up to the <paramref name="propertyName"/> supplied
        /// </summary>
        /// <example>
        /// A PropertyName of: <em>AccountHolders.Primary.Name</em> would return <em>AccountHolders.Primary</em>
        /// </example>
        public static string GetPropertyContainerPropertyChain(string propertyName)
        {
            string[] fullyQualifiedPropertyChain = GetPropertyChain(propertyName);
            return
                fullyQualifiedPropertyChain.TakeWhile((p, i) => (i < fullyQualifiedPropertyChain.Length - 1)).Join(".");
        }


        /// <summary>
        /// Returns the name of the <paramref name="propertyName"/> excluding its property chain
        /// </summary>
        /// <example>
        /// A PropertyName of: <em>AccountHolders.Primary.Name</em> would return <em>Name</em>
        /// </example>
        public static string GetPropertyNameExcludingChain(string propertyName)
        {
            string[] fullyQualifiedPropertyChain = GetPropertyChain(propertyName);
            return fullyQualifiedPropertyChain.Last();
        }


        private static IEnumerable<string> IntersperseWithSeperators(IEnumerable<string> propertyChain)
        {
            foreach (var entry in new SmartEnumerable<string>(propertyChain))
            {
                if (!entry.IsFirst && !entry.Value.StartsWith("["))
                {
                    yield return ".";
                }

                yield return entry.Value;
            }
        }


        private static IEnumerable<string> SplitCallChainOnCollectionIndexAccess(string[] callChain)
        {
            foreach (var call in callChain)
            {
                if (call.Contains("["))
                {
                    int positionOfIndexCallStart = call.IndexOf("[");
                    string propertyCall = call.Substring(0, positionOfIndexCallStart);
                    string indexCall = call.Substring(positionOfIndexCallStart);
                    if (!String.IsNullOrEmpty(propertyCall))
                    {
                        yield return propertyCall;
                    }
                    yield return indexCall;
                }
                else
                {
                    yield return call;
                }
            }
        }


        /// <summary>
        /// Subtract from <paramref name="propertyChain"/> the properties that are also found in the property chain
        /// defined by <paramref name="propertyChainToSubtract"/>
        /// </summary>
        /// <returns>The property names in <paramref name="propertyChain"/> remaining after subtraction</returns>
        public static IEnumerable<string> SubtractPropertyChain(string propertyChain, string propertyChainToSubtract)
        {
            return SubtractPropertyChain(GetPropertyChain(propertyChain), GetPropertyChain(propertyChainToSubtract));
        }


        /// <summary>
        /// Subtract from <paramref name="propertyChain"/> the properties that are also found in the property chain
        /// defined by <paramref name="propertyChainToSubtract"/>
        /// </summary>
        /// <returns>The property names in <paramref name="propertyChain"/> remaining after subtraction</returns>
        public static IEnumerable<string> SubtractPropertyChain(string[] propertyChain, string[] propertyChainToSubtract)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => propertyChain).IsNotNull());
#endif

            if (propertyChainToSubtract == null) return propertyChain;

            return
                propertyChain.SkipWhile(
                    (s, i) => (i < propertyChainToSubtract.Length && s.Equals(propertyChainToSubtract[i]))).ToList();
        }

        #endregion
    }
}