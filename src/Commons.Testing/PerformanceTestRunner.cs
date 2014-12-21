using System;
using System.Collections.Generic;

namespace Eca.Commons.Testing
{
    public class PerformanceTestRunner
    {
        #region Member Variables

        private Action _codeToExecuteBeforeEachTest = delegate {};


        private Type _exceptionToIgnore;


        private bool? _factorOutJitCompilation;
        private int? _numberOfExecutionsPerTest;

        #endregion


        #region Constructors

        public PerformanceTestRunner(ICollection<PerformanceTest> tests)
        {
            Tests = tests;
        }

        #endregion


        #region Properties

        private ICollection<PerformanceTest> Tests { get; set; }

        #endregion


        public PerformanceTestRunner CodeToExecuteBeforeEachTest(Action setupCode)
        {
            _codeToExecuteBeforeEachTest = setupCode;
            return this;
        }


        private void ConfigureTests()
        {
            foreach (PerformanceTest test in Tests)
            {
                PerformanceTest tmp = test;
                var testSpecificSetup = tmp.CodeToExecuteBeforeRun;
                test.CodeToExecuteBeforeRun = delegate {
                    //execute global setup
                    _codeToExecuteBeforeEachTest();
                    //execute test specific setup
                    testSpecificSetup();
                };
                test.ExceptionToIgnore = _exceptionToIgnore;
                test.FactorOutJitCompilation = _factorOutJitCompilation ?? test.FactorOutJitCompilation;
                test.NumberOfExecutions = _numberOfExecutionsPerTest ?? test.NumberOfExecutions;
            }
        }


        private IEnumerable<PerformanceTest.Result> DoRun(IEnumerable<PerformanceTest> source,
                                                          int numberOfRetries)
        {
            int attemptsAllowed = numberOfRetries + 1;
            foreach (PerformanceTest test in source)
            {
                int tries = 0;
                PerformanceTest.Result result;
                do
                {
                    tries++;
                    result = test.Run(tries);
                } while (tries < attemptsAllowed && result.Failed);

                yield return result;
            }
        }


        public PerformanceTestRunner ExceptionToIgnore(Type exceptionToIgnore)
        {
            _exceptionToIgnore = exceptionToIgnore;
            return this;
        }


        public PerformanceTestRunner FactorOutJitCompilation(bool value)
        {
            _factorOutJitCompilation = value;
            return this;
        }


        public PerformanceTestRunner NumberOfExecutionsPerTest(int value)
        {
            _numberOfExecutionsPerTest = value;
            return this;
        }


        public IEnumerable<PerformanceTest.Result> Run()
        {
            ConfigureTests();
            return DoRun(Tests, 0);
        }


        public IEnumerable<PerformanceTest.Result> Run(int retries)
        {
            ConfigureTests();
            return DoRun(Tests, retries);
        }
    }
}