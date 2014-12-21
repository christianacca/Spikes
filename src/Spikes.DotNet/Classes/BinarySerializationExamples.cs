using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Eca.Commons.Testing;
using Eca.Spikes.SerializationVersioningEg;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class BinarySerializationExamples
    {
/*
        [Test]
        public void SerialiseVersionedSimpleObject()
        {
            using (FileStream stream = new FileStream(FilePathToSerializedOldVersion, FileMode.Create, FileAccess.Write))
            {
                IFormatter formatter = NewFormatter;
                formatter.Serialize(stream, ExampleVersionedSimpleObject);
            }
        }
*/

        #region Test helpers

        private VersionedSimpleObject ExampleVersionedSimpleObject
        {
            get { return new VersionedSimpleObject("Christian", "Crowhurst"); }
        }


        private string FilePathToSerializedOldVersion
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VersionedSimpleObject.v1"); }
        }


        private BinaryFormatter NewFormatter
        {
            get { return new BinaryFormatter(); }
        }


        private void AssertAreEquivalent<T>(T o, T deserialized)
        {
            EquivalenceComparer comparer = EquivalenceComparer.For<T>();
            Assert.That(comparer.Equals(o, deserialized), Is.True);
        }

        #endregion

        [Test]
        public void CanSerializeAndDeserializeSingleObject()
        {
            SimpleObject o = new SimpleObject("Christian", "Crowhurst");
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = NewFormatter;
            formatter.Serialize(stream, o);

            stream.Position = 0;
            SimpleObject deserialized = (SimpleObject) formatter.Deserialize(stream);
            AssertAreEquivalent(o, deserialized);
        }


        [Test]
        public void CanEfficientlySerialiseAndDeserializeObjectOmittingTransientData()
        {
            ObjectWithTransientData o = new ObjectWithTransientData(10, 20);
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = NewFormatter;
            formatter.Serialize(stream, o);

            stream.Position = 0;
            ObjectWithTransientData deserialized = (ObjectWithTransientData) formatter.Deserialize(stream);
            AssertAreEquivalent(o, deserialized);
        }


        [Test]
        public void CanByDefaultDeserialiseOldVersionOfObjectIntoNewVersionOfObject()
        {
            using (FileStream stream = new FileStream(FilePathToSerializedOldVersion, FileMode.Open, FileAccess.Read))
            {
                VersionedSimpleObject deserialized = (VersionedSimpleObject) NewFormatter.Deserialize(stream);
                AssertAreEquivalent(ExampleVersionedSimpleObject, deserialized);
            }
        }


        [Test, ExpectedException(typeof (SerializationException))]
        [Ignore("Look into why serialisation is working - this is unexpected")]
        public void CannnotDeserialiseOldVersionOfObjectWhenAssemblyVsCheckingIsEnabled()
        {
            using (
                FileStream stream =
                    new FileStream(FilePathToSerializedOldVersion, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = NewFormatter;
                formatter.AssemblyFormat = FormatterAssemblyStyle.Full;
                formatter.Deserialize(stream);
            }
        }
    }



    [Serializable]
    internal class ObjectWithTransientData : IDeserializationCallback
    {
        private readonly int _quantity;
        private readonly int _unitPrice;
        [NonSerialized] private int _total;


        public ObjectWithTransientData(int unitPrice, int quantity)
        {
            _unitPrice = unitPrice;
            _quantity = quantity;
            CalculateTotal();
        }


        public int Quantity
        {
            get { return _quantity; }
        }


        public int Total
        {
            get { return _total; }
        }

        public int UnitPrice
        {
            get { return _unitPrice; }
        }

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            CalculateTotal();
        }

        #endregion

        private void CalculateTotal()
        {
            _total = _unitPrice*_quantity;
        }
    }



    [Serializable]
    public class SimpleObject
    {
        private readonly string _firstName;
        private readonly string _lastName;


        public SimpleObject(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
        }


        public string FirstName
        {
            get { return _firstName; }
        }

        public string LastName
        {
            get { return _lastName; }
        }
    }
}