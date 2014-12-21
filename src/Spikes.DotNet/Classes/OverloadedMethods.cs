using NUnit.Framework;

namespace Eca.Spikes.UnitTests
{
    [TestFixture]
    public class OverloadedMethods
    {
        [Test]
        public void RuntimeTimeTypeDoesNotDetermineWhichOverloadToExcecute()
        {
            BaseClass baseClassReferenceToSubclass = new Subclass();

            Assert.That(OverloadedMethod(baseClassReferenceToSubclass),
                        Is.EqualTo("Executed: OverloadedMethod(BaseClass)"));
        }


        [Test]
        public void CompileTimeTypeDeterminesWhichOverloadToExcecute()
        {
            Subclass subclassReference = new Subclass();

            Assert.That(OverloadedMethod(subclassReference),
                        Is.EqualTo("Executed: OverloadedMethod(Subclass)"));
        }


        [Test]
        public void OverloadThatTakesBaseClassChoosenOverMethodThatTakesObject()
        {
            Subclass subclassReference = new Subclass();

            Assert.That(MethodTakingObjectOrBaseClass(subclassReference),
                        Is.EqualTo("Executed: MethodTakingObjectOrBaseClass(BaseClass)"));
        }



        public class BaseClass {}



        public class Subclass : BaseClass {}



        public string MethodTakingObjectOrBaseClass(object obj)
        {
            return "Executed: MethodTakingObjectOrBaseClass(Object)";
        }


        public string MethodTakingObjectOrBaseClass(BaseClass baseClass)
        {
            return "Executed: MethodTakingObjectOrBaseClass(BaseClass)";
        }


        public string OverloadedMethod(BaseClass baseClass)
        {
            return "Executed: OverloadedMethod(BaseClass)";
        }


        public string OverloadedMethod(Subclass subclass)
        {
            return "Executed: OverloadedMethod(Subclass)";
        }
    }
}