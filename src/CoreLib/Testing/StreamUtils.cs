using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eca.Commons.Testing
{
    public static class StreamUtils
    {
        #region Class Members

        public static MemoryStream AsciiMemoryStream(string s)
        {
            return new MemoryStream(Encoding.ASCII.GetBytes(s));
        }


        public static string BomFor(Encoding encoding)
        {
            byte[] bomBytes = encoding.GetPreamble();
            return encoding.GetString(bomBytes);
        }


        private static bool ByteOrderMarksMatch(byte[] bomToSearch, byte[] bomToFind)
        {
            bool match = true;
            for (int i = 0; i < bomToFind.Length; ++i)
            {
                if (bomToFind[i] != bomToSearch[i])
                {
                    match = false;
                    break;
                }
            }
            return match;
        }


        public static Encoding GetEncoding(this Stream stream)
        {
            return GetEncoding(stream, false);
        }


        public static Encoding GetEncoding(this Stream stream, bool closeStream)
        {
            byte[] bom = PeekFromStart(stream, 4);

            if (closeStream) stream.Close();

            if (ByteOrderMarksMatch(bom, Encoding.UTF8.GetPreamble()))
                return Encoding.UTF8;
            else if (ByteOrderMarksMatch(bom, Encoding.Unicode.GetPreamble()))
                return Encoding.Unicode;
            else
                return Encoding.ASCII;
        }


        public static bool HasUnicodeBom(this Stream stream)
        {
            return HasUnicodeBom(stream, false);
        }


        public static bool HasUnicodeBom(this Stream stream, bool closeStream)
        {
            return (GetEncoding(stream, closeStream).GetPreamble().Length > 0);
        }


        /// <summary>
        /// Reads from <paramref name="stream"/>, bytes starting from <paramref
        /// name="start"/> to the <paramref name="length"/> specified, while
        /// maintaining the current position of the stream
        /// </summary>
        public static byte[] Peek(this Stream stream, int start, int length)
        {
            bool errored = false;
            long positionOnEntry = stream.Position;

            try
            {
                stream.Position = start;
                var buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
            catch (Exception)
            {
                errored = true;
                throw;
            }
            finally
            {
                if (!errored) stream.Position = positionOnEntry;
            }
        }


        /// <summary>
        /// Reads all bytes from <paramref name="stream"/>, while maintaining
        /// the current position of the stream
        /// </summary>
        public static byte[] PeekAll(this Stream stream)
        {
            return Peek(stream, 0, (int) stream.Length);
        }


        /// <summary>
        /// Reads the specified number of bytes from the start of <paramref
        /// name="stream"/>, while maintaining the current position of the
        /// stream
        /// </summary>
        public static byte[] PeekFromStart(this Stream stream, int length)
        {
            return Peek(stream, 0, length);
        }


        /// <summary>
        /// Reads all bytes from <paramref name="stream"/>, starting at the
        /// current position, while maintaining the current position of the
        /// stream
        /// </summary>
        public static byte[] PeekToEnd(this Stream stream)
        {
            return Peek(stream, (int) stream.Position, (int) (stream.Length - stream.Position));
        }


        public static string PeekUtf8All(this Stream stream)
        {
            return Encoding.UTF8.GetString(PeekAll(stream));
        }


        /// <summary>
        /// Returns bytes read using <see cref="PeekToEnd"/> as a string, while
        /// maintaining the current position of the stream
        /// <remarks>
        /// Assumes that <paramref name="stream"/> is <see
        /// cref="Encoding.UTF8"/> encoded
        /// </remarks>
        /// </summary>
        public static string PeekUtf8ToEnd(this Stream stream)
        {
            return Encoding.UTF8.GetString(PeekToEnd(stream));
        }


        /// <summary>
        /// Reads all bytes from the start of the <paramref name="stream"/> to the end.
        /// </summary>
        /// <remarks>The <paramref name="stream"/> supplied will be left at its end position</remarks>
        /// <seealso cref="PeekAll"/>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            byte[] readBytes = reader.ReadBytes((int) stream.Length);
            return readBytes;
        }


        public static MemoryStream UnicodeMemoryStream(string s)
        {
            return new MemoryStream(Encoding.Unicode.GetBytes(s));
        }


        public static MemoryStream UnicodeMemoryStreamWithBom(string s)
        {
            Encoding encoding = Encoding.Unicode;
            var buffer = new List<byte>(encoding.GetPreamble());
            buffer.AddRange(encoding.GetBytes(s));

            return new MemoryStream(buffer.ToArray());
        }


        public static MemoryStream Utf8MemoryStream(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        #endregion
    }
}