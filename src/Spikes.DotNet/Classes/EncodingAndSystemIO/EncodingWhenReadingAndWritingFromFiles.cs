using System.IO;
using System.Text;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    /// <summary>
    /// <seealso cref="ReadingAndWritingFromStreams.WritingStringDirectlyToStreamWillNotEmbeddBOM"/>
    /// </summary>
    [TestFixture]
    public class EncodingWhenReadingAndWritingFromFiles : FileSystemTestsBase
    {
        private const bool UseEncodingDetectedFromBOM = true;
        private const string _fileContents = "Please delete this file";
        private string _utf16EncodedFile;
        private string _emptyFile;


        [SetUp]
        public override void TestInitialise()
        {
            base.TestInitialise();
            _utf16EncodedFile = TempFile.CreateWithContents(_fileContents, Encoding.Unicode).FullName;
            _emptyFile = TempFile.Create().FullName;
        }


        public override void ReleaseFileLocksIfAnyHeld() {}


        #region Reading

        [Test]
        public void CanConvertAcsiiBytesIntoUtf8()
        {
            byte[] aciiBytes = Encoding.ASCII.GetBytes("Hello World");
            string convertedString = Encoding.UTF8.GetString(aciiBytes);
            Assert.That(convertedString, Is.EqualTo("Hello World"));
        }


        [Test]
        public void MaybeAbleToConvertUtf8BytesIntoAscii()
        {
            byte[] utf8Bytes_FromAsciiChars = Encoding.UTF8.GetBytes("Hello World");
            string convertedString = Encoding.ASCII.GetString(utf8Bytes_FromAsciiChars);
            Assert.That(convertedString, Is.EqualTo("Hello World"));

            const string nonAsciiChar = "É";
            byte[] utf8Bytes_NonAsciiChars = Encoding.UTF8.GetBytes(nonAsciiChar);
            convertedString = Encoding.ASCII.GetString(utf8Bytes_NonAsciiChars);
            Assert.That(convertedString, Is.Not.EqualTo(nonAsciiChar));
        }


        [Test]
        public void CannotConvertUtf16BytesIntoAscii()
        {
            const string textToConvert = "Hello World";
            byte[] utf16Bytes_FromAsciChars = Encoding.Unicode.GetBytes(textToConvert);
            string convertedString = Encoding.ASCII.GetString(utf16Bytes_FromAsciChars);
            Assert.That(convertedString, Is.Not.EqualTo(textToConvert), "Can convert Asci chars");

            const string nonAsciiChar = "É";
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(nonAsciiChar);
            convertedString = Encoding.ASCII.GetString(utf16Bytes);
            Assert.That(convertedString, Is.Not.EqualTo(nonAsciiChar), "Cannot conver non-Ascii chars");
        }

        
        [Test]
        public void StreamReader_MustReadFileBeforeEncodingIsDetected()
        {
            using (StreamReader reader = new StreamReader(File.OpenRead(_utf16EncodedFile), UseEncodingDetectedFromBOM))
            {
                Assert.That(reader.CurrentEncoding, Is.Not.EqualTo(Encoding.Unicode));

                //reading the contents gives the reader an opportunity to detect the BOM from the file being read
                reader.ReadLine();
                Assert.That(reader.CurrentEncoding, Is.EqualTo(Encoding.Unicode));
            }
        }


        [Test]
        public void StreamReader_SuppliedEncodingWillBeOverriddenByDetectedEncoding()
        {
            using (StreamReader reader = new StreamReader(_utf16EncodedFile, Encoding.UTF32, UseEncodingDetectedFromBOM))
            {
                reader.ReadToEnd();
                Assert.That(reader.CurrentEncoding, Is.EqualTo(Encoding.Unicode));
            }
        }


        [Test]
        public void StreamReader_GivenPathOnly_AssumesFileUTF8_AndWillDetectEncodingFromFile()
        {
            using (StreamReader reader = new StreamReader(_utf16EncodedFile))
            {
                VerifyReaderAssumesFileUTF8_ButWillDetectEncodingFromFile(reader);
            }
        }


        [Test]
        public void File_OpenText_ReturnsReaderThatAssumesFileUTF8_ButWillDetectEncodingFromFile()
        {
            using (StreamReader reader = File.OpenText(_utf16EncodedFile))
            {
                VerifyReaderAssumesFileUTF8_ButWillDetectEncodingFromFile(reader);
            }
        }


        [Test]
        public void File_ReadAllText_WillAssumeFileUTF8_ButWillDetectEncodingFromFile()
        {
            string text = File.ReadAllText(_utf16EncodedFile);
            Assert.That(text, Is.EqualTo(_fileContents));
        }


        [Test]
        public void File_ReadAllText_SuppliedEncodingWillBeOverriddenByDetectedEncoding()
        {
            string text = File.ReadAllText(_utf16EncodedFile, Encoding.ASCII);
            Assert.That(text, Is.EqualTo(_fileContents));
        }


        [Test]
        public void Gotcha_StreamReader_OnSecondReadWillWriteOutBomEmbeddedInStream()
        {
            string bom = StreamUtils.BomFor(Encoding.Unicode);
            using (StreamReader reader = new StreamReader(StreamUtils.UnicodeMemoryStreamWithBom("Hello World")))
            {
                Assert.That(reader.ReadToEnd().Contains(bom), Is.False);
                reader.BaseStream.Position = 0;
                Assert.That(reader.ReadToEnd().Contains(bom), Is.True);
            }
        }


        private void VerifyReaderAssumesFileUTF8_ButWillDetectEncodingFromFile(StreamReader reader)
        {
            Assert.That(reader.CurrentEncoding, Is.EqualTo(Encoding.UTF8));
            reader.ReadLine();
            Assert.That(reader.CurrentEncoding, Is.EqualTo(Encoding.Unicode));
        }

        #endregion


        #region Writing


        [Test]
        public void StreamWriter_ByDefaultWillWriteUTF8_WithNoBOM()
        {
            using (TextWriter writer = new StreamWriter(_emptyFile))
            {
                writer.Write("Hello world");
            }
            Assert.That(StreamUtils.HasUnicodeBom(File.OpenRead(_emptyFile), true), Is.False);
        }


        [Test]
        public void File_OpenWrite_ReturnsWriterThatWillWriteUTF8_WithNoBOM()
        {
            using (TextWriter writer = File.CreateText(_emptyFile))
            {
                writer.Write("Hello world");
            }
            Assert.That(StreamUtils.HasUnicodeBom(File.OpenRead(_emptyFile), true), Is.False);
        }


        [Test]
        public void File_WriteAllText_WillWriteUTF8_WithNoBOM()
        {
            File.WriteAllText(_emptyFile, "Hello World");
            Assert.That(StreamUtils.HasUnicodeBom(File.OpenRead(_emptyFile), true), Is.False);
        }


        [Test]
        public void StreamWriter_WillWriteOutBOMWhenEncodingExplicitlySupplied()
        {
            using (TextWriter writer = new StreamWriter(_emptyFile, false, Encoding.UTF8))
            {
                writer.Write("Hello world");
            }
            Assert.That(StreamUtils.HasUnicodeBom(File.OpenRead(_emptyFile), true), Is.True);
        }


        [Test]
        public void File_WriteAllText_WillWriteOutBOMWhenEncodingExplicitlySupplied()
        {
            File.WriteAllText(_emptyFile, "Hello World", Encoding.UTF8);
            Assert.That(StreamUtils.HasUnicodeBom(File.OpenRead(_emptyFile), true), Is.True);
        }


        [Test]
        public void StreamWriter_CanSupplyEncodingThatWillNotWriteOutBOM()
        {
            //you may want to do this to be consistent with the behave of File.CreateText / File.AppendText
            using (TextWriter writer = new StreamWriter(_emptyFile, false, new UTF8Encoding(false)))
            {
                writer.Write("Hello world");
            }
            Assert.That(StreamUtils.HasUnicodeBom(File.OpenRead(_emptyFile), true), Is.False);
        }

        #endregion
    }
}