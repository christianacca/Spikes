using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Eca.Spikes.UnitTests
{
    [TestFixture]
    public class GenericClasses
    {
        [Test, Ignore("Run this test to demonstrate two type constructor's being executed")]
        public void SeparateStaticConstructorCreatedForEachConstructedType()
        {
            // this causes the static constructor for MyGenericType<string> to execute
            new MyGenericType<string>();

            //this causes the static constructor for MyGenericType<MyAbstractClass> to execute (and fail)
            var _myInt32Type = new MyGenericType<MyAbstractClass>();
        }


        [Test]
        public void StaticConstructorForAClosedTypeWillExecuteOnce()
        {
            var nonGenericClass = new NonGenericClass();
            var anotherNonGenericClass = new NonGenericClass();

            Assert.That(NonGenericClass.NumberOfExecutions, Is.EqualTo(1));
        }


        [Test]
        public void StaticClosedTypeFieldNotSharedBetweenConstructedTypes()
        {
            //MyGenericType<string> is one constructed type MyGenericType<int> is another constructed type
            MyGenericType<string>.IntField = 10;
            MyGenericType<string>.IntField = MyGenericType<string>.IntField + 7;
            MyGenericType<int>.IntField = 5;
            MyGenericType<int>.IntField = MyGenericType<int>.IntField + 6;

            Assert.That(MyGenericType<string>.IntField,
                        Is.EqualTo(17),
                        "MyGenericType<string>.IntField");
            Assert.That(MyGenericType<int>.IntField, Is.EqualTo(11), "MyGenericType<int>.IntField");
        }


        [Test]
        public void StaticOpenConstructedTypeFieldNotSharedBetweenConstructedTypes()
        {
            //MyGenericType<string> is one constructed type MyGenericType<int> is another constructed type
            MyGenericType<string>.Items.Add("hello");
            MyGenericType<string>.Items.Add("world");

            MyGenericType<int>.Items.Add(5);

            Assert.That(MyGenericType<string>.Items.Count,
                        Is.EqualTo(2),
                        "MyGenericType<string>.Items.Count");
            Assert.That(MyGenericType<int>.Items.Count, Is.EqualTo(1), "MyGenericType<int>.Items");
        }


        [Test]
        public void GenericAcceptingTypeImplementingTwoInterfacesWithIdenticalMember()
        {
            var generic
                = new GenericWithTypeImplementingTwoInterfaces<ClassImplementingInterfaces>();

            generic.CallMethod1OnTypeParameter();
        }
    }



    internal class ClassImplementingInterfaces : ITest1, ITest2
    {
        void ITest1.Member1() { }


        void ITest2.Member1() {}
    }



    internal class GenericWithTypeImplementingTwoInterfaces<T> where T : ITest1, ITest2, new()
    {
        private readonly T _instance;


        public GenericWithTypeImplementingTwoInterfaces()
        {
            _instance = new T();
        }


        public void CallMethod1OnTypeParameter()
        {
            //notice that we have to cast _instance to one of the interfaces
            //this is required because Member1() is declared in both ITest1 and ITest2
            ((ITest1)_instance).Member1();
        }
    }



    internal interface ITest2
    {
        void Member1();
    }



    internal interface ITest1
    {
        void Member1();
    }



    public abstract class MyAbstractClass {}



    public class NonGenericClass
    {
        private static readonly int _numberOfExecutions;


        public static int NumberOfExecutions
        {
            get { return _numberOfExecutions; }
        }


        static NonGenericClass()
        {
            _numberOfExecutions = _numberOfExecutions + 1;
        }
    }



    public class MyGenericType<T>
    {
        public static int IntField;
        public static List<T> Items = new List<T>();


        static MyGenericType()
        {
            if (typeof (T).IsAbstract)
                throw new InvalidOperationException(
                    "Cannot create a constructed type that has an abstract type as a parameter");
        }


        public int CountOfObjectsCreated
        {
            get { return IntField; }
        }


        public MyGenericType()
        {
            IntField = ++IntField;
        }
    }
}