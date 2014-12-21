using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Eca.Commons.Reflection
{
    /// <summary>
    /// Reference to a property on a specific object. Exposes a more feature rich interface for
    /// working with a reflected property
    /// </summary>
    /// <remarks>
    /// The main intent for this class is to be able to write code like: 
    /// <code>
    /// property.Value = "some new value"; </code> instead of
    /// <code>
    /// property.SetValue(targetObject, value, null)
    /// </code>
    /// </remarks>
    public class PropertyReference : IEquatable<PropertyReference>
    {
        #region Member Variables

        private FieldInfo _backingField;
        private readonly object _owner;
        private readonly PropertyInfo _property;

        #endregion


        #region Constructors

        public PropertyReference(object owner, PropertyInfo property)
        {
            //TODO: Throw ArgumentNull exception when property is null
            _owner = owner;
            _property = property;
        }


        /// <exception cref="ArgumentException"><paramref name="owner"/> does not implement property</exception>
        public PropertyReference(object owner, string property)
        {
            _owner = owner;
            _property = ReflectionUtil.GetInstanceProperty(_owner, property);

            if (_property == null)
            {
                throw new ArgumentException(string.Format("owner does not implement property '{0}'", property),
                                            "property");
            }
        }

        #endregion


        #region Properties

        public bool IsCollectionType
        {
            get { return Type.IsCollectionType(); }
        }


        public bool IsCustomType
        {
            get { return Type.Namespace.IndexOf("System") == -1; }
        }

        public bool IsNullableType
        {
            get { return ReflectionUtil.IsNullableType(Type); }
        }


        public string Name
        {
            get { return _property.Name; }
        }

        public object Owner
        {
            get { return _owner; }
        }

        public PropertyInfo PropertyInfo
        {
            get { return _property; }
        }

        public Type Type
        {
            get { return _property.PropertyType; }
        }

        /// <summary>
        /// Gets/sets the the underlying property
        /// </summary>
        /// <exception cref="PropertyGetException">when underlying property getter throws</exception>
        public object Value
        {
            get
            {
                if (_owner == null) return null;
                try
                {
                    return _property.GetValue(_owner, null);
                }
#if !SILVERLIGHT
                catch (TargetInvocationException e)
                {
                    throw NewPropertyGetException(e);
                }
#else
                catch (TargetInvocationException)
                {
                    throw new Exception("Property Get Exception");
                }
#endif
            }
            set
            {
                if (_owner == null) return;

                if (!_property.CanWrite)
                    DoSetBackingField(value);
                else
                    _property.SetValue(_owner, value, null);
            }
        }

        #endregion


        #region IEquatable<PropertyReference> Members

        public bool Equals(PropertyReference propertyReference)
        {
            if (propertyReference == null) return false;
            return
                ReferenceEquals(_owner, propertyReference._owner) &&
                Equals(_property.Name, propertyReference._property.Name);
        }

        #endregion


        private void DoSetBackingField(object value)
        {
            FieldInfo backingField = GetBackingField();
            if (backingField != null)
            {
                backingField.SetValue(_owner, value);
            }
        }


        private object[] GetAttributes<T>()
        {
            return GetAttributes(typeof (T));
        }


        private object[] GetAttributes(Type attributeType)
        {
            return _property.GetCustomAttributes(attributeType, false);
        }


        public FieldInfo GetBackingField()
        {
            if (_backingField == null)
            {
                string fieldName = ReflectionUtil.DeriveBackingFieldName(_property.Name);
                _backingField = ReflectionUtil.GetField(_owner, fieldName);
            }
            return _backingField;
        }


        /// <summary>
        /// TODO: Needs better handling of generic collection - should be able to determine element type even if empty
        /// </summary>
        /// <returns></returns>
        public Type GetCollectionElementType()
        {
            var list = (IEnumerable) Value;

            if (list == null) return null;

            var firstItem = new object();
            foreach (object o in list)
            {
                firstItem = o;
                break;
            }
            return ReflectionUtil.GetActualType(firstItem);
        }


        public T GetFirstAttribute<T>() where T : Attribute
        {
            if (!HasAttribute<T>()) return null;
            return GetAttributes<T>()[0] as T;
        }


        public bool HasAttribute<T>() where T : Attribute
        {
            return HasAttributeOfType(typeof (T));
        }


        public bool HasAttributeOfType(Type attributeType)
        {
            return GetAttributes(attributeType).Length > 0;
        }


        public bool HasBackingField()
        {
            return GetBackingField() != null;
        }


#if !SILVERLIGHT
        private PropertyGetException NewPropertyGetException(TargetInvocationException e)
        {
            return new PropertyGetException("Underlying property getter threw exception when read.",
                                            this,
                                            e.InnerException);
        }
#endif


        /// <exception cref="InvalidOperationException">property does not implement backing field.</exception>
        public void SetBackingField(object value)
        {
            if (!HasBackingField())
                throw new InvalidOperationException(string.Format("property {0} does not implement backing field {1}",
                                                                  _property.Name,
                                                                  ReflectionUtil.DeriveBackingFieldName(_property.Name)));

            FieldInfo backingField = GetBackingField();
            backingField.SetValue(_owner, value);
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PropertyReference);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (_owner != null ? _owner.GetHashCode() : 0) + 29*_property.GetHashCode();
            }
        }


        public override string ToString()
        {
            if (_owner == null)
                return string.Format("Owner: <Null>; Property: {0}", _property.Name);
            else
                return
                    string.Format("Owner: <{0}> [Hash{1}]; Property: {2}",
                                  _owner.GetType(),
                                  _owner.GetHashCode(),
                                  _property.Name);
        }

        #endregion


        #region Class Members

        public static IEnumerable<PropertyReference> GetInstanceProperties(object obj)
        {
            var properties = ReflectionUtil.GetInstanceProperties(obj);
            return ToPropertyReference(properties, obj);
        }


        public static IEnumerable<PropertyReference> GetInstancePropertiesDecoratedWith<TAttr>(object obj)
            where TAttr : Attribute
        {
            return GetInstancePropertiesDecoratedWithAttribute(typeof (TAttr), obj);
        }


        public static IEnumerable<PropertyReference> GetInstancePropertiesDecoratedWithAttribute(Type attributeType,
                                                                                                 object obj)
        {
            return GetInstanceProperties(obj).Where(property => property.HasAttributeOfType(attributeType));
        }


        public static IEnumerable<PropertyReference> GetPublicInstanceProperties(object obj)
        {
            IEnumerable<PropertyInfo> properties = ReflectionUtil.GetPublicInstanceProperties(obj);
            return ToPropertyReference(properties, obj);
        }


        private static IEnumerable<PropertyReference> ToPropertyReference(IEnumerable<PropertyInfo> properties,
                                                                          object obj)
        {
            return properties.Select(property => new PropertyReference(obj, property));
        }


        public static PropertyReference TryNew(object owner, string propertyName)
        {
            try
            {
                return new PropertyReference(owner, propertyName);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        #endregion
    }



    public class PropertyReference<T> : PropertyReference
    {
        #region Constructors

        /// <exception cref="InvalidCastException">where the type of the property is not derived from <typeparamref name="T"/></exception>
        public PropertyReference(object owner, string property) : base(owner, property)
        {
            if (!typeof (T).IsAssignableFrom(Type)) throw new InvalidCastException();
        }


        public PropertyReference(object owner, PropertyInfo property) : base(owner, property)
        {
            if (!typeof (T).IsAssignableFrom(Type)) throw new InvalidCastException();
        }

        #endregion


        #region Properties

        public new T Value
        {
            get
            {
                if (base.Value == null && typeof (T).IsValueType)
                    return default(T);
                else
                    return (T) base.Value;
            }
            set { base.Value = value; }
        }

        #endregion


        #region Overridden object methods

        public bool Equals(PropertyReference<T> propertyReference)
        {
            return base.Equals(propertyReference);
        }

        #endregion
    }



#if !SILVERLIGHT

    /// <summary>
    /// Exception thrown when underlying property getter, throws an exception
    /// </summary>
    [Serializable]
    public sealed class PropertyGetException : Exception, ISerializable
    {
        #region Member Variables

        private readonly PropertyReference _targetProperty;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGetException"/> class.
        /// </summary>
        public PropertyGetException() {}


        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PropertyGetException(string message) : base(message) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PropertyGetException(string message, Exception innerException) : base(message, innerException) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="targetProperty">The targetProperty.</param>
        public PropertyGetException(string message, PropertyReference targetProperty) : base(message)
        {
            _targetProperty = targetProperty;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="targetProperty">The targetProperty.</param>
        /// <param name="innerException">The inner exception.</param>
        public PropertyGetException(string message, PropertyReference targetProperty, Exception innerException)
            : base(message, innerException)
        {
            _targetProperty = targetProperty;
        }


        /// <summary>
        /// Because this class is sealed, this constructor is private. 
        /// if this class is not sealed, this constructor should be protected.
        /// </summary>
        private PropertyGetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _targetProperty = info.GetValue("TargetProperty", typeof (PropertyReference)) as PropertyReference;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get
            {
                string message = base.Message;
                if (TargetProperty != null)
                    message += string.Format("{0}TargetProperty: {{ {1} }}", Environment.NewLine, _targetProperty);
                return message;
            }
        }

        /// <summary>
        /// The property that threw the exception
        /// </summary>
        public PropertyReference TargetProperty
        {
            get { return _targetProperty; }
        }

        #endregion


        #region ISerializable Members

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/>
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TargetProperty", _targetProperty);
            GetObjectData(info, context);
        }

        #endregion
    }

#endif
}