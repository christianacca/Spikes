using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Eca.Commons;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ReflectionExamples
    {
        private readonly object[] NoParameters = new object[0];
        private readonly BindingFlags PrivateAndPublic = BindingFlags.NonPublic | BindingFlags.Public;


        #region Test helpers

        private Assembly GetAssemblyCallingMe()
        {
            return Assembly.GetCallingAssembly();
        }


        private static int SomeStaticFunction()
        {
            return 42;
        }

        #endregion


        [Test]
        public void DifferentWaysOfReferencingAssembly()
        {
            Assembly assemblyForThisType = Assembly.GetAssembly(GetType());
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Assert.That(assemblyForThisType, Is.EqualTo(executingAssembly), "both refer to this assembly");

            Assembly resharperAssembly = Assembly.GetCallingAssembly();
            Assert.That(resharperAssembly.GetName().Name,
                        Text.StartsWith("nunit.core"),
                        "reference to nunit.core assembly");

            Assert.That(Assembly.GetEntryAssembly(),
                        Is.Null,
                        "Not running within an exe therefore no assembly referenced");
        }


        [Test]
        public void CallingAssemblyMaybeTheSameAsTheExecutingAssembly()
        {
            Assert.That(Assembly.GetExecutingAssembly(), Is.EqualTo(GetAssemblyCallingMe()));
        }


        [Test]
        public void HowToLoadAnAssembly()
        {
            Assembly systemAssembly = Assembly.LoadFrom(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.dll");
            Console.Out.WriteLine("systemAssembly = {0}", systemAssembly.FullName);
        }


        [Test]
        public void HowToRetrieveInformationFromAssemblyAttributes()
        {
            object[] attributes =
                Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
            Assert.That(attributes, Is.Not.Empty);
            AssemblyTitleAttribute attribute = (AssemblyTitleAttribute) attributes[0];
            Assert.That(attribute.Title, Is.Not.Empty);
        }


        [Test]
        public void HowToRetrieveAssemblyVersionNumber()
        {
            string versionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Assert.That(Regex.IsMatch(versionNumber, @"\d*.\d*.\d*.\d*"), Is.True);
        }


        [Test]
        public void HowToRetrieveAllPublicMembers()
        {
            Type type = typeof (AppDomain);

            MemberInfo[] allPublicMembers = type.GetMembers();
            Assert.That(allPublicMembers,
                        Is.EqualTo(type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)));
        }


        [Test]
        public void HowToRetrieveAllMembers()
        {
            Type type = typeof (AppDomain);

            MemberInfo[] allMembers = type.GetMembers(PrivateAndPublic | BindingFlags.Instance | BindingFlags.Static);
            MemberInfo[] allPublicMembers = type.GetMembers();
            Assert.That(allMembers.Length, Is.GreaterThan(allPublicMembers.Length));
        }


        [Test]
        public void HowToRetrieveAllPublicInstanceMembers()
        {
            Type type = typeof (AppDomain);

            MemberInfo[] allPublicInstanceMembers = type.GetMembers(BindingFlags.Instance | BindingFlags.Public);
            Assert.That(allPublicInstanceMembers, Is.Not.Empty);
        }


        [Test]
        public void HowToRetrieveAllNonInheritedPublicInstanceMembers()
        {
            Type type = typeof (AppDomain);

            MemberInfo[] allPublicInstanceMembers = type.GetMembers(BindingFlags.Instance | BindingFlags.Public);

            MemberInfo[] nonInheritedPublicInstanceMembers =
                type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            Assert.That(nonInheritedPublicInstanceMembers, Is.Not.Empty);

            Assert.That(nonInheritedPublicInstanceMembers.Length, Is.LessThan(allPublicInstanceMembers.Length));
        }


        [Test]
        public void HowToRetrieve_Fields_Methods_Events()
        {
            Type type = typeof (AppDomain);

            FieldInfo privateField = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[0];
            Assert.That(privateField, Is.Not.Null);

            PropertyInfo publicProperty = type.GetProperties()[0];
            Assert.That(publicProperty, Is.Not.Null);

            MethodInfo publicMethod = type.GetMethods()[0];
            Assert.That(publicMethod, Is.Not.Null);

            MethodInfo privateMethod = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)[0];
            Assert.That(privateMethod, Is.Not.Null);

            MethodInfo publicStaticMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)[0];
            Assert.That(publicStaticMethod, Is.Not.Null);

            EventInfo publicEvent = type.GetEvents()[0];
            Assert.That(publicEvent, Is.Not.Null);
        }


        [Test]
        public void HowToRetrievePrivateFieldFromBaseType()
        {
            Type childType = typeof (ChildClass);

            FieldInfo privateField = childType.BaseType.GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(privateField, Is.Not.Null);
        }


        [Test, Ignore("Receiving BadImageFormatException ")]
        public void HowToDynamicallyCreateAnInstance_UsingContructorInfo()
        {
            string path = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll";
            Assembly assemblyContainingHashTable = Assembly.LoadFrom(path);

            Type hashTableType = assemblyContainingHashTable.GetType("System.Collections.Hashtable");
            ConstructorInfo defaultContructor = hashTableType.GetConstructor(Type.EmptyTypes);

            object instance = defaultContructor.Invoke(NoParameters);
            Assert.That(instance, Is.Not.Null);
        }


        [Test]
        public void HowToExecuteInstanceMethodDynamically()
        {
            MethodInfo method = GetType().GetMethod("GetAssemblyCallingMe", BindingFlags.Instance | PrivateAndPublic);
            object result = method.Invoke(this, null);
            Assert.That(result, Is.EqualTo(Assembly.GetExecutingAssembly()));
        }


        [Test]
        public void HowToExecuteStaticMethodDynamically()
        {
            MethodInfo staticMethod = GetType().GetMethod("SomeStaticFunction", BindingFlags.Static | PrivateAndPublic);
            int result = (int) staticMethod.Invoke(null, NoParameters);
            Assert.That(result, Is.EqualTo(SomeStaticFunction()));
        }


        [Test]
        public void HowToReferenceTypeInAnotherAssembly()
        {
            string fullQualifedTypeName =
                "Eca.Spikes.WinFormsApplication.ReflectionFriendlyLauncher, Eca.Spikes.WinFormsApplication";
            Type type = Type.GetType(fullQualifedTypeName);
            Assert.That(type, Is.Not.Null);
        }



        internal class ChildClass : ParentClass {}



        [SkipFormatting]
        internal class ParentClass
        {
#pragma warning disable 169
            private int _id;
#pragma warning restore 169
        }
    }
}