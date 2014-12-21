using System;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ReflectionEmitExamples
    {
        [Test]
        public void HowToCreateEmptyAssembly()
        {
            AssemblyName assemblyName = new AssemblyName("TempDynamicAssembly");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            assemblyBuilder.DefineDynamicModule("MainModule");
            assemblyBuilder.Save("TempDynamicAssembly.dll");

            Assembly dynamicAssembly = Assembly.Load(assemblyName);
            Assert.That(dynamicAssembly, Is.Not.Null);
            Assert.That(dynamicAssembly.GetName().Name, Is.EqualTo("TempDynamicAssembly"));
        }


        [Test]
        public void HowToCreateTypeWithVoidMethod()
        {
            AssemblyName assemblyName = new AssemblyName("TempDynamicAssemblyWithSingleType");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName,
                                                              AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder =
                assemblyBuilder.DefineDynamicModule("MainModule", "TempDynamicAssemblyWithSingleType.dll");
            TypeBuilder classBuilder =
                moduleBuilder.DefineType("Eca.TempDynamicAssemblyWithSingleType.PublicClass",
                                         TypeAttributes.Class | TypeAttributes.Public);

            MethodBuilder voidInstanceMethodBuilder =
                classBuilder.DefineMethod("VoidMethod",
                                          MethodAttributes.Public,
                                          CallingConventions.HasThis,
                                          null,
                                          Parameters(typeof (string)));
            voidInstanceMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);

            classBuilder.CreateType();
            assemblyBuilder.Save("TempDynamicAssemblyWithSingleType.dll");

            Assembly dynamicAssembly = Assembly.Load(assemblyName);
            Type dynamicClass = dynamicAssembly.GetType("Eca.TempDynamicAssemblyWithSingleType.PublicClass");
            Assert.That(dynamicClass, Is.Not.Null);

            MethodInfo dynamicMethod = dynamicClass.GetMethod("VoidMethod", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(dynamicMethod.GetParameters().Length, Is.EqualTo(1));
            Assert.That(dynamicMethod.GetParameters()[0].ParameterType, Is.EqualTo(typeof (string)));
        }


        private Type[] Parameters(params Type[] types)
        {
            return types;
        }
    }
}