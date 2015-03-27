using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Eca.Commons.Reflection;
using NUnit.Framework;

namespace Eca.Spikes.DotNet.Classes
{
    public class FakeEnumExamples
    {
        [TestCase]
        public void CanConvertFakeEnumToKeyValuePairs()
        {
            IDictionary<int, string> table = typeof (ChildFakeEnum).ToKeyValuePairs();
            var expectedTable = new Dictionary<int, string>
            {
                {0, "Value2"},
                {1, "Value1"},
                {2, "Value3"},
            };
            Assert.That(table, Is.EquivalentTo(expectedTable));
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class ChildFakeEnum : FakeEnum
        {
            public const string Value3 = "Value3";
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class FakeEnum
        {
            public const string Value2 = "Value2";
            public const string Value1 = "Value1";
        }

        public class TestClass
        {
            [Description(FakeEnum.Value1)]
            public string GetTitle()
            {
                return "There";
            }

            [Description(ChildFakeEnum.Value3)]
            public string GetDescription()
            {
                return "Hello";
            }
        }
    }
}