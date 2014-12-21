using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace Eca.Commons.Testing
{
    public class CrossThreadTestRunner
    {
        #region Member Variables

        private Exception lastException;

        #endregion


        private bool ExceptionWasThrown()
        {
            return lastException != null;
        }


        private void Run(ThreadStart userDelegate, ApartmentState apartmentState)
        {
            lastException = null;

            var thread = new Thread(
                delegate() {
                    try
                    {
                        userDelegate.Invoke();
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                    }
                });
            thread.SetApartmentState(apartmentState);

            thread.Start();
            thread.Join();

            if (ExceptionWasThrown())
                ThrowExceptionPreservingStack(lastException);
        }


        public void RunInMTA(ThreadStart userDelegate)
        {
            Run(userDelegate, ApartmentState.MTA);
        }


        public void RunInSTA(ThreadStart userDelegate)
        {
            Run(userDelegate, ApartmentState.STA);
        }


        #region Class Members

        [ReflectionPermission(SecurityAction.Demand)]
        private static void ThrowExceptionPreservingStack(Exception exception)
        {
            FieldInfo remoteStackTraceString = typeof (Exception).GetField(
                "_remoteStackTraceString",
                BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
            throw exception;
        }

        #endregion
    }
}