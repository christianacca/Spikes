using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eca.Commons.Reflection
{
    public static class ReflectTypeExtensions
    {
        #region Class Members

        /// <summary>
        /// Returns the default instance value for the <paramref name="type"/> supplied.
        /// Equivalent to calling <c>default(T)</c> where <c>T</c> is supplied as the <paramref name="type"/>
        /// </summary>
        public static object GetDefault(this Type type)
        {
            var getDefault = typeof (ReflectTypeExtensions)
                .GetMethod("GetDefaultGeneric");

            var typed = getDefault.MakeGenericMethod(type);
            return typed.Invoke(null, new object[] {});
        }


        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }


        /// <summary>
        /// Searches <paramref name="targetType"/> for the properties specified in <paramref name="propertyNames"/>
        /// </summary>
        /// <param name="targetType">the type to search</param>
        /// <param name="propertyNames">names of properties to search for</param>
        /// <param name="bindingAttr">defines the set of properties to search witin the <paramref name="targetType"/></param>
        /// <returns>a collection containing a <see cref="ParameterInfo"/> object for each property matching the search criteria</returns>
        /// <remarks>
        /// If a property specified in <paramref name="propertyNames"/> is not found then no entry will appear in the results for that
        /// missing property
        /// </remarks>
        public static IEnumerable<PropertyInfo> GetProperties(this Type targetType,
                                                              IEnumerable<string> propertyNames,
                                                              BindingFlags bindingAttr)
        {
            var result = new List<PropertyInfo>();
            foreach (string name in propertyNames)
            {
                PropertyInfo property = targetType.GetProperty(name, bindingAttr);
                if (property != null) result.Add(property);
            }
            return result;
        }


        public static bool HasAttribute<T>(this Type type)
        {
            Type attributeType = typeof (T);
            return type.GetCustomAttributes(attributeType, true).Any();
        }


        public static bool HasPublicProperty(this Type source, string propertyName)
        {
            return HasPublicProperty(source, propertyName, false);
        }


        public static bool HasPublicProperty(this Type source, string propertyName, bool treatEnumerableAsIndexable)
        {
            string[] propertyChain = PropertyNames.GetPropertyChain(propertyName);

            PropertyInfo propertyMatch;
            Type typeToSearch = source;
            foreach (var property in propertyChain)
            {
                if (property.Contains("[") && treatEnumerableAsIndexable && typeToSearch.IsGenericCollectionType())
                {
                    typeToSearch = typeToSearch.GetGenericCollectionElement();
                }
                else
                {
                    propertyMatch = typeToSearch.GetProperty(property);
                    if (propertyMatch == null) return false;
                    typeToSearch = propertyMatch.PropertyType;
                }
            }

            return true;
        }


        /// <summary>
        /// Checks that <paramref name="type"/> declares all the public properties listed in <paramref name="requiredProperties"/>.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="requiredProperties">A list of property names that <paramref name="type"/> is expected to declare</param>
        /// <exception cref="PreconditionException">Thrown on the first occurance of a property that <paramref name="type"/> does not declare</exception>
        public static void RequireProperties(this Type type, IEnumerable<string> requiredProperties)
        {
            const string failureMsg = "{0} expected to declare property: {1}";
            requiredProperties.RequireForEach(propertyName => type.HasPublicProperty(propertyName),
                                              propertyName => String.Format(failureMsg, type.Name, propertyName));
        }

        #endregion
    }
}