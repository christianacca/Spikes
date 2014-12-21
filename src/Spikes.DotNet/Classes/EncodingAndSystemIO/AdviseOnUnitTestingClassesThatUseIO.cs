using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    /// <remarks>
    /// So long as you use <see cref="TempDir"/> and <see cref="TempFile"/> to
    /// create directories and files within your tests you can rely on <see
    /// cref="FileSystemTestsBase"/> to handle the cleanup of files
    /// </remarks>    [TestFixture]
    public class AdviseOnUnitTestingClassesThatUseIO : FileSystemTestsBase
    {
        public override void ReleaseFileLocksIfAnyHeld() {}


        [Test]
        public void PreferPassingInMemoryStreamsRatherThanFilePath()
        {
            string s = "Hello World";
            using (ClassUsingFile unit = new ClassUsingFile(StreamContaining(s)))
            {
                Assert.That(unit.GetStreamContents(), Is.EqualTo(s));
            }
        }


        [Test]
        public void ExampleOfATestSimulatingFileContainingMultipleLines()
        {
            string[] lines = ArraryContaining("Hello World", "Are you ok?");
            using (ClassUsingFile obj = new ClassUsingFile(StreamContaining(StringListBuiltFrom(lines))))
            {
                string[] actual = ArraryContaining(obj.GetStreamContentsAsLines());
                Assert.That(actual, Is.EqualTo(lines));
            }
        }


        [Test]
        public void PreferPassingInTextReaderRatherThanFilePath()
        {
            //if the Class Under Test (CUT) is expected to work against a file,
            //prefer passing in a TextReader/Writer. This allows our unit test to
            //simulate a file with just an in-memory string.
            //This assumes that the file being worked against is a text file. 
            //If this is not true then prefer simulating a file using
            //MemoryStream and passing this to the CUT (see previous examples)

            string[] lines = ArraryContaining("Hello World", "Are you ok?");
            using (ClassUsingFile obj = new ClassUsingFile(ReaderContaining(StringListBuiltFrom(lines))))
            {
                string[] actual = ArraryContaining(obj.GetStreamContentsAsLines());
                Assert.That(actual, Is.EqualTo(lines));
            }
        }


        [Test]
        public void ExampleOfHowToWriteTestThatUsesPhysicalFiles()
        {
            //strictly if a test touches the file system its not a unit test
            //(ie a unit test must be fast)!

            FileInfo file = TempFile.CreateWithContents("Hello");
            using (ClassUsingFile unit = new ClassUsingFile(file.FullName))
            {
                Assert.That(unit.GetStreamContents(), Is.EqualTo("Hello"));
            }
        }


        private TextReader ReaderContaining(string contents)
        {
            return new StringReader(contents);
        }


        public string[] ArraryContaining(params string[] lines)
        {
            return lines;
        }


        public string StringListBuiltFrom(params string[] lines)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string s in lines)
                builder.AppendLine(s);
            return builder.ToString();
        }


        public T[] ArraryContaining<T>(IEnumerable<T> lines)
        {
            return new List<T>(lines).ToArray();
        }

        public static MemoryStream StreamContaining(string s)
        {
            return StreamUtils.Utf8MemoryStream(s);
        }


        public Stream StreamContaining(StringBuilder lines)
        {
            return StreamContaining(lines.ToString());
        }


        public Stream StreamContaining<T>(IEnumerable<T> lines)
        {
            return StreamContaining(lines.ToString());
        }
    }



    /// <summary>
    /// Even though this class is supposed to work against a file, always
    /// declare a constructor that takes a <see cref="Stream"/> and/or <see
    /// cref="TextReader"/>/<see cref="TextWriter"/>
    /// <para>
    /// By declaring the stream parameter as an abstract <see cref="Stream"/>,
    /// this class can work with any of the Stream derivatives eg <see
    /// cref="FileStream"/>, <see cref="NetworkStream"/>, etc.
    /// </para>
    /// <para>
    /// By declaring the reader/writer as an abstract <see
    /// cref="TextReader"/>/<see cref="TextWriter"/>, this class can read from
    /// any of the Stream derivatives or from a simple string
    /// </para>
    /// <para>
    /// Exposing these constructors provide for an optimal unit testing
    /// experience as a file can be simulated by passing in <see
    /// cref="MemoryStream"/> or <see cref="StringReader"/>/<see
    /// cref="StringWriter"/>
    /// </para>
    /// </summary>
    internal class ClassUsingFile : IDisposable
    {
        private readonly TextReader _reader;


        public ClassUsingFile(Stream stream) : this(new StreamReader(stream)) {}


        public ClassUsingFile(TextReader reader) 
        {
            _reader = reader;
        }


        public ClassUsingFile(string filePath) : this(new StreamReader(filePath)) {}


        public string GetStreamContents()
        {
            return _reader.ReadToEnd();
        }


        public IEnumerable<string> GetStreamContentsAsLines()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
                yield return line;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _reader.Dispose();
        }

        #endregion
    }
}