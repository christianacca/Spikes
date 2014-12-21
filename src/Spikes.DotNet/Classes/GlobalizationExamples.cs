using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class GlobalizationExamples
    {
        private const string CustomCultureName = "en-MS";


        #region Setup/Teardown

        [TearDown]
        public void TestCleanup()
        {
            foreach (CultureInfo customCulture in CultureInfo.GetCultures(CultureTypes.UserCustomCulture))
            {
                if (customCulture.Name == CustomCultureName)
                    CultureAndRegionInfoBuilder.Unregister(CustomCultureName);
            }
        }

        #endregion


        [Test]
        public void CannnotInstantiateRegionInfoUsingNeturalCulture()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate {
                new RegionInfo(new CultureInfo("en").LCID);
            });
            Assert.That(ex.Message, Text.Contains("is a neutral culture; a region cannot be created from it"));
        }


        [Test]
        public void CanModifyBuiltInCultureAndUseThisAsCurrentCulture()
        {
            CultureInfo builtInCulture = new CultureInfo("de-DE");
            Assert.That(builtInCulture.NumberFormat.CurrencyGroupSeparator, Is.EqualTo("."), "builtin value");

            builtInCulture.NumberFormat.CurrencyGroupSeparator = ",";
            Thread.CurrentThread.CurrentCulture = builtInCulture;

            Assert.That(Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator,
                        Is.EqualTo(","),
                        "builtin value now modified");
        }


        [Test]
        public void CultureInfoCalandarDoesNotTellTheTimeInDifferentTimeZones()
        {
            DateTime now = DateTime.Now;
            DateTime ukTime =
                CultureInfo.CurrentCulture.DateTimeFormat.Calendar.ToDateTime(now.Year,
                                                                              now.Month,
                                                                              now.Day,
                                                                              now.Hour,
                                                                              now.Minute,
                                                                              now.Second,
                                                                              now.Millisecond);
            DateTime usTime =
                new CultureInfo("en-US").DateTimeFormat.Calendar.ToDateTime(now.Year,
                                                                            now.Month,
                                                                            now.Day,
                                                                            now.Hour,
                                                                            now.Minute,
                                                                            now.Second,
                                                                            now.Millisecond);
            Assert.That(ukTime, Is.EqualTo(usTime));
        }


        [Test, Ignore]
        public void CanCreateCustomCulture()
        {
            CultureAndRegionInfoBuilder builder =
                new CultureAndRegionInfoBuilder(CustomCultureName, CultureAndRegionModifiers.None);
            builder.LoadDataFromCultureInfo(CultureInfo.GetCultureInfo("en-US"));
            builder.LoadDataFromRegionInfo(new RegionInfo("en-US"));

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = "-";
            numberFormat.CurrencySymbol = "@";

            DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
            dateTimeFormat.DateSeparator = ".";
            dateTimeFormat.DayNames =
                new string[7] {"FirstDay", "SecondDay", "ThirdDay", "ForthDay", "FifthDay", "SixthDay", "SeventhDay"};

            builder.NumberFormat = numberFormat;
            builder.GregorianDateTimeFormat = dateTimeFormat;

            builder.Register();


            Thread.CurrentThread.CurrentCulture = new CultureInfo(CustomCultureName);

            string currencyString = 10000.0m.ToString("C");
            Assert.That(currencyString, Is.EqualTo("@10,000-0000"));

            string longDatetimeString = new DateTime(2007, 10, 30).ToString("D");
            Assert.That(longDatetimeString, Is.EqualTo("ThirdDay, 30 October 2007"));

            string shortDatetimeString = new DateTime(2007, 10, 30).ToString("d");
            Assert.That(shortDatetimeString, Is.EqualTo("10.30.2007"));
        }
    }
}