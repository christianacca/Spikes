using System;
using Eca.Commons.Data.NHibernate.Cfg;
using Eca.Commons.Data.NHibernate.ForTesting;
using Eca.Commons.Dates;
using NHibernate;
using NHibernate.Context;
using NUnit.Framework;

namespace Eca.Commons.Testing.NHibernate
{
    [TestFixture, Category("NHibernate tests")]
    public abstract class NHibernateTestsBase<T> where T : class
    {
        protected EquivalenceComparer _comparer;


        #region Setup/Teardown

        [SetUp]
        public virtual void TestInitialize()
        {
            EquivalenceComparer.GlobalFilter =
                EquivalenceComparer.GlobalFilter.Union(NhAssert.PropertiesToIgnoreForDbComparison);
            _comparer = CreateComparer();

            TestConnectionProvider.CloseDatabase(); //just in case

            //ensure Nh by default is using the same session as the one started by our tests
            Nh.CurrentSession = CreateNHibernateSession();
        }


        [TearDown]
        public virtual void TeatCleanup()
        {
            CleanupNHibernateSession();

            TestConnectionProvider.CloseDatabase();

            EquivalenceComparer.ClearGlobalFilter();
            Clock.ResetToSystemClock();
        }

        #endregion


        protected NHibernateTestsBase()
        {
            DefaultDatabaseEngine = DatabaseEngine.SQLite;
        }


        #region Test helpers

        /// <summary>
        /// Convenient shortcut for accessing the <see cref="ISessionFactory"/> that has been initialised for the currently executing test
        /// </summary>
        public ISessionFactory CurrentSessionFactory
        {
            get { return NHibernateTestContext.CurrentContext.SessionFactory; }
        }


        /// <summary>
        /// Use this property to define the default database engine that you can use when initialising nhibernate.
        /// Defaults to <see cref="DatabaseEngine.SQLite"/>
        /// </summary>
        public DatabaseEngine DefaultDatabaseEngine { get; set; }


        protected void AssertAreEquivalent(T o1, T o2)
        {
            NhAssert.AssertAreEquivalent(o1, o2, _comparer);
        }


        protected void AssertAreEquivalent<TOther>(TOther o1, TOther o2) where TOther : class
        {
            NhAssert.AssertAreEquivalent(o1, o2, EquivalenceComparer.For<TOther>());
        }


        protected virtual void CleanupNHibernateSession()
        {
            CurrentSessionContext.Unbind(NHibernateTestContext.CurrentContext.SessionFactory);
            Nh.DisposeCurrentSession();
        }


        protected virtual EquivalenceComparer CreateComparer()
        {
            return EquivalenceComparer.For<T>();
        }


        /// <summary>
        /// Create NHibernate session and database
        /// </summary>
        /// <remarks>
        /// The <see cref="ISession"/> that is created and being returned, is made available globally to all code by 
        /// binding it to the <see cref="CurrentSessionContext"/>. By default this current session context will be 
        /// the current executing thread. 
        /// <para>
        /// To access this global <see cref="ISession"/> (for example within your 
        /// repositories), call <see cref="ISessionFactory.GetCurrentSession"/>. 
        /// Use the property <see cref="CurrentSessionFactory"/> in your tests to access the session factory.
        /// </para>
        /// </remarks>
        protected virtual ISession CreateNHibernateSession()
        {
            ISession session = NHibernateTestContext.CurrentContext.CreateSession();
            CurrentSessionContext.Bind(session);
            return session;
        }


        protected void VerifyIsEquivalentToObjectInDatabase(T entity)
        {
            NhAssert.VerifyIsEquivalentToObjectInDatabase(entity, _comparer);
        }


        protected void VerifyIsEquivalentToObjectInDatabase<TOther>(TOther entity) where TOther : class
        {
            NhAssert.VerifyIsEquivalentToObjectInDatabase(entity, EquivalenceComparer.For<TOther>());
        }

        #endregion
    }



    public abstract class NHibernateTestsBase : NHibernateTestsBase<object> {}
}