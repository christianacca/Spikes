using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eca.Commons.DomainLayer;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;

namespace Eca.Commons.Testing
{
    public abstract class Builder<TargetT, BuilderT> where BuilderT : Builder<TargetT, BuilderT>, new()
    {
        #region Member Variables

        protected Func<TargetT> _buildTargetInstance;
        private readonly HashSet<string> _propertiesExplictlySetByBuilder;
        private readonly HashSet<IEnumerable<string>> _propertiesThatCannotBeExplicitlySetTogetherByBuilder;

        #endregion


        #region Constructors

        protected Builder()
        {
            _buildTargetInstance = BuildTargetInstanceFromBuilderProperties;
            _propertiesExplictlySetByBuilder = new HashSet<string>();
            _propertiesThatCannotBeExplicitlySetTogetherByBuilder = new HashSet<IEnumerable<string>>();
        }

        #endregion


        #region Properties

        protected virtual PropertiesSetter PropertyValueGenerator
        {
            get
            {
                PropertyNameFilter propertiesNotToSet =
                    _propertiesNeverToSet.Union(PropertyNameFilter.ToExclude(_propertiesExplictlySetByBuilder));
                return PropertiesSetter.DefaultFilteredSetter(propertiesNotToSet);
            }
        }

        #endregion


        protected void AddPropertiesThatCannotBeExplicitlySetTogether(params string[] propertyNames)
        {
            _propertiesThatCannotBeExplicitlySetTogetherByBuilder.Add(propertyNames);
        }


        protected void AddPropertiesThatCannotBeExplicitlySetTogether(params PropertyInfo[] properties)
        {
            var propertyNames = properties.Select(property => property.Name).ToArray();
            AddPropertiesThatCannotBeExplicitlySetTogether(propertyNames);
        }


        protected void AddPropertyExplicitlySet(string propertyName)
        {
            _propertiesExplictlySetByBuilder.Add(propertyName);
        }


        protected void AddPropertyExplicitlySet<PropertyT>(Expression<Func<TargetT, PropertyT>> propertyAccessor)
        {
            _propertiesExplictlySetByBuilder.Add(Property(propertyAccessor).Name);
        }


        public TargetT Build()
        {
            CheckGrammer();
            return _buildTargetInstance();
        }


        /// <summary>
        /// Create an instance of <typeparamref name="TargetT"/> with property values provided by this builder
        /// </summary>
        protected abstract TargetT BuildTargetInstanceFromBuilderProperties();


        protected virtual TargetT BuildTargetInstanceFromGeneratedValues()
        {
            TargetT instance = BuildTargetInstanceFromBuilderProperties();
            PropertyValueGenerator.SetPublicPropertiesOn(instance, true);

            //validation gets in the way of verifying that values are being
            //mapped correctly to properties of an object, hence why we disable
            //validation checks by default here
            var configureableInstance = instance as ICanStopValidating;
            if (configureableInstance != null) configureableInstance.DisableValidation();

            return instance;
        }


        public void CheckGrammer()
        {
            foreach (IEnumerable<string> mutuallyExclusiveSet in _propertiesThatCannotBeExplicitlySetTogetherByBuilder)
            {
                var assignedMutuallyExclusiveProperties =
                    _propertiesExplictlySetByBuilder.Intersect(mutuallyExclusiveSet);
                if (assignedMutuallyExclusiveProperties.Count() > 1)
                {
                    throw new InvalidMethodChainGrammerException(
                        string.Format("Found properties that cannot be explicitly assigned together: {0}",
                                      assignedMutuallyExclusiveProperties.Join(", ")));
                }
            }
        }


        protected bool HasAssingedProperty<PropertyT>(Expression<Func<TargetT, PropertyT>> propertyGetter)
        {
            return HasAssingedProperty(Property(propertyGetter).Name);
        }


        protected bool HasAssingedProperty(string propertyName)
        {
            return _propertiesExplictlySetByBuilder.Contains(propertyName);
        }


        public PropertyInfo Property<PropertyT>(Expression<Func<TargetT, PropertyT>> propertyAccessor)
        {
            return ReflectionUtil.GetProperty(propertyAccessor);
        }


        protected void RemovePropertyValueIfNotExplicitlyAssigned<PropertyT>(
            Expression<Func<TargetT, PropertyT>> propertyAccessor, TargetT obj, bool setBackingFieldDirectly)
        {
            RemovePropertyValueIfNotExplicitlyAssigned(propertyAccessor,
                                                       obj,
                                                       setBackingFieldDirectly,
                                                       default(PropertyT));
        }


        protected void RemovePropertyValueIfNotExplicitlyAssigned<PropertyT>(
            Expression<Func<TargetT, PropertyT>> propertyAccessor,
            TargetT obj,
            bool setBackingFieldDirectly,
            PropertyT missingValue)
        {
            var property = Property(propertyAccessor);
            if (HasAssingedProperty(property.Name)) return;

            if (setBackingFieldDirectly)
                obj.SetPropertyBackingField(property, missingValue);
            else
                obj.SetProperty(property, missingValue);
        }


        protected void RemovePropertyValueIfNotExplicitlyAssigned<PropertyT>(
            Expression<Func<TargetT, PropertyT>> propertyAccessor, TargetT obj)
        {
            RemovePropertyValueIfNotExplicitlyAssigned(propertyAccessor, obj, false);
        }


        protected void RemovePropertyValueIfNotExplicitlyAssigned<PropertyT>(
            Expression<Func<TargetT, PropertyT>> propertyAccessor, TargetT obj, PropertyT missingValue)
        {
            RemovePropertyValueIfNotExplicitlyAssigned(propertyAccessor, obj, false, missingValue);
        }


        #region Class Members

        protected static PropertyNameFilter _propertiesNeverToSet = PropertyNameFilter
            .ToExclude("ConcurrencyId")
            .ToExclude<ICanStopValidating>(x => x.IsValidationEnabled);


        public static BuilderT Default
        {
            get { return new BuilderT(); }
        }

        public static BuilderT ForDatabaseMappingTests
        {
            get
            {
                var result = new BuilderT();
                result._buildTargetInstance = result.BuildTargetInstanceFromGeneratedValues;
                return result;
            }
        }

        #endregion


        public static implicit operator TargetT(Builder<TargetT, BuilderT> builder)
        {
            return builder.Build();
        }
    }
}