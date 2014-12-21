using System;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ConvertingBetweenCustomTypes
    {
        [Test]
        public void CanImplicitlyConvertBetweenTypesWhenSupported()
        {
            SourceObject source = new SourceObject();
            source.Surname = "Crowhurst";

            //This works because of the implicit conversion operator declared by TargetObject that takes a SourceObject
            //and returns a TargetObject
            //Be aware that the operator could equally have been declared by SourceObject but not by both. 
            //Also be aware that conversion operators are directional. For example the following would not compile
            //SourceObject someOtherSource = new TargetObject("Crowhurst");
            //This is because there is no implcit operator that takes a TargetObject and return a SourceObject
            TargetObject target = source;

            Assert.That(target.FamilyName, Is.EqualTo("Crowhurst"));
        }


        [Test]
        public void CanExplicityConvertBetweenTypesWhenSupported()
        {
            TargetObject target = new TargetObject("Crowhurst");

            //This works because of the explicit conversion operator declared by TargetObject that takes a TargetObject
            //and returns a SourceObject
            SourceObject source = (SourceObject) target;

            Assert.That(source.Surname, Is.EqualTo("Crowhurst"));
        }


        [Test]
        public void CanExplicitlyConvertBetweenCustomTypeAndInt32Using_Convert_ToInt32()
        {
            Int32 converted = Convert.ToInt32(new Money(10, "GBP"));
            Assert.That(converted, Is.EqualTo(10));
        }


        [Test]
        public void CanExplicitlyConvertBetweenCustomTypeAndInt32Using_Convert_ChangeType()
        {
            Money money = new Money(10, "GBP");

            object converted = Convert.ChangeType(money, TypeCode.Int32);
            Assert.That(converted, Is.InstanceOf<Int32>(), "converted to an Int32");
            Assert.That(converted, Is.EqualTo(10), "returns 10");
        }
    }



    internal class Money : IConvertible
    {
        private readonly decimal _value;
        private readonly string _currency;


        public decimal Value
        {
            get { return _value; }
        }

        public string Currency
        {
            get { return _currency; }
        }


        public Money(decimal value, string currency)
        {
            _value = value;
            _currency = currency;
        }

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }


        public bool ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public char ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public byte ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public short ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public int ToInt32(IFormatProvider provider)
        {
            return (int) _value;
        }


        public uint ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public long ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public float ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public double ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public string ToString(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }


        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(Int32))
                return ToInt32(provider);
            else
                throw new InvalidCastException();
        }

        #endregion
    }



    internal class TargetObject
    {
        public TargetObject(string familyName)
        {
            _familyName = familyName;
        }


        private string _familyName;

        public string FamilyName
        {
            get { return _familyName; }
            set { _familyName = value; }
        }

        public static implicit operator TargetObject(SourceObject arg)
        {
            return new TargetObject(arg.Surname);
        }

        public static explicit operator SourceObject(TargetObject arg)
        {
            SourceObject result = new SourceObject();
            result.Surname = arg.FamilyName;
            return result;
        }
    }



    internal class SourceObject
    {
        private string _surname;

        public string Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }
    }
}