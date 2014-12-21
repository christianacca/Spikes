using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Eca.Commons.Extensions
{
    public static class XmlTextWriterExtensions
    {
        #region Class Members

        /// <summary>
        /// Writes out a start tag with the specified <paramref name="elementName"/>, the <paramref name="text"/> content (if any) 
        /// supplied and any <paramref name="attributes"/> supplied.
        /// </summary>
        /// <remarks>
        /// The disposable object returned should have its <see cref="IDisposable.Dispose"/> method called so that its end tag is written
        /// by the <paramref name="writer"/> 
        /// </remarks>
        /// <param name="writer"></param>
        /// <param name="elementName">The name of the element whose start tag is to be written</param>
        /// <param name="text">an object whose <see cref="Object.ToString"/> method will be called to provide the text content (optional)</param>
        /// <param name="attributes">An anonymous object whose properties will be added as attribute name/value pairs (optional)</param>
        /// <returns>A disposable object that will write the end tag for the specified <paramref name="elementName"/> when its <see cref="IDisposable.Dispose"/> method is called</returns>
        /// <example>
        /// <code>
        /// using (_xmlWriter.WriteBeginElement("fieldset", null, new {@class = "someCssClass"}))
        /// {
        ///     WriteLegendElement();
        ///     //..snip..
        /// }
        /// </code>
        /// </example>
        public static IDisposable WriteBeginElement(this XmlTextWriter writer,
                                                    XName elementName,
                                                    object text,
                                                    object attributes)
        {
            writer.WriteStartElement(elementName.ToString());
            IDictionary<string, object> attributesDictionary = attributes.SafeToDictionary() ??
                                                               new Dictionary<string, object>();
            foreach (var attribute in attributesDictionary)
            {
                writer.WriteAttributeString(attribute.Key, attribute.Value.ToString());
            }
            if (text != null)
            {
                writer.WriteString(text.ToString());
            }
            return new DisposableAction(writer.WriteEndElement);
        }


        /// <summary>
        /// Write the element, the <paramref name="text"/> content (if any) supplied and any <paramref name="attributes"/> supplied.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementName">The name of the element whose start tag is to be written</param>
        /// <param name="text">an object whose <see cref="Object.ToString"/> method will be called to provide the text content (optional)</param>
        /// <param name="attributes">An anonymous object whose properties will be added as attribute name/value pairs (optional)</param>
        public static void WriteElement(this XmlTextWriter writer, XName elementName, object text, object attributes)
        {
            writer.WriteBeginElement(elementName, text, attributes).Dispose();
        }

        /// <summary>
        /// Escapes a string to XML encoding pre to saving within an XML document
        /// Brute implementation but it works and is the fastest (tested System.Web.HttpUtility.HtmlEncode(), System.Security.SecurityElement.Escape() and System.Xml.XmlTextWriter)
        /// </summary>
        /// <param name="toXml">The string to escape</param>
        /// <returns></returns>
        public static string EscapeText(this string toXml)
        {
            return toXml.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }



        #endregion
    }
}