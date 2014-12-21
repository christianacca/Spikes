using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eca.Commons.Testing
{
    public interface IDataGenerator
    {
        object GenerateFor(PropertyInfo property);
    }



    public static class DataGenenerator
    {
        #region Class Members

        public static IDataGenerator CreateConstantGenerator(object constantValue)
        {
            return new ConstantDataGenerator(constantValue);
        }


        public static IDataGenerator CreateGeneratorFromDelegate(Func<object> generator)
        {
            return new DelegateDataGenerator(generator);
        }


        public static IDataGenerator CreateGuidGenerator()
        {
            return new GuidDataGenerator();
        }


        public static IDataGenerator CreatePropertyNameGenerator()
        {
            return new PropertyNameDataGenerator();
        }


        public static IDataGenerator CreateRandomByte()
        {
            return new RandomIntegerDataGenerator(typeof (Byte));
        }


        public static IDataGenerator CreateRandomInt16()
        {
            return new RandomIntegerDataGenerator(typeof (Int16));
        }


        public static IDataGenerator CreateRandomInt32()
        {
            return new RandomIntegerDataGenerator(typeof (Int32));
        }


        public static IDataGenerator CreateRandomInteger(Type type)
        {
            return new RandomIntegerDataGenerator(type);
        }


        public static IDataGenerator CreateRandomString()
        {
            return new RandomStringDataGenerator();
        }


        public static IDataGenerator CreateSequentialItemGenerator<T>(IEnumerable<T> items)
        {
            return new SequentialItemGenerator<T>(items);
        }

        #endregion


        private class ConstantDataGenerator : IDataGenerator
        {
            #region Member Variables

            private readonly object _constantValue;

            #endregion


            #region Constructors

            public ConstantDataGenerator(object constantValue)
            {
                _constantValue = constantValue;
            }

            #endregion


            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo property)
            {
                return _constantValue;
            }

            #endregion
        }



        private class DelegateDataGenerator : IDataGenerator
        {
            #region Member Variables

            private readonly Func<object> _generator;

            #endregion


            #region Constructors

            public DelegateDataGenerator(Func<object> generator)
            {
                _generator = generator;
            }

            #endregion


            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo property)
            {
                return _generator();
            }

            #endregion
        }



        private class GuidDataGenerator : IDataGenerator
        {
            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo property)
            {
                return GuidGenerator.Generate();
            }

            #endregion
        }



        private class PropertyNameDataGenerator : IDataGenerator
        {
            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo propertyValue)
            {
                return propertyValue.Name;
            }

            #endregion
        }



        private class RandomIntegerDataGenerator : IDataGenerator
        {
            #region Member Variables

            private readonly Int32 _maxValue = 255;
            private readonly Int32 _minValue;
            private readonly Type _type;

            #endregion


            #region Constructors

            public RandomIntegerDataGenerator(Type type)
            {
                _type = type;

                if (type.Equals(typeof (Int32)))
                {
                    _minValue = Int32.MinValue;
                    _maxValue = Int32.MaxValue;
                }
                else if (type.Equals(typeof (Int16)))
                {
                    _minValue = Int16.MinValue;
                    _maxValue = Int16.MaxValue;
                }
                else if (type.Equals(typeof (Byte)))
                {
                    _minValue = Byte.MinValue;
                    _maxValue = Byte.MaxValue;
                }
            }

            #endregion


            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo property)
            {
                //warning: if this method is called in a tight loop then it is very likely
                //that the same number will be generated consecutive times
                //If this becomes a problem then the solution would be to wait a tick
                //between instantiating a Random and returning its value.
                //Also this method should be synchronised if it needs to be made thread-safe
                var seed = unchecked((int) DateTime.Now.Ticks);
                var random = new Random(seed);
                return Convert.ChangeType(random.Next(_minValue, _maxValue), _type);
            }

            #endregion
        }



        private class RandomStringDataGenerator : IDataGenerator
        {
            #region Member Variables

            private readonly IDataGenerator _randomInt16Generator = CreateRandomInt16();

            #endregion


            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo property)
            {
                return property.Name + _randomInt16Generator.GenerateFor(property);
            }

            #endregion
        }



        private class SequentialItemGenerator<T> : IDataGenerator
        {
            #region Member Variables

            private int _currentIndex;
            private readonly IList<T> _items;

            #endregion


            #region Constructors

            public SequentialItemGenerator(IEnumerable<T> items)
            {
                _items = new List<T>(items);
            }

            #endregion


            #region IDataGenerator Members

            public object GenerateFor(PropertyInfo property)
            {
                if (_items.Count == 0) return null;
                if (_currentIndex == _items.Count) _currentIndex = 0;
                return _items[_currentIndex++];
            }

            #endregion
        }
    }
}