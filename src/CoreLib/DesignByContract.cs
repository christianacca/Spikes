// Provides support for Design By Contract
// as described by Bertrand Meyer in his seminal book,
// Object-Oriented Software Construction (2nd Ed) Prentice Hall 1997
// (See chapters 11 and 12).
// 
// See also Building Bug-free O-O Software: An Introduction to Design by Contract
// http://www.eiffel.com/doc/manuals/technology/contract/
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Eca.Commons.Extensions;

namespace Eca.Commons
{
    /// <summary>
    /// Design By Contract Checks. Each method generates an exception and optionally a trace
    /// assertion if the contract is broken. A broken contract indicates a programming error. The
    /// specific exception type derived from <see cref="DesignByContractException"/> indicating
    /// whether fault is with the client code or the class that raised the exception.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use Require() to verify that a precondition of a class method is satisfied raising a <see
    /// cref="PreconditionException"/> if not.
    /// </para>
    /// <para>
    /// Use Ensure() to verify that a postcondition of a class method is satisfied raising a <see
    /// cref="PostconditionException"/> if not.
    /// </para>
    /// <para>
    /// Use Invariant() to verify that a class invariant is satisfied raising a <see
    /// cref="InvalidCastException"/> if not.
    /// </para>
    /// <para>
    /// Use Assert() to verify assumptions of internal code raising a <see
    /// cref="AssertionException"/> if not.
    /// </para>
    /// 
    /// <para>
    /// DotNet trace assertions are useful when trying to diagnose a DBC failure. They allow you to
    /// launch a debugger and step through the code at the point of failure. This technique becomes
    /// amazingly powerful when combined with the following trace listener:
    /// http://msdn.microsoft.com/msdnmag/issues/05/11/bugslayer/default.aspx 
    /// </para>
    ///
    /// <para>
    /// By default, DotNet trace asserts will be fired on a violation of an Invariant, Precondition
    /// or Postcondition. To disable these trace asserts set <see cref="FireDotNetAsserts"/> to
    /// false.
    /// </para>
    /// 
    /// <para>
    /// WARNING: be extremely careful when enabling DotNet trace asserts in production deployments
    /// for Windows Services or other non-user interactive applications. You must make sure that any
    /// attached listeners do not open a dialog box (which is the default behaviour for <see
    /// cref="DefaultTraceListener"/> unless its configured so that its <see
    /// cref="DefaultTraceListener.AssertUiEnabled"/> property is set to false
    /// </para>
    /// </remarks>
    [DebuggerStepThrough]
    public static class Check
    {
        #region Delegates

        private delegate void DbcCheckMethod(bool assertion, string message, IEnumerable<Exception> inners);



        public delegate void Proc();

        #endregion


        #region Class Members

        static Check()
        {
            FireDotNetAsserts = true;
        }


        /// <summary>
        /// Set this if you wish to use Trace Assert statements 
        /// in addition to throwing an exception when a DBC contract is broken.
        /// (The default is to use Trace Assert's)
        /// </summary>
        public static bool FireDotNetAsserts { get; set; }


        /// <summary>
        /// Assertion check.
        /// </summary>
        public static void Assert(bool assertion)
        {
            Assert(assertion, "Unspecified");
        }


        /// <summary>
        /// Assertion check.
        /// </summary>
        public static void Assert(bool assertion, string message)
        {
            Assert(assertion, message, null);
        }


        /// <exception cref="AssertionException"><c>AssertionException</c>.</exception>
        private static void Assert(bool assertion, string message, IEnumerable<Exception> innerExceptions)
        {
            if (!assertion)
            {
                RaiseDotNetAssert(message, "Assert");
                throw new AssertionException(message, innerExceptions);
            }
        }


        /// <summary>
        /// Assertion check.
        /// </summary>
        /// <param name="checkers">A collection of procedures that will perform the Dbc check</param>
        /// <remarks>
        /// <paramref name="checkers"/> should signal a Dbc violation by throwing an exception of
        /// type <see cref="ArgumentException"/> or one of its derivatives. The <see
        /// cref="ArgumentException"/> exception thrown by <paramref name="checkers"/> will then be
        /// will be wrapped in a <see cref="AssertionException"/>.
        /// <para>
        /// All other exceptions thrown by <paramref name="checkers"/> will not be wrapped but bubble
        /// up as is.
        /// </para>
        /// </remarks>
        public static void Assert(params Proc[] checkers)
        {
            ExecuteCheck(checkers, Assert);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="AssertionException"/>
        /// on the first occurance of an unsatisfying element
        /// </summary>
        /// <remarks>
        /// The excpetion thrown will contain a message derived from the <paramref name="predicate"/> expression supplied.
        /// For example: 
        /// <c>"AccruedBenefit does not satisfy the predicate: pmt => pmt.IsValid"</c>
        /// </remarks>
        public static void AssertForEach<T>(this IEnumerable<T> source, Expression<Func<T, bool>> predicate)
        {
            CheckForEach(source, predicate, Assert);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="AssertionException"/>
        /// on the first occurance of an unsatisfying element
        /// </summary>
        public static void AssertForEach<T>(this IEnumerable<T> source, Func<T, bool> predicate, string message)
        {
            CheckForEach(source, predicate, ignoredItem => message, Assert);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="AssertionException"/>
        /// on the first occurance of an unsatisfying element 
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="source">sequence containing the elements to test</param>
        /// <param name="predicate">The predicate test for an element</param>
        /// <param name="message">A function that accepts an unsatisfying element and should return the exception message</param>
        /// <exception cref="AssertionException"/>
        public static void AssertForEach<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T, string> message)
        {
            CheckForEach(source, predicate, message, Assert);
        }


        private static void CheckForEach<T>(this IEnumerable<T> source,
                                            Func<T, bool> predicate,
                                            Func<T, string> message,
                                            Action<bool, string> check)
        {
            foreach (T item in source)
            {
                bool isSatisfied = predicate(item);
                if (!isSatisfied) check(false, message(item));
            }
        }


        private static void CheckForEach<T>(this IEnumerable<T> source,
                                            Expression<Func<T, bool>> predicate,
                                            Action<bool, string> check)
        {
            CheckForEach(source, predicate.Compile(), item => FailureMessage(item, predicate), check);
        }


        private static ICollection<Exception> CollectAllExceptionsRaisedBy(IEnumerable<Proc> checkers)
        {
            var argumentExceptions = new List<ArgumentException>();
            foreach (Proc checker in checkers)
            {
                try
                {
                    checker();
                }
                catch (ArgumentException e)
                {
                    argumentExceptions.Add(e);
                }
            }
            return argumentExceptions.Cast<Exception>().ToList();
        }


        /// <summary>
        /// Postcondition check.
        /// </summary>
        public static void Ensure(bool assertion)
        {
            Ensure(assertion, "Unspecified", null);
        }


        /// <summary>
        /// Postcondition check.
        /// </summary>
        public static void Ensure(bool assertion, string message)
        {
            Ensure(assertion, message, null);
        }


        /// <exception cref="PostconditionException"><paramref name="message"/></exception>
        private static void Ensure(bool assertion, string message, IEnumerable<Exception> innerExceptions)
        {
            if (!assertion)
            {
                RaiseDotNetAssert(message, "Postcondition");
                throw new PostconditionException(message, innerExceptions);
            }
        }


        /// <summary>
        /// Postcondition check.
        /// </summary>
        /// <param name="checkers">A collection of procedures that will perform the Dbc check</param>
        /// <remarks>
        /// <paramref name="checkers"/> should signal a Dbc violation by throwing an exception of
        /// type <see cref="ArgumentException"/> or one of its derivatives. The <see
        /// cref="ArgumentException"/> exception thrown by <paramref name="checkers"/> will then be
        /// will be wrapped in a <see cref="PostconditionException"/>.
        /// <para>
        /// All other exceptions thrown by <paramref name="checkers"/> will not be wrapped but bubble
        /// up as is.
        /// </para>
        /// </remarks>
        public static void Ensure(params Proc[] checkers)
        {
            ExecuteCheck(checkers, Ensure);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="PostconditionException"/>
        /// on the first occurance of an unsatisfying element
        /// </summary>
        /// <remarks>
        /// The excpetion thrown will contain a message derived from the <paramref name="predicate"/> expression supplied.
        /// For example: 
        /// <c>"AccruedBenefit does not satisfy the predicate: pmt => pmt.IsValid"</c>
        /// </remarks>
        public static void EnsureForEach<T>(this IEnumerable<T> source, Expression<Func<T, bool>> predicate)
        {
            CheckForEach(source, predicate, Ensure);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="PostconditionException"/>
        /// on the first occurance of an unsatisfying element
        /// </summary>
        public static void EnsureForEach<T>(this IEnumerable<T> source, Func<T, bool> predicate, string message)
        {
            CheckForEach(source, predicate, ignoredItem => message, Ensure);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="PostconditionException"/>
        /// on the first occurance of an unsatisfying element 
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="source">sequence containing the elements to test</param>
        /// <param name="predicate">The predicate test for an element</param>
        /// <param name="message">A function that accepts an unsatisfying element and should return the exception message</param>
        /// <exception cref="PostconditionException"/>
        public static void EnsureForEach<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T, string> message)
        {
            CheckForEach(source, predicate, message, Ensure);
        }


        private static void ExecuteCheck(IEnumerable<Proc> checkers, DbcCheckMethod dbcCheckMethod)
        {
            ICollection<Exception> innerExceptions = CollectAllExceptionsRaisedBy(checkers);

            if (innerExceptions.Count == 0) return;

            string message = innerExceptions.Count > 1 ? "See Inner exceptions" : "See Inner exception";
            dbcCheckMethod(false, message, innerExceptions.ToArray());
        }


        private static string FailureMessage<T>(T item, Expression<Func<T, bool>> predicateExpression)
        {
            return string.Format("{0} does not satisfy predicate: {1}", item, predicateExpression);
        }


        /// <summary>
        /// Invariant check.
        /// </summary>
        public static void Invariant(bool assertion)
        {
            Invariant(assertion, "Unspecified", null);
        }


        /// <summary>
        /// Invariant check.
        /// </summary>
        public static void Invariant(bool assertion, string message)
        {
            Invariant(assertion, message, null);
        }


        /// <exception cref="InvariantException"><paramref name="message"/></exception>
        private static void Invariant(bool assertion, string message, IEnumerable<Exception> innerExceptions)
        {
            if (!assertion)
            {
                RaiseDotNetAssert(message, "Invariant");
                throw new InvariantException(message, innerExceptions);
            }
        }


        /// <summary>
        /// Invariant check.
        /// </summary>
        /// <param name="checkers">A collection of procedures that will perform the Dbc check</param>
        /// <remarks>
        /// <paramref name="checkers"/> should signal a Dbc violation by throwing an exception of
        /// type <see cref="ArgumentException"/> or one of its derivatives. The <see
        /// cref="ArgumentException"/> exception thrown by <paramref name="checkers"/> will then be
        /// will be wrapped in a <see cref="InvariantException"/>.
        /// <para>
        /// All other exceptions thrown by <paramref name="checkers"/> will not be wrapped but bubble
        /// up as is.
        /// </para>
        /// </remarks>
        public static void Invariant(params Proc[] checkers)
        {
            ExecuteCheck(checkers, Invariant);
        }


        private static void RaiseDotNetAssert(string message, string assertType)
        {
#if !SILVERLIGHT
            if (FireDotNetAsserts) Trace.Assert(false, string.Format("{0}:- {1}", assertType, message));
#endif
        }


        /// <summary>
        /// Precondition check.
        /// </summary>
        public static void Require(bool assertion)
        {
            Require(assertion, "Unspecified", null);
        }


        /// <summary>
        /// Precondition check.
        /// </summary>
        public static void Require(bool assertion, string message)
        {
            Require(assertion, message, null);
        }


        /// <exception cref="PreconditionException"><paramref name="message"/></exception>
        private static void Require(bool assertion, string message, IEnumerable<Exception> innerExceptions)
        {
            if (!assertion)
            {
                RaiseDotNetAssert(message, "Precondition");
                throw new PreconditionException(message, innerExceptions);
            }
        }


        /// <summary>
        /// Precondition check.
        /// </summary>
        /// <param name="checkers">A collection of procedures that will perform the Dbc check</param>
        /// <remarks>
        /// <paramref name="checkers"/> should signal a Dbc violation by throwing an exception of
        /// type <see cref="ArgumentException"/> or one of its derivatives. The <see
        /// cref="ArgumentException"/> exception thrown by <paramref name="checkers"/> will then be
        /// will be wrapped in a <see cref="PreconditionException"/>.
        /// <para>
        /// All other exceptions thrown by <paramref name="checkers"/> will not be wrapped but bubble
        /// up as is.
        /// </para>
        /// </remarks>
        public static void Require(params Proc[] checkers)
        {
            ExecuteCheck(checkers, Require);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="PreconditionException"/>
        /// on the first occurance of an unsatisfying element
        /// </summary>
        /// <remarks>
        /// The excpetion thrown will contain a message derived from the <paramref name="predicate"/> expression supplied.
        /// For example: 
        /// <c>"AccruedBenefit does not satisfy the predicate: pmt => pmt.IsValid"</c>
        /// </remarks>
        public static void RequireForEach<T>(this IEnumerable<T> source, Expression<Func<T, bool>> predicate)
        {
            CheckForEach(source, predicate, Require);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing an <see cref="PreconditionException"/>
        /// on the first occurance of an unsatisfying element
        /// </summary>
        public static void RequireForEach<T>(this IEnumerable<T> source, Func<T, bool> predicate, string message)
        {
            CheckForEach(source, predicate, ignoredItem => message, Require);
        }


        /// <summary>
        /// Tests each element in <paramref name="source"/> against <paramref name="predicate"/> throwing a <see cref="PreconditionException"/>
        /// on the first occurance of an unsatisfying element 
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="source">sequence containing the elements to test</param>
        /// <param name="predicate">The predicate test for an element</param>
        /// <param name="message">A function that accepts an unsatisfying element and should return the exception message</param>
        /// <exception cref="PreconditionException"/>
        public static void RequireForEach<T>(this IEnumerable<T> source,
                                             Func<T, bool> predicate,
                                             Func<T, string> message)
        {
            CheckForEach(source, predicate, message, Require);
        }

        #endregion


        [SkipFormatting]
        public class DisableDotNetAssertsScope : IDisposable
        {
            private readonly bool _currentValue = FireDotNetAsserts;


            public DisableDotNetAssertsScope()
            {
                FireDotNetAsserts = false;
            }


            public void Dispose()
            {
                FireDotNetAsserts = _currentValue;
            }
        }
    }



    /// <summary>
    /// Exception raised when a contract is broken. A broken contract indicates a programming error.
    /// Catch this exception type if you wish to differentiate between any DesignByContract
    /// exception and other runtime exceptions.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DesignByContractException : Exception
    {
        #region Member Variables

        protected readonly ICollection<Exception> _innerExceptions = new List<Exception>();

        #endregion


        #region Constructors

        protected DesignByContractException() {}
        protected DesignByContractException(string message) : base(message) {}
        protected DesignByContractException(string message, Exception inner) : this(message, new[] {inner}) {}


        protected DesignByContractException(string message, IEnumerable<Exception> inners)
            : base(message, inners != null ? inners.FirstOrDefault() : null)
        {
            if (inners == null) return;
            inners.ForEach(_innerExceptions.Add);
        }


#if !SILVERLIGHT
        protected DesignByContractException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif


        #endregion


        #region Properties

        public ICollection<Exception> InnerExceptions
        {
            get { return _innerExceptions; }
        }

        #endregion
    }



    /// <summary>
    /// Exception raised when a precondition fails.
    /// </summary>
    /// <remarks>
    /// <see cref="PreconditionException"/> is used to indicate that a precondition of a class
    /// method has been violated. A method that throws this exception has detected and is alterting
    /// of a programming error in the client code that is calling it. The client has not upheld its
    /// responsibility to only call the method with valid parameters or when the object receiving
    /// the method call is in an appropriate state.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PreconditionException : DesignByContractException
    {
        #region Constructors

        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException() {}


        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message) : base(message) {}


        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message, Exception inner) : base(message, inner) {}


        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message, IEnumerable<Exception> inner) : base(message, inner) {}


#if !SILVERLIGHT
        protected PreconditionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif

        #endregion
    }



    /// <summary>
    /// Exception raised when a postcondition fails.
    /// </summary>
    /// <remarks>
    /// <see cref="PostconditionException"/> is used to indicate that a postcondition of a class
    /// method has been violated. A method that throws this exception has detected and is alterting
    /// of a programming error in its code. The method has not upheld its responsibility to deliver
    /// its intended service, for example it has not returned a valid result.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PostconditionException : DesignByContractException
    {
        #region Constructors

        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException() {}


        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message) : base(message) {}


        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message, Exception inner) : base(message, inner) {}


        public PostconditionException(string message, IEnumerable<Exception> inners) : base(message, inners) {}


#if !SILVERLIGHT
        protected PostconditionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif


        #endregion
    }



    /// <summary>
    /// Exception raised when an invariant fails.
    /// </summary>
    /// <remarks>
    /// <see cref="InvariantException"/> indicates that an invariant of a class has been violated.
    /// An invariant is similar to a <see cref="PreconditionException"/>, being different only by
    /// virtue of the fact that an invariant is a condition that must at all times hold true for the
    /// class that has declared the invariant and is not specific to an individual method.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class InvariantException : DesignByContractException
    {
        #region Constructors

        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException() {}


        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message) : base(message) {}


        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message, Exception inner) : base(message, inner) {}


        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message, IEnumerable<Exception> inners) : base(message, inners) {}


#if !SILVERLIGHT
        protected InvariantException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif


        #endregion
    }



    /// <summary>
    /// Exception raised when an assertion fails.
    /// </summary>
    /// <remarks>
    /// <see cref="AssertionException"/> is used to indicate that an assumption in an internal
    /// method of a class has been violated. A class that throws this exception has detected and is
    /// alerting of a programming error in its code - a condition that the developer of the class
    /// has assumed never to happene has happened.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class AssertionException : DesignByContractException
    {
        #region Constructors

        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException() {}


        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message) : base(message) {}


        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message, Exception inner) : base(message, inner) {}


        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message, IEnumerable<Exception> inners) : base(message, inners) {}


#if !SILVERLIGHT
        protected AssertionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
#endif


        #endregion
    }
}