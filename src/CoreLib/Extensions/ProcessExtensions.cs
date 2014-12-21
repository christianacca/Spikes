using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Eca.Commons.Extensions
{
    public static class ProcessExtensions
    {
        #region Class Members

        /// <summary>
        /// Filters the list of <paramref name="processes"/> supplied to those that are running
        /// </summary>
        public static ICollection<Process> GetRunning(this IEnumerable<Process> processes)
        {
            return processes.Safe().SkipNulls().Where(wp => !wp.HasExited).ToList();
        }


        /// <summary>
        /// Returns the ids of the <paramref name="processes"/> supplied as a comma delimited string
        /// </summary>
        /// <param name="processes"></param>
        /// <returns></returns>
        public static string Ids(this IEnumerable<Process> processes)
        {
            return processes.Join(",", process => process.Id.ToString());
        }


        /// <summary>
        /// Starts the process while pausing the current thread and waiting indefinitely for the process
        /// to exit and return an exit code.
        /// </summary>
        /// <param name="process">Process to run</param>
        /// <returns>Exit code that highlights whether the Process exited naturally (0) or had some failure</returns>
        public static int Run(this Process process)
        {
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }


        /// <summary>
        /// Starts the process while pausing the current thread and waiting on1 the process
        /// to exit for the given amount of time and return an exit code.
        /// </summary>
        /// <param name="process">Process to run</param>
        /// <param name="milliseconds">Wait time in milliseconds</param>
        /// <returns>Exit code that highlights whether the Process exited naturally (0) or had some failure</returns>
        public static int Run(this Process process, int milliseconds)
        {
            process.Start();
            process.WaitForExit(milliseconds);
            return process.ExitCode;
        }

        #endregion
    }
}