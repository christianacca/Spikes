using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Eca.Commons.Logging
{
    /// <summary>
    /// Trace listener that will write messages to OutputDebugString
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can use DebugView (Dbgview) tool to view the messages output by this trace listener
    /// (to get this viewer: http://technet.microsoft.com/en-us/sysinternals/bb896647)
    /// </para>
    /// <para>
    /// Outputting messages using OutputDebugString has minimal overhead running to the running program.
    /// However, a badly written monitoring program (a program that allows you to view the messages being output)
    /// could result in slow downs in your application performance as explained in this article:
    /// http://www.grimes.demon.co.uk/workshops/InstrWSFour.htm#default 
    /// </para>
    /// <para>
    /// <strong>GOTCHA</strong>: messages output using OutputDebugString will NOT be viewable in the DebugView tool under the following circumstances:
    /// <list type="bullet">
    /// <item>When the Visual Studio debugger is attached to the process, for example when you've run up project in Visual Studio using F5</item>
    /// <item>On testing - when running the process inside of IIS 7 on a windows 64 bit machine</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class OutputDebugStringTraceListener : TraceListener
    {
        private void DoWrite(string message)
        {
            OutputDebugString(message);
        }


        public override void Write(string message)
        {
            if (message.Length <= 16384)
            {
                DoWrite(message);
            }
            else
            {
                int startIndex = 0;
                while (startIndex < (message.Length - 16384))
                {
                    DoWrite(message.Substring(startIndex, 16384));
                    startIndex += 16384;
                }
                DoWrite(message.Substring(startIndex));
            }
        }


        public override void WriteLine(string message)
        {
            Write(message + "\r\n");
        }


        #region Class Members

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern void OutputDebugString(string message);

        #endregion
    }
}