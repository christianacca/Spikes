#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion


using System;
using System.Collections.Generic;
using Eca.Commons.Data.NHibernate.Cfg;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Eca.Commons.Data.NHibernate.ForTesting
{
    /// <summary>
    /// Sets up and manages the NHibernate <see cref="ISessionFactory"/>, ensuring that only one is created for a given 
    /// combination of <see cref="DatabaseEngine"/> and database name. Each test class that requires NHibernate should
    /// call <see cref="Initialize"/> as demonstrated in the example below.
    /// </summary>
    /// <remarks>
    /// Each <see cref="NHibernateTestContext"/> created to manage a <see cref="ISessionFactory"/> is stored in 
    /// <see cref="Contexts"/> and the most recent one requested is assigned to <see cref="CurrentContext"/>.
    /// </remarks>
    /// <example>
    /// <code lang="c#" escaped="true">
    /// using Eca.Commons.Data.NHibernate.Cfg;
    /// using Eca.Commons.Data.NHibernate.ForTesting;
    /// using Eca.Commons.Testing.NHibernate;
    /// using NHibernate;
    /// using NUnit.Framework;
    /// 
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///		ISession _session;
    /// 
    ///		[TestFixtureSetup]
    ///		public void TestFixtureSetup()
    ///		{
    ///			NHibernateTestContext.Initialize(MappingInfo.ForAssemblyContaining&lt;Foo&gt;())
    ///			    .Using(DatabaseEngine.SQLite, null)
    ///			    .Now();
    ///		}
    /// 
    ///		[Setup]
    ///		public void TestSetup()
    ///		{
    ///			TestConnectionProvider.CloseDatabase(); //just in case
    ///			_session = NHibernateTestContext.CurrentContext.CreateSession();
    ///		}
    /// 
    ///		[TearDown]
    ///		public void TestTearDown()
    ///		{
    ///			CurrentSessionContext.Unbind(NHibernateTestContext.CurrentContext.SessionFactory);
    ///			_session.Dispose();
    ///			TestConnectionProvider.CloseDatabase();
    ///		}
    /// 
    ///		[Test]
    ///		public void CanSaveFoo()
    ///		{
    ///			Foo f = new Foo();
    ///			f.Name = "Bar";
    /// 
    ///			using(ITransaction tx = _session.BeginTransaction())
    ///			{
    ///			    _session.Save(f);
    ///			    _session.Flush();
    ///			    tx.Commit();
    ///			}
    /// 
    ///			_session.Clear();
    ///			Foo fooFromDb = _session.Get(f.Id);
    /// 
    ///			Assert.IsNotNull(fooFromDb);
    ///			Assert.AreEqual("Bar", fooFromDb.Name);
    ///		}
    /// }
    /// </code>
    /// </example>
    public class NHibernateTestContext : IDisposable
    {
        #region Member Variables

        private ISessionFactory _factory;

        #endregion


        #region Constructors

        public NHibernateTestContext(INHibernateConfigurator nhConfigurator,
                                     DbConnectionInfo connectionInfo,
                                     DatabaseEngine databaseEngine)
        {
            NhConfigurator = nhConfigurator;
            ConnectionInfo = connectionInfo;
            DatabaseEngine = databaseEngine;
        }

        #endregion


        #region Properties

        public DbConnectionInfo ConnectionInfo { get; private set; }
        public DatabaseEngine DatabaseEngine { get; set; }
        private INHibernateConfigurator NhConfigurator { get; set; }

        public virtual ISessionFactory SessionFactory
        {
            get { return _factory; }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            //nothing to do
        }

        #endregion


        /// <summary>
        /// Opens an NHibernate session and creates the db schema.
        /// </summary>
        /// <returns>The open NHibernate session.</returns>
        public virtual ISession CreateSession()
        {
            ISession session = SessionFactory.OpenSession();
            SetupDatabase();
            return session;
        }


        public void InitializeSessionFactory()
        {
            _factory = NhConfigurator.NHibernateConfiguration.BuildSessionFactory();
        }


        /// <summary>
        /// Calls <see cref="SetupDatabase(bool)"/> supplying false as the parameter value
        /// </summary>
        public void SetupDatabase()
        {
            SetupDatabase(false);
        }


        /// <summary>
        /// Creates the physical database and its schema as defined by the nhibernate mappings
        /// </summary>
        /// <param name="outputScript">Output the DDL statements to the console?</param>
        public void SetupDatabase(bool outputScript)
        {
            DbMediaBuilder.For(DatabaseEngine, ConnectionInfo).CreateDatabaseMedia();
            new SchemaExport(NhConfigurator.NHibernateConfiguration).Execute(outputScript, true, false);
        }


        #region Overridden object methods

        public override string ToString()
        {
            return
                String.Format("DatabaseEngine: {0}; Database: {1}",
                              DatabaseEngine,
                              ConnectionInfo);
        }

        #endregion


        #region Class Members

        /// <summary>
        /// All contexts that have been created during the course of a single test run.
        /// Generally, you can ignore these as you will be only interested in the <see cref="CurrentContext"/>
        /// </summary>
        public static List<NHibernateTestContext> Contexts = new List<NHibernateTestContext>();

        /// <summary>
        /// Holds a reference to the <see cref="ISessionFactory"/> that is available for the current
        /// running test
        /// </summary>
        public static NHibernateTestContext CurrentContext;


        /// <summary>
        /// Throw away all <see cref="NHibernateTestContext"/> objects within <see cref="Contexts"/>
        /// and referenced by <see cref="CurrentContext"/>. WARNING: Subsequent calls to  <see
        /// cref="Initialize"/> will now take considerably longer as the persistent framework 
        /// will be initialised a fresh.
        /// </summary>
        /// <remarks>
        /// This method should be used vary sparingly. It is highly unlikely that you will need to
        /// call this method between every test.
        /// </remarks>
        public static void DisposeAndRemoveAllUoWTestContexts()
        {
            foreach (NHibernateTestContext context in Contexts)
                context.Dispose();

            CurrentContext = null;
            Contexts.Clear();
        }


        /// <summary>
        /// Builds the Nhibernate <see cref="ISessionFactory"/> using a Fluent builder. The session factory thus built
        /// is assigned to the <see cref="CurrentContext"/>.
        /// </summary>
        /// <param name="mappingInfo">Information used to map classes to database tables and queries.</param>
        /// <remarks>
        /// Initialisation of nhibernate will only happen once for the same database.
        /// So for example, the second time <see cref="Initialize"/> is called with the same database engine and name,
        /// a new nhibernate session factory will <strong>NOT</strong> be created. 
        /// The second call to <see cref="Initialize"/> will simply set the <see cref="CurrentContext"/> to 
        /// one already created and stored in <see cref="Contexts"/>.
        /// </remarks>
        public static NHibernateTestContextInitializer Initialize(MappingInfo mappingInfo)
        {
            return new NHibernateTestContextInitializer(mappingInfo);
        }

        #endregion
    }
}