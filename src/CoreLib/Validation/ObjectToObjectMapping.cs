using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;
using NValidate.Framework;

namespace Eca.Commons.Validation
{
    public class ObjectToObjectMapping : IEquatable<ObjectToObjectMapping>
    {
        #region Constructors

        public ObjectToObjectMapping(Type destinationType,
                                     Type sourceType,
                                     IEnumerable<PropertyInfo> destinationPropertyChain,
                                     IEnumerable<PropertyInfo> sourceProperyChain)
            : this(
                destinationType, sourceType, destinationPropertyChain, sourceProperyChain.Safe().Join(".", m => m.Name)) {}


        public ObjectToObjectMapping(Type destinationType,
                                     Type sourceType,
                                     IEnumerable<PropertyInfo> destinationPropertyChain,
                                     string sourceProperyChain)
        {
            Check.Require(() => Demand.The.Param(() => destinationType).IsNotNull(),
                          () => Demand.The.Param(() => sourceType).IsNotNull());

            DestinationType = destinationType;
            SourceType = sourceType;

            var destinationPropertyChainList = destinationPropertyChain.SafeToList();
            DestinationPropertyName = destinationPropertyChain.Join(".", m => m.Name);
            if (destinationPropertyChainList.Any())
            {
                DestinationPropertyType = destinationPropertyChainList.Last().PropertyType;
            }

            SourceProperyName = sourceProperyChain;
        }

        #endregion


        #region Properties

        public string DestinationPropertyName { get; private set; }
        public Type DestinationPropertyType { get; private set; }
        public Type DestinationType { get; private set; }
        public string SourceProperyName { get; private set; }
        public Type SourceType { get; private set; }

        #endregion


        #region IEquatable<ObjectToObjectMapping> Members

        public bool Equals(ObjectToObjectMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.DestinationPropertyName, DestinationPropertyName) &&
                   Equals(other.DestinationType, DestinationType) && Equals(other.SourceProperyName, SourceProperyName) &&
                   Equals(other.SourceType, SourceType);
        }

        #endregion


        internal IEnumerable<string> GetSourcePropertyChainMatching(string propertyName)
        {
            return PropertyNames.GetMatchingPropertyChain(propertyName, SourceProperyName);
        }


        internal string GetSourcePropertyChainToAddToDestination(string propertyOnSource)
        {
            IEnumerable<string> propertyChainToAdd = PropertyNames.SubtractPropertyChain(propertyOnSource,
                                                                                         SourceProperyName);
            string destination = PropertyNames.ConcatPropertyChain(propertyChainToAdd);
            return destination;
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ObjectToObjectMapping)) return false;
            return Equals((ObjectToObjectMapping) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = DestinationPropertyName.GetHashCode();
                result = (result*397) ^ DestinationType.GetHashCode();
                result = (result*397) ^ SourceProperyName.GetHashCode();
                result = (result*397) ^ SourceType.GetHashCode();
                return result;
            }
        }


        public override string ToString()
        {
            return
                string.Format(
                    "{{ {0}: DestinationPropertyName = {1}, DestinationType = {2}, SourceProperyName = {3}, SourceType = {4}}}",
                    GetType().Name,
                    DestinationPropertyName,
                    DestinationType,
                    SourceProperyName,
                    SourceType);
        }

        #endregion
    }
}