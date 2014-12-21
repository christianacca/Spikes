using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ComparerExamples
    {
        private List<TestObject> _testObjects;


        [Test]
        public void SortUsingDefaultComparer()
        {
            _testObjects = new List<TestObject>();

            TestObject first = new TestObject("Anthony", 41, DateTime.MinValue);
            TestObject second = new TestObject("Christian", 32, new DateTime(2000, 02, 01));
            TestObject third = new TestObject("Katie", 28, new DateTime(2003, 07, 01));

            _testObjects.Add(second);
            _testObjects.Add(third);
            _testObjects.Add(first);

            //the default comparer calls the IComparer.CompareTo(TestObject other) implemented by TestObject 
            _testObjects.Sort();

            Assert.That(_testObjects[0], Is.EqualTo(first), "first appears in slot 0");
            Assert.That(_testObjects[1], Is.EqualTo(second), "second appears in slot 1");
            Assert.That(_testObjects[2], Is.EqualTo(third), "third appears in slot 1");
        }


        [Test]
        public void SortUsingDelegate()
        {
            _testObjects = new List<TestObject>();

            TestObject third = new TestObject("Anthony", 41, DateTime.MinValue);
            TestObject second = new TestObject("Christian", 32, new DateTime(2000, 02, 01));
            TestObject first = new TestObject("Katie", 28, new DateTime(2003, 07, 01));

            _testObjects.Add(second);
            _testObjects.Add(third);
            _testObjects.Add(first);

            _testObjects.Sort(
                delegate(TestObject x, TestObject y) { return x.Age.CompareTo(y.Age); });

            Assert.That(_testObjects[0], Is.EqualTo(first), "first appears in slot 0");
            Assert.That(_testObjects[1], Is.EqualTo(second), "second appears in slot 1");
            Assert.That(_testObjects[2], Is.EqualTo(third), "third appears in slot 1");
        }

        [Test]
        public void SortUsingComparerObject()
        {
            _testObjects = new List<TestObject>();

            TestObject first = new TestObject("Anthony", 41, DateTime.MinValue);
            TestObject second = new TestObject("Christian", 32, new DateTime(2000, 02, 01));
            TestObject third = new TestObject("Katie", 28, new DateTime(2003, 07, 01));

            _testObjects.Add(second);
            _testObjects.Add(third);
            _testObjects.Add(first);

            _testObjects.Sort(new CompareTestObjectByGraduation());

            Assert.That(_testObjects[0], Is.EqualTo(first), "first appears in slot 0");
            Assert.That(_testObjects[1], Is.EqualTo(second), "second appears in slot 1");
            Assert.That(_testObjects[2], Is.EqualTo(third), "third appears in slot 1");
        }


        [Test]
        public void CaseInsenistiveStringEqualityComparison()
        {
            string s1 = "Hello";
            string ss = "hello";

            Assert.That(s1.Equals(ss, StringComparison.InvariantCultureIgnoreCase), Is.True);
        }


        [Test]
        public void UsingInsensitiveComparerForDictionaryKeys()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            //ordinarily this would result in two key/value pairs being added
            dictionary["Hello"] = "World";
            dictionary["hello"] = "Arse";

            Assert.That(dictionary.Count, Is.EqualTo(1)); //there is only one key/value pair stored
            Assert.That(dictionary.ContainsKey("hello"), Is.True);
            Assert.That(dictionary.ContainsKey("Hello"), Is.True);

            //here are the actual key/value pair values
            Dictionary<string, string>.Enumerator enumerator = dictionary.GetEnumerator();
            enumerator.MoveNext();
            Assert.That(enumerator.Current.Key, Is.EqualTo("Hello"));
            Assert.That(enumerator.Current.Value, Is.EqualTo("Arse"));
        }


        [Test]
        public void UsingDefaultComparerWithSortedDictionary()
        {
            //will use Comparer<string>.Default to sort the keys
            SortedDictionary<string, int> dictionary = new SortedDictionary<string, int>();

            dictionary.Add("B", 20);
            dictionary.Add("A", 20);
            int position = 0;
            foreach (KeyValuePair<string, int> pair in dictionary)
            {
                if (position == 0)
                    Assert.That(pair.Key, Is.EqualTo("A"));
                else
                    Assert.That(pair.Key, Is.EqualTo("B"));
                position++;
            }
        }


        [Test]
        public void UsingInsensitiveComparerForListDictionary()
        {
            ListDictionary dictionary = new ListDictionary(new CaseInsensitiveComparer(CultureInfo.InvariantCulture));

            //ordinarily this would result in two key/value pairs being added
            dictionary["Hello"] = "World";
            dictionary["hello"] = "Arse";

            Assert.That(dictionary.Count, Is.EqualTo(1)); //there is only one key/value pair stored
            Assert.That(dictionary.Contains("hello"), Is.True);
            Assert.That(dictionary.Contains("Hello"), Is.True);
        }


        [Test]
        public void UsingGenericSortedList()
        {
            SortedList<string, int> list = new SortedList<string, int>();
            list.Add("Hello", 1);
            list.Add("hello", 2);
            Assert.That(list.Values.Count, Is.EqualTo(2));
        }
    }


    class MyComparer : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }

    }



    internal class CompareTestObjectByGraduation : IComparer<TestObject> {
        public int Compare(TestObject x, TestObject y)
        {
            return x.Graduation.CompareTo(y.Graduation);
        }
    }



    [DebuggerDisplay("Name = {_name}, Age = {_age}, Graduation = {_graduation}")]
    internal class TestObject : IComparable<TestObject>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private string _name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private int _age;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private DateTime _graduation;


        public TestObject(string name, int age, DateTime graduation)
        {
            _name = name;
            _age = age;
            _graduation = graduation;
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public DateTime Graduation
        {
            get { return _graduation; }
            set { _graduation = value; }
        }


        [DebuggerStepThrough]
        public int CompareTo(TestObject other)
        {
            if (Name != other.Name) return Name.CompareTo(other.Name);
            if (Age != other.Age) return Age.CompareTo(other.Age);
            return Graduation.CompareTo(other.Graduation);
        }
    }
}