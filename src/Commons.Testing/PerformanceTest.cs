using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    public static class PerformanceTestExtensions
    {
        #region Class Members

        public static void AssertFailuresNotExceeding(this IEnumerable<PerformanceTest.Result> results,
                                                      int allowedFailures)
        {
            var failures = results.Failures().ToList();

            Debug.WriteIf(allowedFailures > 0 && failures.Count > 0,
                          string.Format("{0} performance tests have failed but are being allowed to pass",
                                        failures.Count));

            Assert.That(failures.Count, Is.LessThanOrEqualTo(allowedFailures), "failures exceeded number allowed");
        }


        public static void AssertNoFailures(this IEnumerable<PerformanceTest.Result> results)
        {
            results.AssertFailuresNotExceeding(0);
        }


        public static PerformanceTestRunner Configure(this ICollection<PerformanceTest> source)
        {
            return new PerformanceTestRunner(source);
        }


        public static IEnumerable<PerformanceTest.Result> Failures(this IEnumerable<PerformanceTest.Result> source)
        {
            return source.Where(result => result.Failed);
        }


        public static PerformanceTest GetByName(this ICollection<PerformanceTest> source, string name)
        {
            return source.Single(test => test.TestName == name);
        }


        public static IEnumerable<PerformanceTest.Result> PrintResultsTo(
            this IEnumerable<PerformanceTest.Result> results, TextWriter writer)
        {
            foreach (PerformanceTest.Result result in results)
            {
                writer.WriteLine(result);
                yield return result;
            }
        }


        public static ICollection<PerformanceTest.Result> Run(this ICollection<PerformanceTest> source)
        {
            return new PerformanceTestRunner(source).Run().ToList();
        }


        public static ICollection<PerformanceTest.Result> RunWithRetries(this ICollection<PerformanceTest> source,
                                                                         int numberOfRetries)
        {
            return new PerformanceTestRunner(source).Run(numberOfRetries).ToList();
        }

        #endregion
    }



    public class PerformanceTest
    {
        #region Constructors

        public PerformanceTest()
        {
            NumberOfExecutions = 1000;
            FactorOutJitCompilation = true;
            CodeToExecuteBeforeRun = delegate {};
        }

        #endregion


        #region Properties

        public Action CodeToExecuteBeforeRun { get; set; }
        public Type ExceptionToIgnore { get; set; }
        public bool FactorOutJitCompilation { get; set; }
        public int NumberOfExecutions { get; set; }
        public double RequiredAverageExecutionTimeInMilliseconds { get; set; }

        public int RequiredNumberOfExecutionsPerSecond
        {
            get { return (int) (1000/RequiredAverageExecutionTimeInMilliseconds); }
        }

        public Action Test { get; set; }
        public string TestName { get; set; }

        #endregion


        /// <exception cref="Exception">Any exception that is not being ignored</exception>
        private void ExecuteTest()
        {
            for (int i = 0; i < NumberOfExecutions; i++)
            {
                try
                {
                    Test();
                }
                catch (Exception ex)
                {
                    if (ExceptionToIgnore == null || ex.GetType() != ExceptionToIgnore)
                        throw;
                }
            }
        }


        public Result Run(int attempt)
        {
            if (FactorOutJitCompilation)
            {
                //ensure that the 'execute' method has been JIT compiled as we don't want the time for JIT compiliation
                //included in the timing
                CodeToExecuteBeforeRun();
                With.PerformanceCounter(ExecuteTest);
            }

            CodeToExecuteBeforeRun();
            //now time the test
            double actualTotalTimeInSeconds = With.PerformanceCounter(ExecuteTest);
            return new Result(actualTotalTimeInSeconds, attempt, this);
        }


        #region Overridden object methods

        public override string ToString()
        {
            return String.Format(
                "{0}; required to take <= {1} ms, ie {2:n} executions per second (eps)",
                TestName,
                RequiredAverageExecutionTimeInMilliseconds,
                RequiredNumberOfExecutionsPerSecond);
        }

        #endregion


        public class Result
        {
            #region Member Variables

            private readonly double _actualTotalTimeInSeconds;
            private readonly int _attempt;
            private readonly PerformanceTest _perfTest;

            #endregion


            #region Constructors

            public Result(double actualTotalTimeInSeconds, int attempt, PerformanceTest test)
            {
                _actualTotalTimeInSeconds = actualTotalTimeInSeconds;
                _attempt = attempt;
                _perfTest = test;
            }

            #endregion


            #region Properties

            public double ActualAverageExecutionTimeInMilliseconds
            {
                get
                {
                    double actualTotalTimeInMilliseconds = _actualTotalTimeInSeconds*1000;
                    return actualTotalTimeInMilliseconds/_perfTest.NumberOfExecutions;
                }
            }

            private long ActualNumberOfExecutionsPerSecond
            {
                get
                {
                    if (ActualAverageExecutionTimeInMilliseconds == 0) return 0;

                    return (long) (1000/ActualAverageExecutionTimeInMilliseconds);
                }
            }

            public bool Failed
            {
                get
                {
                    return ActualAverageExecutionTimeInMilliseconds >
                           _perfTest.RequiredAverageExecutionTimeInMilliseconds;
                }
            }

            public int Retries
            {
                get { return _attempt - 1; }
            }

            #endregion


            #region Overridden object methods

            public override string ToString()
            {
                string infoText = Retries > 0 ? string.Format("; Note: Number of retries {0}", Retries) : string.Empty;
                return String.Format(
                    "Test({0}): {1}; took {2} ms, ie {3:n} eps{4}",
                    Failed ? "F" : "P",
                    _perfTest,
                    ActualAverageExecutionTimeInMilliseconds,
                    ActualNumberOfExecutionsPerSecond,
                    infoText);
            }

            #endregion
        }
    }
}