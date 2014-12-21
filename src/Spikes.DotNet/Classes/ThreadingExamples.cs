using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ThreadingExamples
    {
        private bool _completed;
        private int _counter;
        private int _leftHand;
        private int _rightHand;
        private int _valueOfParameter;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialize()
        {
            _completed = false;
            _counter = 0;
            _leftHand = 0;
            _rightHand = 0;
        }

        #endregion


        private delegate int IntReturingMethodDelegate();


        #region Test helpers

        private void MethodProtectedWithCriticalRegion(WaitHandle waitHandle)
        {
            Thread.BeginCriticalRegion();
            _leftHand -= 5;
            waitHandle.WaitOne(); // main thread is now going to call Abort() on me
            _rightHand += 5;
            Thread.EndCriticalRegion();

            _completed = true; // this will never be called because I will be aborted
        }


        private void MethodProtectedWithTryCatchFinally(WaitHandle waitHandle)
        {
            //I would seriously consider using internal Memento pattern here in order to rollback
            //state
            int oldLeftHand = _leftHand;
            int oldRightHand = _rightHand;
            bool error = false;
            try
            {
                _leftHand -= 5;
                waitHandle.WaitOne(); // main thread is now going to call Abort() on me
                _rightHand += 5;
            }
            catch (ThreadAbortException)
            {
                error = true;
                throw;
            }
            finally
            {
                if (error)
                {
                    _leftHand = oldLeftHand;
                    _rightHand = oldRightHand;
                }
            }

            _completed = true; // this will never be called because I will be aborted
        }


        private void SimpleOperation()
        {
            _completed = true;
            _counter++;
        }


        private Thread StartInBackgroundThread(ThreadStart operation)
        {
            Thread thread = new Thread(operation);
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }


        private int SumOf(IEnumerable<int> values)
        {
            int result = 0;
            foreach (int value in values)
                result += value;
            return result;
        }

        #endregion


        [Test]
        public void HowToAsynchronouslyExecuteAMethodThatReturns()
        {
            IntReturingMethodDelegate methodToExecuteAsynchronously = delegate {
                return 10;
            };

            int result = 0;
            AsyncCallback callbackMethod = delegate(IAsyncResult ar) {
                object aynchDelegate = ((AsyncResult) ar).AsyncDelegate;
                result = ((IntReturingMethodDelegate) aynchDelegate).EndInvoke(ar);
            };

            //this method will be executed in the thread pool
            methodToExecuteAsynchronously.BeginInvoke(callbackMethod, null);

            Thread.Sleep(10); //give asynch method time to execute

            Assert.That(result, Is.EqualTo(10));
        }


        [Test]
        public void CanExecuteSimpleVoidMethodInABackgroundThread()
        {
            Thread thread = new Thread(SimpleOperation);
            thread.IsBackground = true;
            thread.Start();
            //ensure that we block until the thread has completed
            thread.Join();

            Assert.That(_completed, Is.True);
        }


        [Test]
        public void CanExecuteSimpleVoidMethodInThreadPool()
        {
            ThreadPool.QueueUserWorkItem(delegate {
                SimpleOperation();
            });

            Thread.Sleep(10);
            Assert.That(_completed, Is.True);
        }


        [Test]
        public void CanExecuteMultipleSimpleVoidMethodInBackgroundThread()
        {
            int numberOfThreads = 5;
            Thread[] threads = new Thread[numberOfThreads];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = StartInBackgroundThread(SimpleOperation);
                threads[i].Join();
            }
            Assert.That(_counter, Is.EqualTo(numberOfThreads));
        }


        [Test]
        public void CanExecuteMethodThatTakesAParameterInABackgroundThread()
        {
            Thread thread = new Thread(delegate(object obj) {
                _valueOfParameter = (int) obj;
            });

            int parameterValue = 10;
            thread.Start(parameterValue);
            thread.Join();

            Assert.That(_valueOfParameter, Is.EqualTo(parameterValue));
        }


        [Test]
        public void GotchaCannotGuaranteeRegionsOfCodeAreNotAbortedPartWayThrough()
        {
            _leftHand = 10;
            _rightHand = 10;
            Thread myThread = StartInBackgroundThread(delegate {
                MethodProtectedWithCriticalRegion(new AutoResetEvent(false));
            });
            Thread.Sleep(10); //give myThread a time slice to execute its first statement

            myThread.Abort();
            myThread.Join(); //wait on myThread to finish-up and abort

            Assert.That(_leftHand, Is.EqualTo(5), "left hand side down by 5");
            Assert.That(_rightHand, Is.EqualTo(10), "statement did not run other otherwise _rightHand would equal 15");
            Assert.That(_completed, Is.False, "operation did not finish");
        }


        [Test]
        public void MustManuallyEnsureAtmoicityOfBlockOfStatementsExecutedInThread()
        {
            _leftHand = 10;
            _rightHand = 10;
            Thread myThread = StartInBackgroundThread(delegate {
                MethodProtectedWithTryCatchFinally(new AutoResetEvent(false));
            });
            Thread.Sleep(1); //give myThread a time slice to execute its first statement

            myThread.Abort();
            myThread.Join(); //wait on myThread to finish-up and abort

            Assert.That(_leftHand, Is.EqualTo(10), "left hand side not changed");
            Assert.That(_rightHand, Is.EqualTo(10), "right hand side not changed");
            Assert.That(_completed, Is.False, "operation did not finish");
        }


        [Test]
        public void CanControlAccessToResource_Analogous_To_AutomaitcGate()
        {
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            List<int> threadIds = new List<int>(2);
            ThreadStart operationToLogThreadId = delegate {
                waitHandle.WaitOne();
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            };

            Thread t1 = StartInBackgroundThread(operationToLogThreadId);
            Thread.Sleep(1); //give t1 a time slice to call WaitOne

            Thread t2 = StartInBackgroundThread(operationToLogThreadId);
            Thread.Sleep(1); //give t2 a time slice to call WaitOne

            waitHandle.Set();
            Thread.Sleep(1); //give t1 a time slice to finish

            Assert.That(threadIds.Count, Is.EqualTo(1), "only one thread has been allowed to process");

            waitHandle.Set();
            Thread.Sleep(1); //give t2 a time slice to finish

            Assert.That(threadIds, Has.Member(t1.ManagedThreadId), "first thread got to run");
            Assert.That(threadIds, Has.Member(t2.ManagedThreadId), "second thread got to run");
        }


        [Test, Ignore("No longer correct")]
        public void CanControlAccessToResource_Analogous_To_TrafficLights()
        {
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            List<int> threadIds = new List<int>(2);
            ThreadStart operationToLogThreadId = delegate {
                waitHandle.WaitOne();
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            };

            StartInBackgroundThread(operationToLogThreadId);
            Thread.Sleep(1); //give thread a time slice to call WaitOne

            StartInBackgroundThread(operationToLogThreadId);
            Thread.Sleep(1); //give thread a time slice to call WaitOne

            waitHandle.Set();
            Thread.Sleep(10); //give both threads time to finish

            Assert.That(threadIds.Count, Is.EqualTo(2), "both threads has been allowed to process");
        }


        [Test]
        public void CanEnsureThatTwoThreadsUnblockAtTheSameTime()
        {
            DateTime? thread1Time = null;
            DateTime? thread2Time = null;

            AutoResetEvent handle1 = new AutoResetEvent(false);
            AutoResetEvent handle2 = new AutoResetEvent(false);

            Thread t1 = StartInBackgroundThread(delegate {
                WaitHandle.SignalAndWait(handle1, handle2);
                thread1Time = DateTime.Now;
            });

            Thread t2 = StartInBackgroundThread(delegate {
                Thread.Sleep(1000); //simulate work
                WaitHandle.SignalAndWait(handle2, handle1);
                thread2Time = DateTime.Now;
            });

            t1.Join();
            t2.Join();

            TimeSpan? span = thread1Time - thread2Time;
            Assert.That(span.Value.Milliseconds, Is.EqualTo(0));
        }


        [Test]
        public void CanSynchroniseThreadsUsingTwoSeparateMutexInstances()
        {
            Mutex existingMutex = null;
            try
            {
                //create mutex and "lock"
                existingMutex = new Mutex(true, "Existing");

                bool timedOut = false;
                Thread thread = StartInBackgroundThread(delegate {
                    using (Mutex mutex = new Mutex(true, "Existing"))
                    {
                        timedOut = (mutex.WaitOne(1, false) == false);
                    }
                });

                thread.Join();
                Assert.That(timedOut, Is.True);
            }
            finally
            {
                if (existingMutex != null) existingMutex.ReleaseMutex();
            }
        }


        [Test, Ignore("Bug in NUnit framework causing false negative")]
        public void CanSignalWaitingThreadThatResourceIsReadyOrConditionSatisfied()
        {
            object waitLocker = new object();

            int timeForThread = 1000;
            Thread thread = new Thread(delegate() {
                lock (waitLocker)
                {
                    Thread.Sleep(timeForThread); //simulate working being done
                    Monitor.Pulse(waitLocker); //signal waiting thread that work is now done
                } //lock on waitLocker will now be released, and our main thread will now reaquire it
            });

            Stopwatch stopwatch;
            lock (waitLocker)
            {
                stopwatch = Stopwatch.StartNew();
                thread.Start();
                Monitor.Wait(waitLocker); //release lock then block until signalled by another thread
                stopwatch.Stop();
            }
            int giveOrTake = 30;
            //this fails because of a bug in the NUnit framework
            Assert.That(stopwatch.ElapsedMilliseconds,
                        Is.GreaterThanOrEqualTo(timeForThread - giveOrTake) &
                        Is.LessThanOrEqualTo(timeForThread + giveOrTake));
        }


        [Test]
        public void SurprisingFewThreadsWillBeUsedFromThreadPoolToExecuteScheduledOperations()
        {
            object locker = new object();
            IDictionary<int, int> threadUsageStatistics = new Dictionary<int, int>();

            WaitOrTimerCallback recordThreadUsage = delegate {
                lock (locker)
                {
                    if (threadUsageStatistics.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        threadUsageStatistics[Thread.CurrentThread.ManagedThreadId]++;
                    else
                        threadUsageStatistics.Add(Thread.CurrentThread.ManagedThreadId, 1);
                }
            };

            ManualResetEvent waitHandle = new ManualResetEvent(false);
            int scheduledOperations = 100;
            for (int i = 0; i < scheduledOperations; i++)
            {
                ThreadPool.RegisterWaitForSingleObject(waitHandle, recordThreadUsage, null, Timeout.Infinite, true);
            }

            waitHandle.Set(); //start all threads in the pool
            Thread.Sleep(100); //give threads enough time to complete

            Assert.That(threadUsageStatistics.Count,
                        Is.LessThan(5),
                        "very few physical threads used to execute our delegates");
            Assert.That(SumOf(threadUsageStatistics.Values),
                        Is.EqualTo(scheduledOperations),
                        "all scheduled delegates where executed");
        }


        [Test]
        public void HowToWaitForAllThreadPoolThreadsToComplete()
        {
            //note: the following article shows another way to do this but its flawed as WaitHandle.WaitAll() 
            //is not supported when caller is on a thread decorated with STAThread (like all WinForm apps!)
            //http://msdn2.microsoft.com/en-us/library/system.threading.autoresetevent.aspx

            object waitLocker = new object();

            int threads = 10;
            int runningThreads = threads;
            int timeForOneThread = 20;

            WaitCallback routineToSchedule = delegate {
                Thread.Sleep(timeForOneThread); //simulate working being done
                lock (waitLocker)
                {
                    runningThreads--;
                    Monitor.Pulse(waitLocker); //signal waiting thread that work is now done
                } //lock on waitLocker will now be released, and our main thread will now reaquire it
            };

            lock (waitLocker)
            {
                for (int i = 0; i < threads; i++)
                {
                    ThreadPool.QueueUserWorkItem(routineToSchedule);
                }
                //release lock then block until signalled by threads that all threads are complete
                while (runningThreads > 0) Monitor.Wait(waitLocker);
            }
        }


        [Test]
        public void ReadLockWillBlockWhenWriteLockHeld()
        {
            ReaderWriterLock readerWriterLock = new ReaderWriterLock();
            readerWriterLock.AcquireWriterLock(0);

            bool timedOut = false;
            Thread thread = StartInBackgroundThread(delegate {
                try
                {
                    readerWriterLock.AcquireReaderLock(1);
                }
                catch (ApplicationException)
                {
                    timedOut = true;
                }
            });

            thread.Join();
            Assert.That(timedOut, Is.True);
        }


        [Test]
        public void ReadLocksWillNotBlockWhenAnotherReadLockHeld()
        {
            ReaderWriterLock readerWriterLock = new ReaderWriterLock();
            readerWriterLock.AcquireReaderLock(0);

            bool timedOut = false;
            Thread thread = StartInBackgroundThread(delegate {
                try
                {
                    readerWriterLock.AcquireReaderLock(1);
                }
                catch (ApplicationException)
                {
                    timedOut = true;
                }
            });

            thread.Join();
            Assert.That(timedOut, Is.False);
        }
    }
}