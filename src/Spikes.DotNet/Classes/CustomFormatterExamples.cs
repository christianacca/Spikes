using System;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class CustomFormatterExamples
    {
        [Test]
        public void CanUseCustomFormatterWithStringBuilder()
        {
            PersonFormatter fmt = new PersonFormatter();
            fmt.OutputName = true;

            Person p = new Person("Christian Crowhurst", DateTime.Parse("1974-06-25 08:00:00"));
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(fmt, "{0}", p);
            Assert.AreEqual("Christian Crowhurst", sb.ToString(), "Name of person only ouput");
        }


        [Test]
        public void CanUseCustomFormatterWithStringFormatMethod()
        {
            PersonFormatter fmt = new PersonFormatter();
            fmt.OutputName = true;

            Person p = new Person("Christian Crowhurst", DateTime.Parse("1974-06-25 08:00:00"));
            Assert.AreEqual("Christian Crowhurst", string.Format(fmt, "{0}", p), "Name of person only ouput");
        }


        [Test]
        public void CanFormatBothPersonAndPrimitiveTypes()
        {
            PersonFormatter fmt = new PersonFormatter();
            fmt.OutputName = true;

            Person p = new Person("Christian Crowhurst", DateTime.Parse("1974-06-25 08:00:00"));
            string actual = string.Format(fmt, "{0} as output on {1}", p, DateTime.Now);
            string expected = string.Format("Christian Crowhurst as output on {0}", DateTime.Now);
            Assert.AreEqual(expected, actual, "both types output correctly");
        }
    }



    public class PersonFormatter : IFormatProvider, ICustomFormatter
    {
        public bool OutputDob = false;
        public bool OutputName = false;


        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            Person p = arg as Person;
            if (p == null)
            {
                IFormattable formattable = arg as IFormattable;
                if (formattable == null)
                    return arg.ToString();
                else
                    return formattable.ToString(format, formatProvider);
            }
            else
            {
                string output = string.Empty;
                if (OutputName)
                    output = p.Name;
                if (OutputDob)
                    output += " " + p.DateOfBirth.ToString(formatProvider);

                return output.Trim();
            }
        }

        #endregion


        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof (ICustomFormatter))
                return this;
            else
                return Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
        }

        #endregion
    }



    public class Person
    {
        public DateTime DateOfBirth = DateTime.MinValue;
        public String Name = String.Empty;


        public Person(String name, DateTime dob)
        {
            Name = name;
            DateOfBirth = dob;
        }
    }
}