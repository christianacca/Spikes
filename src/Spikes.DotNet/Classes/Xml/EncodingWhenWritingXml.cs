using System.IO;
using System.Text;
using System.Xml;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class EncodingWhenWritingXml
    {
        [Test]
        public void WillWriteToStreamUsingUtf8WithoutBomByDefault()
        {
            using (XmlTextWriter w = new XmlTextWriter(new MemoryStream(), null))
            {
                WriteHelloWorldXml(w);
                Assert.That(StreamUtils.HasUnicodeBom(w.BaseStream), Is.False);
            }
        }


        [Test]
        public void WillWriteToStreamWithoutXmlEncodingAttributeByDefault()
        {
            using (XmlTextWriter w = new XmlTextWriter(new MemoryStream(), null))
            {
                WriteHelloWorldXml(w);
                string contents = StreamUtils.PeekUtf8ToEnd(w.BaseStream);
                Assert.That(contents, Text.DoesNotContain("encoding='"));
            }
        }


        [Test]
        public void WillWriteBomToStreamWhenEncodingSupplied()
        {
            using (XmlTextWriter w = new XmlTextWriter(new MemoryStream(), Encoding.UTF8))
            {
                WriteHelloWorldXml(w);
                Assert.That(StreamUtils.GetEncoding(w.BaseStream), Is.EqualTo(Encoding.UTF8));
            }
        }


        [Test]
        public void WillWriteXmlEncodingAttributeToStreamWhenEncodingSupplied()
        {
            using (XmlTextWriter w = new XmlTextWriter(new MemoryStream(), Encoding.UTF8))
            {
                WriteHelloWorldXml(w);
                AssertHasXmlEncodingUtf8Attribute(w);
            }
        }


        [Test]
        public void CanExplicitlyStopWritingBomToStream()
        {
            using (XmlTextWriter w = new XmlTextWriter(new MemoryStream(), new UTF8Encoding(false)))
            {
                WriteHelloWorldXml(w);
                Assert.That(StreamUtils.HasUnicodeBom(w.BaseStream), Is.False);
            }
        }


        [Test]
        public void WillWriteXmlEncodingAttributeToStreamEvenWhenBomExplictlySupressed()
        {
            using (XmlTextWriter w = new XmlTextWriter(new MemoryStream(), new UTF8Encoding(false)))
            {
                WriteHelloWorldXml(w);
                AssertHasXmlEncodingUtf8Attribute(w);
            }
        }


        private void AssertHasXmlEncodingUtf8Attribute(XmlTextWriter w) 
        {
            string contents = StreamUtils.PeekUtf8All(w.BaseStream);
            Assert.That(contents, Text.Contains("encoding=\"utf-8\""));
        }


        private void WriteHelloWorldXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteElementString("hello", "world");
            writer.WriteEndDocument();
            writer.Flush();
        }

    }
}