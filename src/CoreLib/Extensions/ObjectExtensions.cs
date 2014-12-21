using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eca.Commons.Reflection;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Extensions
{
    public class RequirePropertyOptions
    {
        #region Constructors

        public RequirePropertyOptions()
        {
            IgnoreReadonly = false;
        }

        #endregion


        #region Properties

        public bool IgnoreReadonly { get; set; }

        #endregion
    }



    public static class ObjectExtensions
    {
        #region Overridden object methods

        public static IDictionary<string, string> ToStringDictionary(this object source)
        {
            return source.ToDictionary(propertyValue => propertyValue.ToString());
        }

        #endregion


        #region Class Members

        public static string ClassName(this object source)
        {
            return source.GetType().Name;
        }

        /// <summary>
        /// If the string is null, converts it to the default string specified. Essentially the same as the SQL IsNull()
        /// function.
        /// </summary>
        /// <param name="inString"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public static string IsNull(this object inString, string defaultString)
        {
            if (inString == null)
                return defaultString;

            return inString.ToString();
        }

        /// <seealso cref="CopyPropertiesTo(object,object,bool)"/>
        public static void CopyPropertiesTo(this object source, object target)
        {
            CopyPropertiesTo(source, target, false);
        }


        /// <summary>
        /// Performs a shallow copy of public properties from <paramref name="source"/> to <paramref name="target"/>
        /// </summary>
        /// <param name="target">target object which will receive property values from <paramref name="source"/></param>
        /// <param name="mustMatchAllTargetProperties">When true, all properties on the target must have a matching property on source</param>
        /// <param name="source">source object whose properties are being copied</param>
        public static void CopyPropertiesTo(this object source, object target, bool mustMatchAllTargetProperties)
        {
            if (ReferenceEquals(null, target)) return;
            if (ReferenceEquals(null, source)) return;

#if !SILVERLIGHT
            if (mustMatchAllTargetProperties)
            {
                source.RequireAllPublicPropertyNamesOf(target, new RequirePropertyOptions {IgnoreReadonly = true});
            }
#endif

            var sourceProperties = source.GetType().GetProperties().AsEnumerable();
            var targetProperties = target.GetType().GetProperties().AsEnumerable().ToList();

            var targetMappedToSource
                = from targetProperty in targetProperties
                  join sourceProperty in sourceProperties on
                      new {targetProperty.Name, targetProperty.PropertyType}
                      equals
                      new {sourceProperty.Name, sourceProperty.PropertyType}
                  where sourceProperty.CanRead
                  select
                      new
                          {
                              Target = new PropertyReference(target, targetProperty),
                              Source = new PropertyReference(source, sourceProperty)
                          };

            foreach (var mappedProperty in targetMappedToSource)
            {
                mappedProperty.Target.Value = mappedProperty.Source.Value;
            }
        }


        public static string FullClassName(this object source)
        {
            return source.GetType().FullName;
        }


#if !SILVERLIGHT
        /// <seealso cref="RequirePublicPropertyNamesOf{T}(object,T,Eca.Commons.Extensions.RequirePropertyOptions,System.Linq.Expressions.Expression{System.Func{T,object}}[])"/>
        public static void RequireAllPublicPropertyNamesOf(this object source,
                                                           object target,
                                                           RequirePropertyOptions options)
        {
            RequirePublicPropertyNamesOf(source, target, options, null);
        }


        /// <seealso cref="RequirePublicPropertyNamesOf{T}(object,T,Eca.Commons.Extensions.RequirePropertyOptions,System.Linq.Expressions.Expression{System.Func{T,object}}[])"/>
        public static void RequireAllPublicPropertyNamesOf(this object source, object target)
        {
            RequirePublicPropertyNamesOf(source, target, new RequirePropertyOptions(), null);
        }


        /// <seealso cref="RequirePublicPropertyNamesOf{T}(object,T,Eca.Commons.Extensions.RequirePropertyOptions,System.Linq.Expressions.Expression{System.Func{T,object}}[])"/>
        public static void RequirePublicPropertyNamesOf<T>(this object source,
                                                           T target,
                                                           params Expression<Func<T, object>>[] ignore)
        {
            source.RequirePublicPropertyNamesOf(target, null, ignore);
        }


        /// <seealso cref="RequirePublicPropertyNamesOf{T}(object,T,Eca.Commons.Extensions.RequirePropertyOptions,System.Linq.Expressions.Expression{System.Func{T,object}}[])"/>
        public static void RequirePublicPropertyNamesOf<T>(this object source,
                                                           T target)
        {
            source.RequirePublicPropertyNamesOf(target, new RequirePropertyOptions(), null);
        }


        /// <summary>
        /// Checks that <paramref name="source"/> declares public properties of <paramref name="target"/>, using the
        /// property name as the matching criteria, skipping any properties on target explicitly ignored.
        /// </summary>
        /// <param name="source">The source object to check for properties declared on target</param>
        /// <param name="target">target object whose public properties must be declared on source</param>
        /// <param name="options">optional options that determine which properties on target should be considered in check</param>
        /// <param name="ignore">optional list of properties to ignore on target when performing the check</param>
        /// <exception cref="PreconditionException">Thrown on the first occurance of a property that <paramref name="source"/> does not declare</exception>        
        public static void RequirePublicPropertyNamesOf<T>(this object source,
                                                           T target,
                                                           RequirePropertyOptions options,
                                                           params Expression<Func<T, object>>[] ignore)
        {
            options = options ?? new RequirePropertyOptions();

            IEnumerable<string> propertiesToIgnore =
                ignore.SafeToList().Select(x => ReflectionUtil.GetAccessor(x).Name);

            var candidateTargetProperties =
                from p in ReflectionUtil.GetPublicInstanceProperties(target)
                where !options.IgnoreReadonly || p.CanWrite
                select p.Name;
            IEnumerable<string> targetProperties = candidateTargetProperties.Except(propertiesToIgnore);

            ReflectionUtil.GetActualType(source).RequireProperties(targetProperties);
        }
#endif


        public static string SafeClassName(this object source)
        {
            return source == null ? String.Empty : source.GetType().Name;
        }


        public static Type SafeGetType(this object source)
        {
            return source == null ? null : source.GetType();
        }


        public static string SafeFullClassName(this object source)
        {
            return source == null ? String.Empty : source.GetType().FullName;
        }


        /// <summary>
        /// See <see cref="SafeToDictionary(object,System.Reflection.BindingFlags)"/>
        /// </summary>
        public static IDictionary<string, object> SafeToDictionary(this object source)
        {
            if (source == null) return null;
            return source.ToDictionary(propertyValue => propertyValue);
        }


        /// <summary>
        /// Converts properties from <paramref name="source"/> into a dictionary. Where <paramref name="source"/> is <c>null</c> will return <c>null</c>
        /// </summary>
        /// <remarks>
        /// See <see cref="ToDictionary{T}(object,System.Func{object,T},System.Func{string,string},System.Reflection.BindingFlags)"/>
        /// for more details
        /// </remarks>
        public static IDictionary<string, object> SafeToDictionary(this object source, BindingFlags propertySelector)
        {
            if (source == null) return null;
            return source.ToDictionary(propertyValue => propertyValue, propertySelector);
        }


        /// <summary>
        /// See <see cref="ToDictionary{T}(object,System.Func{object,T},System.Func{string,string},System.Reflection.BindingFlags)"/>
        /// </summary>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary(propertyValue => propertyValue);
        }


        /// <summary>
        /// See <see cref="ToDictionary{T}(object,System.Func{object,T},System.Func{string,string},System.Reflection.BindingFlags)"/>
        /// </summary>
        public static IDictionary<string, object> ToDictionary(this object source, BindingFlags scope)
        {
            return source.ToDictionary(propertyValue => propertyValue, scope);
        }


        /// <summary>
        /// See <see cref="ToDictionary{T}(object,System.Func{object,T},System.Func{string,string},System.Reflection.BindingFlags)"/>
        /// </summary>
        public static IDictionary<string, T> ToDictionary<T>(this object source,
                                                             Func<object, T> propertyValueSelector,
                                                             Func<string, string> propertyNameTransform)
        {
            return source.ToDictionary(propertyValueSelector,
                                       propertyNameTransform,
                                       BindingFlags.Public | BindingFlags.Instance);
        }


        /// <summary>
        /// See <see cref="ToDictionary{T}(object,System.Func{object,T},System.Func{string,string},System.Reflection.BindingFlags)"/>
        /// </summary>
        public static IDictionary<string, T> ToDictionary<T>(this object source, Func<object, T> propertyValueSelector)
        {
            return source.ToDictionary(propertyValueSelector, BindingFlags.Public | BindingFlags.Instance);
        }


        /// <summary>
        /// See <see cref="ToDictionary{T}(object,System.Func{object,T},System.Func{string,string},System.Reflection.BindingFlags)"/>
        /// </summary>
        public static IDictionary<string, T> ToDictionary<T>(this object source,
                                                             Func<object, T> propertyValueSelector,
                                                             BindingFlags propertySelector)
        {
            return ToDictionary(source, propertyValueSelector, propertyName => propertyName, propertySelector);
        }


        /// <summary>
        /// Converts each property on <paramref name="source"/> into a key/value pair and returns these as a dictionary
        /// </summary>
        /// <param name="source">The object to be converted</param>
        /// <param name="propertyValueSelector">
        /// A delegate that is supplied property values from <paramref name="source"/>, and should return the value that should be associated with the key
        /// </param>
        /// <param name="propertyNameTransform">
        /// A delegate that is supplied property names from <paramref name="source"/>, and should return a key name
        /// </param>
        /// <param name="scope">Determines which properties from <paramref name="source"/> should be returned in a dictionary</param>
        /// <remarks>
        /// If <paramref name="source"/> is actually a <see cref="IDictionary{TKey,TValue}"/> then it will be returned <em>as is</em>
        /// ie the exact same reference to <paramref name="source"/> will be returned and <em>not</em> a clone of its key value pairs
        /// </remarks>
        public static IDictionary<string, T> ToDictionary<T>(this object source,
                                                             Func<object, T> propertyValueSelector,
                                                             Func<string, string> propertyNameTransform,
                                                             BindingFlags scope)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => source).IsNotNull());
#endif

            if (source is IDictionary<string, T>)
            {
                return source as IDictionary<string, T>;
            }

            var nameValuePairs = from prop in ReflectionUtil.GetActualType(source).GetProperties(scope)
                                 where prop.CanRead
                                 let value = prop.GetValue(source, null)
                                 where value != null
                                 select
                                     new {Name = propertyNameTransform(prop.Name), Value = propertyValueSelector(value)};

            return nameValuePairs.ToDictionary(pair => pair.Name, arg => arg.Value);
        }

        #endregion
    }
}