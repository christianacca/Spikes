using System;
using System.IO;
using Eca.Commons.Testing;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    /// <summary>
    /// **WARNING** You would not be swalling exceptions like you see here if
    /// this code was demostrating how to handle stream exceptions in a class
    /// library.
    //  Also if the file was expected to be found then letting the exception
    //  bubble up would be the better thing to do rather than catch it
    /// </summary>
    /// <remarks>
    /// So long as you use <see cref="TempDir"/> and <see cref="TempFile"/> to
    /// create directories and files within your tests you can rely on <see
    /// cref="FileSystemTestsBase"/> to handle the cleanup of files
    /// </remarks>
    [TestFixture]
    public class HandlingStreamExceptionsInAWinFormApp : FileSystemTestsBase
    {
        public override void ReleaseFileLocksIfAnyHeld() {}


        [Test]
        public void ExampleExceptionHandling_GeneralPattern()
        {
            try
            {
                using (TextReader reader = new StreamReader(PathToFileContaining("Hello World")))
                {
                    //do what ever you need to do with the file content
                    Assert.That(reader.ReadToEnd(), Is.EqualTo("Hello World"));

                } // any handles on the file will be disposed of here
            }
            //We're assuming that its expected or not really a problem that the file on occassion not to be found 
            //or that the user does not have sufficient access privaleges.
            //In which case show a useful message to the user and get out
            catch (IOException ex)
            {
                Console.WriteLine("Problem opening file - {0}", ex.Message);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Insufficient permissions to access this file");
                return;
            }
        }


        private string PathToFileContaining(string contents)
        {
            return TempFile.CreateWithContents(contents).FullName;
        }


        [Test]
        public void ExampleExceptionHandling_SeparatingExceptionsWhenReading_FromExceptionsWhenOpening()
        {
            //Its probably unlikely that you would want to handle exceptions when reading from the file.
            //You'd probably want these to bubble up and be logged within the unexpected exception event handler
            //as its hard to say how you want the program to recover.

            //If you do then you need separate catch blocks as follows...

            try
            {
                using (TextReader reader = new StreamReader(PathToFileContaining("Hello World")))
                {
                    //handle exceptions when reading
                    try
                    {
                        //do what ever you need to do with the file content
                        Assert.That(reader.ReadToEnd(), Is.EqualTo("Hello World"));
                    }
                    catch 
                    {
                        Console.WriteLine("Problem reading from file");

                        //code to handle the exception would go here - like I say its hard to say why you want to handle
                        //this as its unlikely that the program can recover

                        //the only thought is that you have to rollback state changes within this class
                        //in which case you would then rethrow your exception like so:
                        throw;
                    }
                } // any handles on the file will be disposed of here
            }
            //We're assuming that its expected or not really a problem that the file on occassion not to be found 
            //or that the user does not have sufficient access privaleges.
            //In which case show a useful message to the user and get out
            catch (IOException ex)
            {
                Console.WriteLine("Problem opening file - {0}", ex.Message);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Insufficient permissions to access this file");
                return;
            }
        }
    }
}