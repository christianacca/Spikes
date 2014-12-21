using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Eca.Commons.Extensions;
using Microsoft.Web.Administration;

namespace Eca.Commons.App.Web
{
    public static class ApplicationPoolExtensions
    {
        #region Member Variables

        /// <summary>
        /// The default amount of time (in milliseonds) to wait on any w3c worker processes that are serving the pool to complete and exit
        /// </summary>
        public const int DefaultStopMaxWait = 30000;

        #endregion


        #region Class Members

        /// <summary>
        /// Calls <see cref="Restart(Microsoft.Web.Administration.ApplicationPool, int)"/> with a <c>stopMaxWait</c>
        /// set to <see cref="DefaultStopMaxWait"/>.
        /// </summary>
        public static void Restart(this ApplicationPool pool)
        {
            Restart(pool, DefaultStopMaxWait);
        }


        /// <summary>
        /// <see cref="ApplicationPool.Stop"/> and <see cref="ApplicationPool.Start"/> the application pool
        /// </summary>
        /// <remarks>
        /// The <paramref name="stopMaxWait"/> determines the amount of time to wait on any w3c worker processes
        /// that are serving the pool to complete and exit. If this timeout is exceeded an <see cref="TimeoutException"/>
        /// will be thrown and the app pool will not be restarted.
        /// </remarks>
        /// <param name="pool">pool to restart </param>
        /// <param name="stopMaxWait">time in milliseconds</param>
        public static void Restart(this ApplicationPool pool, int stopMaxWait)
        {
            IEnumerable<Process> w3cWorkers =
                pool.WorkerProcesses.Select(p => Process.GetProcessById(p.ProcessId)).ToList();
            pool.Stop();

            int nextWaitMilliseconds = 1;
            while (w3cWorkers.GetRunning().Any() && nextWaitMilliseconds <= stopMaxWait)
            {
                Thread.Sleep(nextWaitMilliseconds);
                nextWaitMilliseconds += nextWaitMilliseconds;
                Console.Out.WriteLine("Waiting on processes to exit: {0}", w3cWorkers.GetRunning().Ids());
            }

            try
            {
                var runningWorkers = w3cWorkers.GetRunning();
                if (runningWorkers.Any())
                {
                    string msg =
                        string.Format(
                            "Previous w3c worker process failed to stop within the time allowed ({0}); process ids that were not stopped: {1}",
                            stopMaxWait,
                            runningWorkers.Ids());
                    throw new TimeoutException(msg);
                }
            }
            finally
            {
                foreach (var worker in w3cWorkers)
                {
                    worker.Dispose();
                }
            }
            pool.Start();
        }

        #endregion
    }
}