/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * This code was borrowed from this article:  http://www.bestbrains.dk/dansk.aspx/Artikler/Using_Files_In_Unit_Tests
 * I have sent an email to the owner for their authorisation.
 * The had no license or copyright notice.
 * 
 * Getting Started
 * 1. Setup Visual Studio to output an XML documentation file (Project -> Properties -> Build -> "XML documentation file")
 * 2. Suppress warning '1591' to stop the compiler from warning you that every method should have an XML summary
 * 3. Start writing unit tests!
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Eca.Commons.Testing
{
    public class CommentReader
    {
        #region Class Members

        public static string GetElement(string elementName)
        {
            var stackTrace = new StackTrace();
            MethodBase callingMethod = stackTrace.GetFrame(1).GetMethod();
            return GetElement(elementName, callingMethod);
        }


        public static string GetElement(string elementName, MethodBase callingMethod)
        {
            Assembly callingAssembly = callingMethod.DeclaringType.Assembly;

            var generatedDocumentation = new XmlDocument();
            generatedDocumentation.Load(String.Format("{0}.xml",
                                                      callingAssembly.FullName.Split(',')[0]));

            XmlNode node = generatedDocumentation.SelectSingleNode(
                String.Format("doc/members/member[contains(@name, '{0}.{1}')]/{2}",
                              callingMethod.DeclaringType,
                              callingMethod.Name,
                              elementName));

            if (node != null)
                return HttpUtility.HtmlDecode(node.InnerXml);
            else
                throw new ArgumentOutOfRangeException("elementName", elementName, "Element not found.");
        }

        #endregion
    }
}