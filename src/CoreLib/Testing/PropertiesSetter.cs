using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Eca.Commons.Dates;
using Eca.Commons.Reflection;

namespace Eca.Commons.Testing
{
    public class PropertiesSetter
    {
        #region Member Variables

        private readonly PropertyNameFilter _propertyFilter;

        private readonly IDictionary<Type, IDataGenerator> _registeredDataGenerators =
            new Dictionary<Type, IDataGenerator>();

        #endregion


        #region Constructors

        public PropertiesSetter(IDictionary<Type, object> values) : this(values, PropertyNameFilter.NoFilter) {}


        public PropertiesSetter(IDictionary<Type, object> values, PropertyNameFilter propertyFilter)
        {
            _propertyFilter = propertyFilter;
            _registeredDataGenerators = CreateDataGenerators(values);
        }

        #endregion


        #region Properties

        public ICollection<Type> TypesSet
        {
            get { return _registeredDataGenerators.Keys; }
        }

        #endregion


        private IDataGenerator GetRegisteredDataGeneratorFor(Type type)
        {
            if (_registeredDataGenerators.ContainsKey(type)) return _registeredDataGenerators[type];

            foreach (KeyValuePair<Type, IDataGenerator> registeredValue in _registeredDataGenerators)
            {
                if (type.IsSubclassOf(registeredValue.Key)) return registeredValue.Value;
            }
            return null;
        }


        private void SetProperty(object obj, PropertyInfo property, Type type)
        {
            IDataGenerator dataGenerator = GetRegisteredDataGeneratorFor(type);
            new PropertyReference(obj, property).Value = dataGenerator.GenerateFor(property);
        }


        public void SetPublicPropertiesOn(object obj)
        {
            SetPublicPropertiesOn(obj, false);
        }


        public void SetPublicPropertiesOn(object obj, bool includeReadonlyProperties)
        {
            foreach (PropertyInfo property in _propertyFilter.Apply(obj))
            {
                if (WillSetPropertiesOfType(property.PropertyType) && (includeReadonlyProperties || property.CanWrite))
                    SetProperty(obj, property, property.PropertyType);
            }
        }


        public object ValueForPropertiesOfType(Type type)
        {
            return _registeredDataGenerators[type];
        }


        public bool WillSetPropertiesOfType(Type type)
        {
            return GetRegisteredDataGeneratorFor(type) != null;
        }


        #region Class Members

        private static readonly PropertiesSetter _defaultSetter = new PropertiesSetter(CreateDefaultPropertyValues());

        public static PropertiesSetter DefaultValueSetter
        {
            get { return _defaultSetter; }
        }


        private static IDictionary<Type, IDataGenerator> CreateDataGenerators(IDictionary<Type, object> propertyValues)
        {
            var result = new Dictionary<Type, IDataGenerator>(propertyValues.Count);
            foreach (KeyValuePair<Type, object> entry in propertyValues)
            {
                if (entry.Value is IDataGenerator)
                    result.Add(entry.Key, (IDataGenerator) entry.Value);
                else
                    result.Add(entry.Key, DataGenenerator.CreateConstantGenerator(entry.Value));
            }
            return result;
        }


        public static IDictionary<Type, object> CreateDefaultPropertyValues()
        {
            IDictionary<Type, object> result = new Dictionary<Type, object>();

            //CRITICAL: Don't remove any types from this list. These types
            //are required in order to reliably test object CRUD functionality.

            result.Add(typeof (Boolean), true);
            result.Add(typeof (Byte), DataGenenerator.CreateRandomByte());
            result.Add(typeof (Char), Char.MaxValue);
            result.Add(typeof (DateTime), new DateTime(2000, 01, 07));
            result.Add(typeof (Date), new Date(DateTime.MaxValue));
            result.Add(typeof (DateRange), new DateRange(new Date("2000-01-07"), new Date("2009-01-07")));
            result.Add(typeof (Decimal), 100000m);
            result.Add(typeof (Double), 10000d);
            result.Add(typeof (Enum), DataGenenerator.CreateConstantGenerator(1));
            result.Add(typeof (Guid), DataGenenerator.CreateGuidGenerator());
            result.Add(typeof (Int16), 1000);
            result.Add(typeof (Int32), 10000);
            result.Add(typeof (Int64), 10000);
            result.Add(typeof (SByte), SByte.MaxValue);
            result.Add(typeof (Single), 10000f);
            result.Add(typeof (String), DataGenenerator.CreatePropertyNameGenerator());
            result.Add(typeof (UInt16), 1000);
            result.Add(typeof (UInt32), 10000);
            result.Add(typeof (UInt64), 1000);
            return result;
        }


        public static PropertiesSetter DefaultFilteredSetter(PropertyNameFilter filter)
        {
            return new PropertiesSetter(CreateDefaultPropertyValues(), filter);
        }


        public static void LoadCollectionBackingField<T>(object target, string propertyName, T[] items)
        {
            var collectionProperty = new PropertyReference(target, propertyName);

            if (!collectionProperty.HasBackingField()) return;

            FieldInfo backingField = collectionProperty.GetBackingField();

            if (backingField.FieldType.Equals(typeof (IList)))
                LoadList(items, collectionProperty);
            else if (backingField.FieldType.FullName == "Iesi.Collections.ISet")
                LoadSet(items, collectionProperty);
        }


        private static void LoadList<T>(T[] items, PropertyReference listProperty)
        {
            var backingField = (IList) listProperty.GetBackingField().GetValue(listProperty.Owner);
            foreach (T item in items)
            {
                backingField.Add(item);
            }
        }


        private static void LoadSet<T>(T[] items, PropertyReference collectionProperty)
        {
            object backingField = collectionProperty.GetBackingField().GetValue(collectionProperty.Owner);
            foreach (T item in items)
            {
                backingField.InvokeMethod("Add", item);
            }
        }

        #endregion
    }
}