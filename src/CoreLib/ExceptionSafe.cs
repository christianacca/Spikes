using System;
using System.Diagnostics;

namespace Eca.Commons
{
    /// <summary>
    /// Utility methods that execute a delegate in a try/catch block that swallows an expected exception
    /// </summary>
    /// <remarks>
    /// Use with caution! Swallowing to broad a set of Exceptions, it usually bad practice
    /// </remarks>
    [DebuggerStepThrough]
    public static class ExceptionSafe
    {
        #region Class Members

        private static void CheckActionSupplied(object action)
        {
            if (action == null)
                throw new ArgumentNullException("action",
                                                "The intent is to protect code executing within 'action', not to protect against calling a null delegate");
        }


        private static void CheckFuncSupplied(object func)
        {
            if (func == null)
                throw new ArgumentNullException("func",
                                                "The intent is to protect code executing within 'func', not to protect against calling a null delegate");
        }


        /// <summary>
        /// Execute <paramref name="action"/> in a try catch block that will
        /// swallows an exception of type or exception deriving from type <typeparamref name="TException"/> that <paramref name="action"/> throws
        /// </summary>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is null</exception>
        public static void ExecuteAndIgnore<TException>(Action action)
        {
            CheckActionSupplied(action);
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (!typeof (TException).IsAssignableFrom(e.GetType()))
                    throw;
            }
        }


        /// <summary>
        /// Execute <paramref name="func"/> in a try catch block that will
        /// return the default value of <typeparamref name="TResult"/> if <paramref name="func"/> throws
        /// an exception of type or exception deriving from type <typeparamref name="TException"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">when <paramref name="func"/> is null</exception>
        public static TResult ExecuteAndIgnore<TException, TResult>(Func<TResult> func)
        {
            CheckFuncSupplied(func);
            try
            {
                return func();
            }
            catch (Exception e)
            {
                if (!typeof (TException).IsAssignableFrom(e.GetType())) throw;
                return default(TResult);
            }
        }

        #endregion
    }
}