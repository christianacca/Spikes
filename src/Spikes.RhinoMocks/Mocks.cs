using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace Eca.Spikes.RhinoMocks
{
    [TestFixture]
    public class Mocks
    {
        private MockRepository _mocks;


        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
        }

        #endregion


        [Test]
        public void SetupResultAndExpectationOnTheSameMethodDoesNotWork()
        {
            var mock = _mocks.StrictMock<ClassBeingMocked>();

            var obj = new object();
            Expect.Call(mock.SomeMethod()).Return(obj);
            SetupResult.For(mock.SomeMethod()).Return(obj);
            _mocks.ReplayAll();

            //this should be satisfying the expectation set above; the problem here
            //is the SetupResult seems to "hide" the fact we are satisfying the expectations
            mock.SomeMethod();

            string msg = "ClassBeingMocked.SomeMethod(); Expected #1, Actual #0.";
            var ex = Assert.Throws<ExpectationViolationException>(_mocks.VerifyAll);
            Assert.That(ex.Message, Is.EqualTo(msg));
        }


        [Test]
        public void SupplyingAnUnexpectedParameter_StrictMockWillFailFast()
        {
            var mocked = _mocks.StrictMock<ClassBeingMocked>();

            mocked.MethodWithParameters(5);
            _mocks.ReplayAll();

            string msg = "ClassBeingMocked.MethodWithParameters(6); Expected #0, Actual #1.";
            var ex = Assert.Throws<ExpectationViolationException>(() => mocked.MethodWithParameters(6));
            Assert.That(ex.Message, Text.StartsWith(msg));
        }


        [Test]
        public void SupplyingAnUnexpectedParameter_DynamicMockWillFailAtVerifyAll()
        {
            var mocked = _mocks.DynamicMock<ClassBeingMocked>();

            mocked.MethodWithParameters(5);
            _mocks.ReplayAll();

            mocked.MethodWithParameters(6);

            string msg = "ClassBeingMocked.MethodWithParameters(5); Expected #1, Actual #0.";
            var ex = Assert.Throws<ExpectationViolationException>(_mocks.VerifyAll);
            Assert.That(ex.Message, Is.EqualTo(msg));
        }


        [Test]
        public void HowToVerifyParameterUsingCustomDelegate()
        {
            //setup mock and real object
            var myMock = _mocks.DynamicMock<ClassBeingMocked>();
            var testObject = new MyRealClass(myMock);

            //record expectations
            int whatever = 100;
            ValidateIntParameter validationCriteria = delegate(int i) {
                return i == 10;
            };
            Expect.Call(myMock.IntMethodWithParameters(0))
                .Callback(validationCriteria)
                .Return(whatever);
            _mocks.ReplayAll();

            //exercise the real object
            testObject.InteractWithMock();

            //verify that the real object interacted with mock as expected
            _mocks.VerifyAll();
        }



        public delegate bool ValidateIntParameter(int i);
    }



    internal class MyRealClass
    {
        #region Member Variables

        private readonly ClassBeingMocked _mock;

        #endregion


        #region Constructors

        public MyRealClass(ClassBeingMocked mock)
        {
            _mock = mock;
        }

        #endregion


        #region Methods: Public

        public void InteractWithMock()
        {
            int result = _mock.IntMethodWithParameters(10);
            Console.Out.WriteLine("result = {0}", result);
        }

        #endregion
    }



    public class ClassBeingMocked
    {
        #region Methods: Public

        public virtual int IntMethodWithParameters(int i)
        {
            return 0;
        }


        public virtual void MethodWithParameters(int parameter) {}


        public virtual object SomeMethod()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}