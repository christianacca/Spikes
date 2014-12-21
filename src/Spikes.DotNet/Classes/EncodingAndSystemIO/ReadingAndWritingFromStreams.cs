using System;
using System.IO;
using System.Text;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ReadingAndWritingFromStreams : FileSystemTestsBase
    {
        private string _emptyFile;


        [SetUp]
        public override void TestInitialise()
        {
            base.TestInitialise();
            _emptyFile = TempFile.Create().FullName;
        }


        public override void ReleaseFileLocksIfAnyHeld() {}


        private string PathToFileWithContents
        {
            get { return TempFile.CreateWithContents().FullName; }
        }

        #region Writing

        [Test]
        public void CanCreateFileAndOpenFileStreamAtSameTime()
        {
            string newFile = Path.Combine(TempDir.RootPath, Path.GetRandomFileName());
            //not testing yet just clarifying assumptions
            Assert.That(File.Exists(newFile), Is.False);

            using (new FileStream(newFile, FileMode.CreateNew)) {}
            Assert.That(File.Exists(newFile), Is.True);
        }




        [Test]
        public void CanOpenFileStreamDirectlyForWriting()
        {
            string filePath = TempFile.Create().FullName;
            using (new FileStream(filePath, FileMode.Open, FileAccess.Write)) {}
        }


        [Test]
        public void CanOpenFileStreamDirectlyForReading()
        {
            string filePath = TempFile.Create().FullName;
            using (new FileStream(filePath, FileMode.Open, FileAccess.Read)) {}
        }


        [Test]
        public void FileClassProvideConvenientMethodsToWorkDirectlyWithStreams()
        {
            string filePath = TempFile.Create().FullName;
            using (FileStream openForReading = File.OpenRead(filePath))
            {
                Assert.That(openForReading.CanRead, Is.True);
                Assert.That(openForReading.CanWrite, Is.False);
            }

            using (FileStream openForWriting = File.OpenWrite(filePath))
            {
                Assert.That(openForWriting.CanRead, Is.False);
                Assert.That(openForWriting.CanWrite, Is.True);
            }

        }


        /// <summary>
        /// Use this technique to quickly write a string to a stream. The
        /// underlying backing store is not relevant, this example uses a
        /// memory stream
        /// </summary>
        [Test]
        public void CanWriteStringDirectlyToStream()
        {
            string s = "Hello World";
            Stream stream = StreamUtils.Utf8MemoryStream(s);

            Assert.That(stream.Length, Is.EqualTo(Encoding.UTF8.GetByteCount(s)), "all string bytes written to stream");
        }


        [Test]
        public void WritingStringDirectlyToStreamWillNotEmbeddBOM()
        {
            Stream stream = StreamUtils.Utf8MemoryStream("Hello World");
            Assert.That(StreamUtils.HasUnicodeBom(stream), Is.False, "BOM was not written to stream");
        }


        [Test]
        public void CanWriteStringBuilderDirectlyToStream()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Hello");
            builder.AppendLine("World");
            string s = builder.ToString();

            Stream stream = StreamUtils.Utf8MemoryStream(s);

            Assert.That(stream.Length, Is.EqualTo(Encoding.UTF8.GetByteCount(s)), "all string bytes written to stream");
        }


        [Test]
        public void CanWriteToStreamUsingStreamWriter()
        {
            Stream stream = new MemoryStream();
            using(TextWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("{1} {0}", "Hello", "World");
                writer.Flush();
                string expected = string.Concat("Hello World", Environment.NewLine);
                Assert.That(stream.Length, Is.EqualTo(Encoding.UTF8.GetByteCount(expected)));
            }
        }


        [Test]
        public void CanWriteStreamDirectlyToFile()
        {
            string streamContents = "Hello World";
            Stream stream = StreamUtils.Utf8MemoryStream(streamContents);

            //write's content out as Utf8
            File.WriteAllBytes(_emptyFile, StreamUtils.PeekToEnd(stream));
            Assert.That(File.ReadAllText(_emptyFile), Is.EqualTo(streamContents));
        }


        [Test]
        public void PositionOfBaseStreamCriticalWhenWriting()
        {
            Stream stream = StreamUtils.Utf8MemoryStream("Hello World");
            stream.Position = 6;
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write("Again");
                writer.Flush();
                stream.Position = 0;
                string contents = new StreamReader(stream).ReadToEnd();
                Assert.That(contents, Is.EqualTo("Hello Again"));
            }
        }


        [Test]
        public void MustFlushWriterBeforeContentIsWrittenToStream()
        {
            MemoryStream stream = new MemoryStream();
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.Write("Hello world");
                Assert.That(stream.Length, Is.EqualTo(0), "Conent not yet written to stream");
                writer.Flush();
                Assert.That(stream.Length > 0, Is.True, "Content now written to stream");
            }
        }


        [Test]
        public void FlushingWriterWillFlushStreamtoUnderlyingBackingStore()
        {
            using (TextWriter writer = new StreamWriter(File.Open(_emptyFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
            {
                writer.Write("Hello world");
                writer.Flush();
                using (TextReader reader = new StreamReader(File.Open(_emptyFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    Assert.That(reader.ReadToEnd().Length, Is.Not.EqualTo(0));
                }
            }
        }


        [Test]
        public void ClosingWriterWillFlushStreamToUnderlyingBackingStore()
        {
            TextWriter writer = new StreamWriter(File.OpenWrite(_emptyFile));
            writer.Write("Hello world");
            writer.Close();
            Assert.That(File.ReadAllBytes(_emptyFile).Length, Is.Not.EqualTo(0));
        }


        [Test]
        public void ClosingStreamWriterWillCloseBaseStream()
        {
            Stream stream = new MemoryStream();
            Assert.That(stream.CanWrite, Is.True, "Stream is open for writing");

            new StreamWriter(stream).Dispose();

            Assert.That(stream.CanWrite, Is.False, "Stream closed");
        }


        [Test]
        public void StreamMustBeFlushedBeforeContentIsWrittenToBackingStore()
        {
            using (FileStream stream = new FileStream(PathToFileWithContents, FileMode.Open))
            {
                long lengthBeforeWrite = stream.Length;
                stream.WriteByte(Encoding.UTF8.GetBytes("X")[0]);
                Assert.That(stream.Length, Is.EqualTo(lengthBeforeWrite));
            }
        }


        [Test]
        public void ClosingStreamWillNotFlushContentToBackingStore()
        {
            long lengthBeforeWrite;
            string filePath = PathToFileWithContents;
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                lengthBeforeWrite = stream.Length;
                stream.WriteByte(Encoding.UTF8.GetBytes("X")[0]);
            }
            Assert.That(File.ReadAllBytes(filePath).Length, Is.EqualTo(lengthBeforeWrite));
        }

        #endregion

        #region Reading


        [Test]
        public void CanReadStringFromMemoryStream()
        {
            MemoryStream stream = StreamUtils.Utf8MemoryStream("Hello World");
            string contents = Encoding.UTF8.GetString(stream.ToArray());
            Assert.That(contents, Is.EqualTo("Hello World"));
        }


        [Test]
        public void CanReadStringFromAnyStream()
        {
            Stream stream = StreamUtils.Utf8MemoryStream("Hello World");
            string contents = Encoding.UTF8.GetString(StreamUtils.PeekToEnd(stream));
            Assert.That(contents, Is.EqualTo("Hello World"));
        }


        [Test]
        public void PositionOfBaseStreamCriticalWhenReading()
        {
            Stream stream = StreamUtils.Utf8MemoryStream("Hello World");
            stream.Position = 6;
            using(TextReader reader = new StreamReader(stream))
            {
                Assert.That(reader.ReadToEnd(), Is.EqualTo("World"));
            }
        }


        [Test]
        public void ReadingMovesPositionOfStreamToNextUnreadByte()
        {
            Stream stream = StreamUtils.Utf8MemoryStream("Hello World");
            byte[] buffer = new byte[1024];

            stream.Read(buffer, 0, 2);
            Assert.That(stream.Position, Is.EqualTo(2));
        }


        [Test]
        public void ClosingStreamReaderWillCloseBaseStream()
        {
            Stream stream = new MemoryStream();
            Assert.That(stream.CanWrite, Is.True, "Stream is open for writing");

            new StreamReader(stream).Dispose();

            Assert.That(stream.CanWrite, Is.False, "Stream closed");
        }


        [Test]
        public void DefaultStreamReader_OverAnAsciiFile_WillConvertContentsToUnicode()
        {
            string asciFilePath = TempFile.CreateWithContents("Hello", Encoding.ASCII).FullName;
            using (TextReader reader = new StreamReader(asciFilePath))
            {
                //string variable line, is an array of utf16 unicode characters. 
                //The reader actually translated the file content into this utf16 character array using 
                //the Encoding object supplied (in this case this defaulted to UTF8)
                string line = reader.ReadLine();
                Assert.That(line, Is.EqualTo("Hello"));
            }
        }

        #endregion
    }
}