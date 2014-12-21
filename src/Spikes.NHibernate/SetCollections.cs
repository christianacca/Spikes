using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NUnit.Framework;

namespace Eca.Spikes.NHibernate
{
    [TestFixture]
    public class SetCollections
    {
        [Test]
        public void BadForStoringMutableValueObjects()
        {
            ISet<MutableValueObject> mutableValueObjectSet = new HashedSet<MutableValueObject>();
            IList<MutableValueObject> mutableValueObjectList = new List<MutableValueObject>();

            var mutableValueObject = new MutableValueObject("Christian", 32);
            mutableValueObjectSet.Add(mutableValueObject);
            mutableValueObjectList.Add(mutableValueObject);

            Assert.That(mutableValueObjectSet.Contains(mutableValueObject),
                        Is.True,
                        "struct found in set before modification");
            Assert.That(mutableValueObjectList.Contains(mutableValueObject),
                        Is.True,
                        "struct found in list before modification");

            mutableValueObject.Age = 55;
            Assert.That(mutableValueObjectSet.Contains(mutableValueObject),
                        Is.False,
                        "struct not found in set after update");
            Assert.That(mutableValueObjectList.Contains(mutableValueObject),
                        Is.True,
                        "struct still found in list after update");
        }
    }



    internal class MutableValueObject
    {
        #region Member Variables

        private int _age;
        private string _name;

        #endregion


        #region Constructors

        public MutableValueObject(string name, int age)
        {
            _name = name;
            _age = age;
        }

        #endregion


        #region Properties

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var mutableValueObject = obj as MutableValueObject;
            if (mutableValueObject == null) return false;
            return Equals(_name, mutableValueObject._name) && _age == mutableValueObject._age;
        }


        public override int GetHashCode()
        {
            return unchecked(_name.GetHashCode() + 29*_age);
        }

        #endregion
    }
}