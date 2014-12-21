using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Eca.Commons.Extensions;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Reflection
{
    public static class ReflectionUtil
    {
        #region Member Variables

        public const BindingFlags InstanceMembers =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public const BindingFlags PublicInstanceMembers = BindingFlags.Public | BindingFlags.Instance;

        #endregion


        #region Class Members

        private static readonly ICollection<string> BuiltinCollectionTypes
            = new List<string>
                  {
                      "System.Collections.Generic.ICollection",
                      "System.Collections.Generic.IEnumerable",
                      "System.Collections.Generic.IList",
                      "System.Collections.Generic.List",
                      "System.Collections.Generic.HashSet",
                      "System.Collections.Generic.KeyedByTypeCollection",
                      "System.Collections.Generic.LinkedList",
                      "System.Collections.Generic.LinkedListNode",
                      "System.Collections.Generic.Queue",
                      "System.Collections.Generic.SortedSet",
                      "System.Collections.Generic.Stack",
                      "System.Collections.Generic.SynchronizedCollection",
                      "System.Collections.Generic.SynchronizedReadOnlyCollection"
                  };


        private static Regex _proxyNamingCriteria;


        private static Regex ProxyNamingCriteria
        {
            get
            {
                //starts with any number of alpha numeric characters and is suffixed with
                //the name 'Proxy' and 32 alpha-numeric characters 
                const string proxyNamingPattern = @"^\w+?Proxy\w{32}$";
#if !SILVERLIGHT
                return
                    _proxyNamingCriteria = _proxyNamingCriteria ?? new Regex(proxyNamingPattern, RegexOptions.Compiled);
#else
                return
                    _proxyNamingCriteria = _proxyNamingCriteria ?? new Regex(proxyNamingPattern);
#endif
            }
        }


        /// <seealso cref="ObjectExtensions.CopyPropertiesTo(object, object, bool)"/>
        public static void CopyProperties(object source, object target)
        {
            source.CopyPropertiesTo(target);
        }


        public static object CreateInstance(Type type)
        {
#if !SILVERLIGHT
            return type.Assembly.CreateInstance(type.FullName, true, InstanceMembers, null, null, null, null);
#else
            return type.Assembly.CreateInstance(type.FullName);
#endif
        }


#if !SILVERLIGHT
        public static object CreateInstance(string assemblyName, string typeName)
        {
            if (!typeName.ToLower().Contains(assemblyName.ToLower()))
                typeName = assemblyName + '.' + typeName;


            return
                Activator.CreateInstance(assemblyName, typeName, true, InstanceMembers, null, null, null, null, null).
                    Unwrap();
        }


        public static object CreateInstance(string assemblyName, string typeName, object[] args)
        {
            if (!typeName.ToLower().Contains(assemblyName.ToLower()))
                typeName = assemblyName + '.' + typeName;

            return
                Activator.CreateInstance(assemblyName, typeName, true, InstanceMembers, null, args, null, null, null).
                    Unwrap();
        }

#endif


        public static T CreateInstance<T>()
        {
            return (T) CreateInstance(typeof (T));
        }


        public static T CreateInstance<T>(object[] args)
        {
            return (T) CreateInstance(typeof (T), args);
        }


        public static object CreateInstance(Type type, object[] args)
        {
#if !SILVERLIGHT
            return type.Assembly.CreateInstance(type.FullName, true, InstanceMembers, null, args, null, null);
#else
            return type.Assembly.CreateInstance(type.FullName);
#endif
        }


        private static FieldInfo FindFieldInInheritanceHierarchy(object owner, string fieldName)
        {
            FieldInfo result;
            Type type = owner.GetType();
            do
            {
                result = type.GetField(fieldName, InstanceMembers);
            } while (result == null && (type = type.BaseType) != null);

            return result;
        }


        private static PropertyInfo FindPropertyInInheritanceHierarchy(object owner, string propertyName)
        {
            PropertyInfo result;
            Type type = GetActualType(owner);
            do
            {
                result = type.GetProperty(propertyName, InstanceMembers);
            } while (result == null && (type = type.BaseType) != null);

            return result;
        }


        public static MemberInfo GetAccessor<T>(Expression<T> accessorExpression)
        {
            return GetMemberExpression(accessorExpression).Member;
        }


        public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> propertyExpression)
        {
            return (PropertyInfo) GetAccessor(propertyExpression);
        }


        public static PropertyInfo GetProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return (PropertyInfo) GetAccessor(propertyExpression);
        }


        /// <summary>
        /// Return the <see cref="Type"/> for the <paramref name="obj"/> supplied, taking into account that an object
        /// could be proxied using the castle dynamic proxy library. Where the object is proxied, return the real
        /// implementation type rather than the proxy type
        /// proxy
        /// </summary>
        public static Type GetActualType(object obj)
        {
            //note: the BaseType for a proxy could actually be Object
            //This occurs when an interface has been proxied. In this case the most satisfactory type to return 
            //is the type for the interface
            if (IsCastleDynamicProxy(obj) && obj.GetType().BaseType != typeof (Object))
            {
                return obj.GetType().BaseType;
            }
            return obj.GetType();
        }


        public static FieldInfo GetField(object owner, string fieldName)
        {
            return FindFieldInInheritanceHierarchy(owner, fieldName);
        }


        /// <exception cref="ArgumentException">obj does not implement field</exception>
        public static T GetField<T>(this object obj, string fieldName)
        {
            FieldInfo field = GetField(obj, fieldName);
            if (field == null)
                throw new ArgumentException("obj does not implement field", fieldName);
            object fieldValue = field.GetValue(obj);
            return fieldValue is T ? (T) fieldValue : default(T);
        }


        /// <exception cref="ArgumentException">obj does not implement field</exception>
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            PropertyInfo property = GetInstanceProperty(obj, propertyName);
            if (property == null)
                throw new ArgumentException("obj does not implement property", propertyName);
            object fieldValue = property.GetValue(obj, null);
            return fieldValue is T ? (T) fieldValue : default(T);
        }


        /// <summary>
        /// Same as <see cref="GetPropertyValue{T}"/>  but does not throw when <paramref name="obj"/> does not implement requested property
        /// </summary>
        public static T SafeGetPropertyValue<T>(this object obj, string propertyName)
        {
            try
            {
                return GetPropertyValue<T>(obj, propertyName);
            }
            catch (ArgumentException)
            {
                return default(T);
            }
        }


        public static Type GetGenericCollectionElement(this Type type)
        {
            if (!type.IsGenericCollectionType())
                throw new ArgumentException(String.Format("{0} is not a generic collection", type.Name));

            Type[] genericArguments = type.GetGenericArguments();

#if !SILVERLIGHT
            Check.Assert(genericArguments.Length == 1,
                         "Assuming only to find one generic type argument for collection types");
#endif

            return genericArguments[0];
        }


#if !SILVERLIGHT
        public static IEnumerable<PropertyInfo> GetInstanceProperies(object obj, IEnumerable<string> propertyNames)
        {
            return GetProperties(obj, propertyNames, InstanceMembers);
        }
#endif


        public static IEnumerable<PropertyInfo> GetInstanceProperties(object obj)
        {
            return GetProperties(obj, InstanceMembers);
        }


        public static PropertyInfo GetInstanceProperty(object obj, string propertyName)
        {
            return FindPropertyInInheritanceHierarchy(obj, propertyName);
        }


        public static object GetLeastDerived(object obj1, object obj2)
        {
            return obj1.GetType().IsAssignableFrom(obj2.GetType()) ? obj1 : obj2;
        }


        private static MemberExpression GetMemberExpression<T>(Expression<T> propertyAccessors)
        {
            if (propertyAccessors.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression) propertyAccessors.Body;
                return (MemberExpression) body.Operand;
            }
            else
            {
                return (MemberExpression) propertyAccessors.Body;
            }
        }


        public static IEnumerable<PropertyInfo> GetProperties(object obj, BindingFlags flags)
        {
            return GetActualType(obj).GetProperties(flags);
        }


#if !SILVERLIGHT
        public static IEnumerable<PropertyInfo> GetProperties(object obj,
                                                              IEnumerable<string> propertyNames,
                                                              BindingFlags memberScope)
        {
            return GetActualType(obj).GetProperties(propertyNames, memberScope);
        }
#endif


        public static string GetPropertyNames<T>(Expression<Func<T, object>> propertyAccessors)
        {
            return GetPropertyChain(propertyAccessors).Join(".", m => m.Name);
        }


        public static IEnumerable<PropertyInfo> GetPropertyChain<T>(Expression<Func<T, object>> expression)
        {
            ICollection<MemberInfo> members = GetMemberChain(expression);
            if (members.Any(m => m.MemberType != MemberTypes.Property && m.MemberType != MemberTypes.Field))
            {
                throw new ArgumentException(
                    "Expression contains members that are not properties. Consider using GetMemberChain instead.");
            }
            return members.Cast<PropertyInfo>();
        }


        public static ICollection<MemberInfo> GetMemberChain<T>(Expression<Func<T, object>> expression)
        {
            //ie: src => src
            if (expression.Body is ParameterExpression) return new List<MemberInfo>();

            var members = new List<MemberInfo>();

            var currentExpr = GetMemberExpression(expression);
            while (currentExpr != null)
            {
                members.Add(currentExpr.Member);
                currentExpr = currentExpr.Expression as MemberExpression;
            }
            members.Reverse();
            return members;
        }


#if !SILVERLIGHT
        public static IEnumerable<PropertyInfo> GetPublicInstanceProperies(object obj, IEnumerable<string> propertyNames)
        {
            return GetProperties(obj, propertyNames, PublicInstanceMembers);
        }
#endif


        public static IEnumerable<PropertyInfo> GetPublicInstanceProperties(object obj)
        {
            return GetProperties(obj, PublicInstanceMembers);
        }


        public static PropertyInfo GetPublicInstanceProperty(object obj, string propertyName)
        {
            return GetActualType(obj).GetProperty(propertyName, PublicInstanceMembers);
        }


        /// <summary>
        /// Returns the <see cref="Type"/> for <typeparamref name="T"/>, returning the underlying type
        /// where <typeparamref name="T"/> is a <see cref="Nullable{T}"/>
        /// </summary>
        public static Type GetTypeOrUnderlyingType<T>()
        {
            return GetTypeOrUnderlyingType(typeof (T));
        }


        /// <summary>
        /// Returns the <see cref="Type"/> for <paramref name="type"/>, returning the underlying type
        /// where <paramref name="type"/> is a <see cref="Nullable{T}"/>
        /// </summary>
        public static Type GetTypeOrUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }


        /// <exception cref="InvalidOperationException">Thrown when <paramref name="obj"/> does not implement method or does not expect the <paramref name="args"/> supplied</exception>
        public static object InvokeMethod(this object obj, string methodName, params object[] args)
        {
            return DoInvokeMethod(obj,
                                  args,
                                  obj.GetType(),
                                  methodName,
                                  BindingFlags.Instance |
                                  BindingFlags.NonPublic | BindingFlags.Public);
        }


        /// <exception cref="InvalidOperationException">Thrown when <paramref name="targetType"/> does not implement method or does not expect the <paramref name="args"/> supplied</exception>
        public static object InvokeStaticMethod(this Type targetType, string methodName, params object[] args)
        {
            return DoInvokeMethod(null,
                                  args,
                                  targetType,
                                  methodName,
                                  BindingFlags.Static |
                                  BindingFlags.NonPublic | BindingFlags.Public);
        }


        private static object DoInvokeMethod(object targetObject,
                                             object[] args,
                                             Type targetType,
                                             string methodName,
                                             BindingFlags methodScope)
        {
            Type[] paremeterTypes;
            if (args != null && args.Length != 0)
            {
                if (args.Contains(null))
                    throw new ArgumentException("Cannot determine type of argument; args must not contain null", "args");
                paremeterTypes = args.Select(arg => arg.GetType()).ToArray();
            }
            else
            {
                paremeterTypes = new Type[0];
            }

            MethodInfo method = targetType.GetMethod(methodName, methodScope, null, paremeterTypes, null);
            if (method == null)
            {
                string errorMsg =
                    string.Format(
                        "Type '{0}' does not implement method '{1}' or there is a mismatch between arguments supplied and the parameters the method expects",
                        targetType.Name,
                        methodName);
                throw new InvalidOperationException(errorMsg);
            }
            return method.Invoke(targetObject, args);
        }


        public static bool IsCastleDynamicProxy(object obj)
        {
            Type type = obj.GetType();
// ReSharper disable PossibleNullReferenceException
            return NullSafe.Execute(() => type.FullName.StartsWith("Castle.Proxies.")) ||
                   ProxyNamingCriteria.IsMatch(type.Name);
// ReSharper restore PossibleNullReferenceException
        }


        public static bool IsCollectionType(this Type type)
        {
            return type != typeof (string) && typeof (IEnumerable).IsAssignableFrom(type);
        }


        public static bool IsDecoratedWithTypeConvertor<T>(Type typeToTest)
        {
            return IsDecoratedWithTypeConvertor(typeToTest, typeof (T));
        }


        public static bool IsDecoratedWithTypeConvertor(Type typeToTest, Type typeConvertor)
        {
            Type typeToInspect = GetTypeOrUnderlyingType(typeToTest);
            var converterAttribute =
                typeToInspect.GetCustomAttributes(false).OfType<TypeConverterAttribute>().SingleOrDefault();
            return converterAttribute != null &&
                   converterAttribute.ConverterTypeName == typeConvertor.AssemblyQualifiedName;
        }


        public static bool IsGenericCollectionType(this Type type)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => type).IsNotNull();
#endif

            //note: this comparing of type to a well known built in collections is a hack
            //a better implementation would be to test whether type is assignable to a generic IEnumerable<T>.
            //I think this is possible but don't have time to do this here.
            return type.IsGenericType && BuiltinCollectionTypes.Any(x => type.FullName.StartsWith(x));
        }


        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }


        /// <summary>
        /// Returns true if type supplied is numeric
        /// </summary>
        public static bool IsNumericType(Type type)
        {
            if (null == type) return false;

            if (type == typeof (byte)) return true;
            if (type == typeof (sbyte)) return true;
            if (type == typeof (decimal)) return true;
            if (type == typeof (double)) return true;
            if (type == typeof (float)) return true;
            if (type == typeof (int)) return true;
            if (type == typeof (uint)) return true;
            if (type == typeof (long)) return true;
            if (type == typeof (short)) return true;
            if (type == typeof (ushort)) return true;

            return false;
        }


        public static bool IsTypeOf<T>(this Type typeToTest)
        {
            var requiredType = GetTypeOrUnderlyingType<T>();
            return IsTypeOf(typeToTest, requiredType);
        }


        public static bool IsTypeOf(this Type typeToTest, Type requiredType)
        {
            Type actualType = GetTypeOrUnderlyingType(typeToTest);
            return requiredType.IsAssignableFrom(actualType);
        }


        /// <exception cref="ArgumentException"><paramref name="obj"/> does not implement property</exception>
        public static void SetField<T>(this T obj, string fieldName, object value)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => obj).IsNotNull();
#endif

            FieldInfo field = GetField(obj, fieldName);
            if (field == null)
            {
                throw new ArgumentException(String.Format("obj does not implement field '{0}'", fieldName), "fieldName");
            }

            field.SetValue(obj, value);
        }


        public static void SetField<T, PropertyT>(this T obj,
                                                  Expression<Func<T, PropertyT>> propertyAccessor,
                                                  object value)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => propertyAccessor).IsNotNull();
#endif

            var propertyName = GetAccessor(propertyAccessor).Name;
            var fieldName = DeriveBackingFieldName(propertyName);
            SetField(obj, fieldName, value);
        }


        /// <summary>
        /// Sets the field named <paramref name="fieldName"/>. If the field is not implemented no exception wil be thrown
        /// </summary>
        public static void SetFieldIfFound<T>(this T obj, string fieldName, object value)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => obj).IsNotNull();
#endif

            FieldInfo field = GetField(obj, fieldName);
            if (field == null) return;

            field.SetValue(obj, value);
        }


        public static void SetProperty(this object obj, string propertyName, object value)
        {
            new PropertyReference(obj, propertyName).Value = value;
        }


#if !SILVERLIGHT
        /// <summary>
        /// Assigns <paramref name="value"/> to the property named <paramref name="propertyName"/>
        /// </summary>
        public static void SetPropertyIfFound(this object obj, string propertyName, object value)
        {
            try
            {
                new PropertyReference(obj, propertyName).Value = value;
            }
            catch (ArgumentException e)
            {
                if (e.ParamName != "property") throw;
            }
        }
#endif


        public static void SetProperty<T, PropertyT>(this T obj,
                                                     Expression<Func<T, PropertyT>> propertyAccessor,
                                                     object value)
        {
            new PropertyReference(obj, GetAccessor(propertyAccessor).Name).Value = value;
        }


        public static void SetProperty(this object obj, PropertyInfo property, object value)
        {
            new PropertyReference(obj, property).Value = value;
        }


        /// <exception cref="ArgumentException"><paramref name="obj"/> does not implement property</exception>
        public static void SetPropertyBackingField<T>(this T obj, PropertyInfo property, object value)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => obj).IsNotNull();
#endif
            new PropertyReference(obj, property).SetBackingField(value);
        }


        /// <exception cref="ArgumentException"><paramref name="type"/> does not implement property</exception>
        public static void SetSaticField(this Type type, string fieldName, object value)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => type).IsNotNull();
#endif
            FieldInfo field = type.GetField(fieldName,
                                            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (field == null)
                throw new ArgumentException("type does not implement static field", "fieldName");

            field.SetValue(null, value);
        }

        #endregion


        public static string DeriveBackingFieldName(string propertyName)
        {
            string camelCaseName = propertyName[0].ToString().ToLower() + propertyName.Substring(1);
            return String.Format("_{0}", camelCaseName);
        }
    }
}