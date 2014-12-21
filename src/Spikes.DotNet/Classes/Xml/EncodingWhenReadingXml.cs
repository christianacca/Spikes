using System;
using System.IO;
using System.Text;
using System.Xml;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class EncodingWhenReadingXml
    {
        [Test]
        public void WillInferEncodingFromEmbeddedBom()
        {
            //notice that xml encoding attribute is not included in xml
            string xml = "<?xml version='1.0'?><hello><world/></hello>";
            using (XmlTextReader r = new XmlTextReader(StreamUtils.UnicodeMemoryStreamWithBom(xml)))
            {
                r.Read();
                Assert.That(r.Encoding, Is.EqualTo(Encoding.Unicode));
            }
        }


        [Test]
        public void WillInferEncodingFromXmlEncodingAttribute()
        {
            string xml = "<?xml version='1.0' encoding='US-ASCII'?><hello><world/></hello>";
            using (XmlTextReader r = new XmlTextReader(UnicodeMemoryStreamWithNoBom(xml)))
            {
                r.Read();
                Assert.That(r.Encoding, Is.EqualTo(Encoding.ASCII));
            }
        }

        [Test]
        public void WillUseXmlEncodingAttributeOverBom()
        {
            string xml = "<?xml version='1.0' encoding='US-ASCII'?><hello><world/></hello>";
            using (XmlTextReader r = new XmlTextReader(StreamUtils.UnicodeMemoryStreamWithBom(xml)))
            {
                r.Read();
                Assert.That(r.Encoding, Is.EqualTo(Encoding.ASCII));
            }
        }

        [Test]
        public void WillAssumeUtf8WhenNoEncodingAttributeAndNoBom()
        {
            string xml = "<?xml version='1.0'?><hello><world/></hello>";
            using (XmlTextReader r = new XmlTextReader(StreamUtils.AsciiMemoryStream(xml)))
            {
                r.Read();
                Assert.That(r.Encoding.CodePage, Is.EqualTo(Encoding.UTF8.CodePage));
            }
        }

        [Test]
        public void WillDetectUtf16WithoutNeedingEmbeddedBomOrXmlEncodingAttribute()
        {
            string xml = "<?xml version='1.0'?><hello><world/></hello>";
            using (XmlTextReader r = new XmlTextReader(UnicodeMemoryStreamWithNoBom(xml)))
            {
                r.Read();
                Assert.That(r.Encoding, Is.EqualTo(Encoding.Unicode));
            }
        }

        [Test]
        public void ReadingXmlWithStreamReaderCanThrowErrorsWhenReading()
        {
            string xml = "<?xml version='1.0' encoding='utf-16'?><hello><world/></hello>";
            using (StreamReader sr = new StreamReader(StreamUtils.UnicodeMemoryStreamWithBom(xml)))
            {
                //reading here causes exceptions reading xml later
                sr.ReadToEnd();
                sr.BaseStream.Position = 0;
                string output2 = sr.ReadToEnd();

                //not testing yet just clarifying assumptions
                Assert.That(output2.StartsWith(StreamUtils.BomFor(Encoding.Unicode)),
                            Is.True,
                            "Bom character output on the second read");

                sr.BaseStream.Position = 0;
                using (XmlTextReader xmlReader = new XmlTextReader(sr))
                {
                    try
                    {
                        xmlReader.Read();
                        Assert.Fail("Should have already failed");
                    }
                    catch (XmlException ex)
                    {
                        Assert.That(ex.Message, Text.StartsWith("Data at the root level is invalid"));
                    }
                }
            }
        }


        [Test]
        public void WillThrowWhenReadingXmlAssertedAsUtf8ButWrittenAsUnicode()
        {
            string xml = "<?xml version='1.0' encoding='utf-8'?><hello><world/></hello>";
            using (XmlTextReader r = new XmlTextReader(StreamUtils.UnicodeMemoryStreamWithBom(xml)))
            {
                try
                {
                    r.MoveToContent();
                    Assert.Fail("Should have already failed");
                }
                catch (XmlException ex)
                {
                    Assert.That(ex.Message, Text.StartsWith("Name cannot begin with the '.' character"));
                }
            }
        }


        private MemoryStream UnicodeMemoryStreamWithNoBom(string xml) 
        {
            return StreamUtils.UnicodeMemoryStream(xml);
        }
    }
}