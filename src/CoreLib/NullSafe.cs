using System;
using System.Diagnostics;

namespace Eca.Commons
{
    /// <summary>
    /// Provides utility methods that make working with standard Func and Action delegates less prone to 
    /// causing <see cref="NullReferenceException"/> from bubbling up
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sometimes, a <see cref="NullReferenceException"/> is an indicator of a problem with the code
    /// running within the Func delegate. In other cases, having to guard against <see cref="NullReferenceException"/> 
    /// can lead to the mudding the main success path of code with <see cref="NullReferenceException"/> checks.
    /// The utility methods here aim to reduce this noise and increase the clarity of the main code path.
    /// </para>
    /// <para>
    /// They are particularly when writing in a functional programming style, or in framework code, where nulls often just get in the way
    /// </para>
    /// </remarks>
    [DebuggerStepThrough]
    public static class NullSafe
    {
        #region Class Members

        /// <summary>
        /// Execute <paramref name="action"/> in a try catch block that will
        /// swallow any <see cref="NullReferenceException"/> that <paramref name="action"/> throws
        /// </summary>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is null</exception>
        public static void Execute<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action,
                                                   T1 arg1,
                                                   T2 arg2,
                                                   T3 arg3,
                                                   T4 arg4)
        {
            ExceptionSafe.ExecuteAndIgnore<NullReferenceException>(() => action(arg1, arg2, arg3, arg4));
        }


        /// <summary>
        /// Execute <paramref name="func"/> in a try catch block that will
        /// return the default value of <typeparamref name="TResult"/> if <paramref name="func"/> throws a
        /// <see cref="NullReferenceException"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">when <paramref name="func"/> is null</exception>
        public static TResult Execute<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func,
                                                               T1 arg1,
                                                               T2 arg2,
                                                               T3 arg3,
                                                               T4 arg4)
        {
            return ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg1, arg2, arg3, arg4));
        }


        /// <seealso cref="Execute{T1,T2,T3,T4,TResult}"/>
        public static TResult Execute<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func,
                                                           T1 arg1,
                                                           T2 arg2,
                                                           T3 arg3)
        {
            return ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg1, arg2, arg3));
        }


        /// <seealso cref="Execute{T1,T2,T3,T4,TResult}"/>
        public static TResult Execute<T1, T2, TResult>(Func<T1, T2, TResult> func,
                                                       T1 arg1,
                                                       T2 arg2)
        {
            return ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg1, arg2));
        }


        /// <seealso cref="Execute{T1,T2,T3,T4,TResult}"/>
        public static TResult Execute<T1, TResult>(Func<T1, TResult> func, T1 arg)
        {
            return ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg));
        }


        /// <seealso cref="Execute{T1,T2,T3,T4,TResult}"/>
        public static TResult Execute<TResult>(Func<TResult> func)
        {
            return ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(func);
        }


        /// <summary>
        /// Creates a wrapper function that will execute <paramref name="func"/> in a try catch block that will
        /// return the default value of <typeparamref name="TResult"/> if <paramref name="func"/> throws a
        /// <see cref="NullReferenceException"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">when <paramref name="func"/> is null</exception>
        public static Func<T1, T2, T3, T4, TResult> MakeSafe<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func)
        {
            return
                (arg1, arg2, arg3, arg4) =>
                ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg1, arg2, arg3, arg4));
        }


        /// <seealso cref="MakeSafe{T1,T2,T3,T4,TResult}"/>
        public static Func<T1, T2, T3, TResult> MakeSafe<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
        {
            return
                (arg1, arg2, arg3) =>
                ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg1, arg2, arg3));
        }


        /// <seealso cref="MakeSafe{T1,T2,T3,T4,TResult}"/>
        public static Func<T1, T2, TResult> MakeSafe<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            return
                (arg1, arg2) => ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg1, arg2));
        }


        /// <seealso cref="MakeSafe{T1,T2,T3,T4,TResult}"/>
        public static Func<T, TResult> MakeSafe<T, TResult>(Func<T, TResult> func)
        {
            return arg => ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(() => func(arg));
        }


        /// <seealso cref="MakeSafe{T1,T2,T3,T4,TResult}"/>
        public static Func<TResult> MakeSafe<TResult>(Func<TResult> func)
        {
            return () => ExceptionSafe.ExecuteAndIgnore<NullReferenceException, TResult>(func);
        }

        #endregion
    }
}