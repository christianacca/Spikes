using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Eca.Spikes.RhinoMocks
{
    [TestFixture]
    public class Stubs
    {
        private MockRepository _mocks;


        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
        }

        [Test]
        public void DoNotNeedToSetupResultForProperty()
        {
            TestObject stub = _mocks.Stub<TestObject>();

            Assert.That(stub.IntProperty, Is.EqualTo(0));

            Assert.That(stub.StringProperty, Is.Null);
        }


        [Test]
        public void PropertiesBehaveLikeProperties()
        {
            TestObject stub = _mocks.Stub<TestObject>();

            stub.IntProperty = 5;
            Assert.That(stub.IntProperty, Is.EqualTo(5));

            stub.StringProperty = "Hello";
            Assert.That(stub.StringProperty, Is.EqualTo("Hello"));
        }


        [Test]
        public void NonVirtualMethodsWillNotBeMocked()
        {
            var stub = _mocks.Stub<TestObject>();

            Assert.That(stub.NonVirtualMethodReturningNewObject().Name, Is.EqualTo("Christian"));
        }

        [Test]
        public void VirtualMethodsDoNotNeedExplicitSetup()
        {
            var stub = _mocks.Stub<TestObject>();

            Assert.That(stub.VirtualMethodReturningNewObject(),
                        Is.Not.Null,
                        "This is new behaviour - to return an object even though we do not setup a result");
        }


        [Test]
        public void CanSetupResultsForVirtualMethods()
        {
            TestObject stub = _mocks.Stub<TestObject>();

            OtherTestObject objectToReturn = new OtherTestObject();
            SetupResult.For(stub.VirtualMethodReturningNewObject()).Return(objectToReturn);

            //you need to explicity indicate that record mode is now over when you want
            //to explictly return something from stubbed methods
            _mocks.ReplayAll();

            Assert.That(stub.VirtualMethodReturningNewObject(), Is.EqualTo(objectToReturn));
        }


        [Test]
        public void CannotSetupResultForNonVirtualMethod()
        {
            TestObject stub = _mocks.Stub<TestObject>();

            Assert.Throws<InvalidOperationException>(delegate {
                SetupResult.For(stub.NonVirtualMethodReturningNewObject())
                    .Return(new OtherTestObject());
            });
        }

        [Test]
        public void UnsatisifedExpectationsWillNotFailTest()
        {
            TestObject stub = _mocks.Stub<TestObject>();

            Expect.Call(stub.VirtualMethodReturningNewObject()).Return(new OtherTestObject());
            _mocks.ReplayAll();
            _mocks.VerifyAll(); //notice that this does not throw exception
        }

    }

    public class TestObject
    {
        private int _intProperty;
        private string _stringProperty;

        public virtual int IntProperty
        {
            get { return _intProperty; }
            set { _intProperty = value; }
        }

        public virtual string StringProperty
        {
            get { return _stringProperty; }
            set { _stringProperty = value; }
        }


        public OtherTestObject NonVirtualMethodReturningNewObject()
        {
            return new OtherTestObject { Name = "Christian" };
        }


        public virtual OtherTestObject VirtualMethodReturningNewObject()
        {
            return new OtherTestObject{ Name = "Christian" };
        }
    }

    public class OtherTestObject {
        public string Name { get; set; }
    }
}