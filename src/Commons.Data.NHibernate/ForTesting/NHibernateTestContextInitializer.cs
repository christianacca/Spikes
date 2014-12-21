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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Eca.Commons.Data.NHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using Environment = NHibernate.Cfg.Environment;

namespace Eca.Commons.Data.NHibernate.ForTesting
{
    public class NHibernateTestContextInitializer
    {
        #region Constructors

        internal NHibernateTestContextInitializer(MappingInfo mappingInfo)
        {
            if (mappingInfo == null)
            {
                throw new ArgumentNullException("mappingInfo");
            }
            MappingInfo = mappingInfo;
            DatabaseEngine = DatabaseEngine.SQLite;
            AfterInit = factory => {
                //do nothing by default
            };
        }

        #endregion


        #region Properties

        private Action<ISessionFactory> AfterInit { get; set; }
        private DbConnectionInfo ConnectionInfo { get; set; }
        private Action<Configuration> CustomConfiguration { get; set; }
        private DatabaseEngine DatabaseEngine { get; set; }
        private MappingInfo MappingInfo { get; set; }
        private IDictionary<string, string> NhConfigurationOverrideProperties { get; set; }

        #endregion


        /// <summary>
        /// Additional configuration for the NHibernate <see cref="Configuration"/> object that is going to be created
        /// </summary>
        /// <param name="configuration">A delegate that will be passed the <see cref="Configuration"/> object</param>
        /// <remarks>
        /// Any configurations performed by the <paramref name="configuration"/> delegate will take preference over any 
        /// <see cref="Configuration.Properties"/> supplied to <see cref="ConfiguredBy"/>.
        /// <para>
        /// You can also supply additional configuration using the <see cref="Eca.Commons.Data.NHibernate.Cfg.MappingInfo"/>
        /// supplied to <see cref="NHibernateTestContext.Initialize"/>
        /// </para>
        /// </remarks>
        public NHibernateTestContextInitializer AdditionalConfiguration(
            Action<Configuration> configuration)
        {
            CustomConfiguration = configuration;
            return this;
        }


        /// <summary>
        /// Code within <paramref name="afterInit"/> that should run immediately after the creation of the <see cref="ISessionFactory"/>.
        /// </summary>
        /// <param name="afterInit">A delegate that will be supplied the newly created <see cref="ISessionFactory"/></param>
        /// <remarks>
        /// <paramref name="afterInit"/> will <strong>not</strong> be executed if a session factory for the same database has already been 
        /// created and is stored in <see cref="NHibernateTestContext.Contexts"/> because of a previous invocation of 
        /// <see cref="NHibernateTestContext.Initialize"/>
        /// </remarks>
        public NHibernateTestContextInitializer AfterInitialization(Action<ISessionFactory> afterInit)
        {
            AfterInit = afterInit;
            return this;
        }


        /// <summary>
        /// Additional nhibernate configuration values to be assigned to <see cref="Configuration.Properties"/>
        /// </summary>
        /// <seealso cref="AdditionalConfiguration"/>
        public NHibernateTestContextInitializer ConfiguredBy(
            IDictionary<string, string> nhibernateConfigurationProperties)
        {
            NhConfigurationOverrideProperties = nhibernateConfigurationProperties;
            return this;
        }


        private NHibernateTestContext CreateContext()
        {
            INHibernateConfigurator configurator = CreateNhConfigurator();
            var context = new NHibernateTestContext(configurator, ConnectionInfo, DatabaseEngine);
            Debug.Print(String.Format("Created another UnitOfWorkContext for: {0}", context));
            return context;
        }


        private INHibernateConfigurator CreateNhConfigurator()
        {
            NHibernateConfigurator nhConfigurator = NHibernateConfigurator.Create(DatabaseEngine,
                                                                                  ConnectionInfo,
                                                                                  MappingInfo);
            IDictionary<string, string> additionalConfigProperties = NhConfigurationOverrideProperties ??
                                                                     new Dictionary<string, string>();
            //always use thread context to bind the current session as this makes sense in tests
            additionalConfigProperties[Environment.CurrentSessionContextClass] = "thread_static";

            nhConfigurator.AdditionalConfiguration(
                configs => configs.Properties.Merge(additionalConfigProperties));

            if (CustomConfiguration != null) nhConfigurator.AdditionalConfiguration(CustomConfiguration);

            return nhConfigurator;
        }


        protected internal NHibernateTestContext GetUnitOfWorkTestContext()
        {
            NHibernateTestContext context =
                NHibernateTestContext.Contexts.Find(x => x.DatabaseEngine == DatabaseEngine &&
                                                         x.ConnectionInfo == ConnectionInfo);
            if (context == null)
            {
                context = CreateContext();
                NHibernateTestContext.Contexts.Add(context);
            }
            return context;
        }


        private void InternalComplete()
        {
            ConnectionInfo = ConnectionInfo ?? new DbConnectionInfo();
            if (String.IsNullOrEmpty(ConnectionInfo.DatabaseName))
            {
                string databaseName = DeriveDatabaseNameFrom(DatabaseEngine,
                                                             MappingInfo.AssembliesWithEmbeddedMapping.
                                                                 ElementAt(0));
                ConnectionInfo.DatabaseName = databaseName;
            }
            NHibernateTestContext context = GetUnitOfWorkTestContext();

            if (!Equals(context, NHibernateTestContext.CurrentContext))
            {
                context.InitializeSessionFactory();
                AfterInit(context.SessionFactory);
            }
            NHibernateTestContext.CurrentContext = context;
            Debug.Print(String.Format("CurrentContext is: {0}", NHibernateTestContext.CurrentContext));
        }


        /// <summary>
        /// Signals the completion of specifying how NHibernate is to be initialised. Without calling this method, a 
        /// <see cref="NHibernateTestContext"/>, and its <see cref="ISessionFactory"/> that it is reponsible for, will 
        /// not be created .
        /// </summary>
        public void Now()
        {
            InternalComplete();
        }


        /// <param name="dbConnectionInfo">The database name and server (optional)</param>
        /// <param name="databaseEngine">The database engine that tests should be performed against</param>
        /// <remarks>
        /// If <paramref name="dbConnectionInfo"/> is <see langword="null" /> or returns <see langword="null" /> 
        /// or <see cref="string.Empty"/> for the <see cref="DbConnectionInfo.DatabaseName"/>, a database with a 
        /// name derived from the other parameters supplied will be created. See
        /// <see cref="DeriveDatabaseNameFrom"/>
        /// <para>
        /// <paramref name="databaseEngine"/> largely determines the configuration of nhibernate. The argument 
        /// supplied is used to pick from the predefined <see cref="NhCommonConfigurations"/>
        /// </para>
        /// </remarks>
        public NHibernateTestContextInitializer Using(DatabaseEngine databaseEngine, DbConnectionInfo dbConnectionInfo)
        {
            DatabaseEngine = databaseEngine;
            ConnectionInfo = dbConnectionInfo;

            return this;
        }


        #region Class Members

        public static string DeriveDatabaseNameFrom(DatabaseEngine databaseEngine, Assembly assembly)
        {
            if (databaseEngine == DatabaseEngine.SQLite)
                return NhCommonConfigurations.SQLiteDbName;
            else if (databaseEngine == DatabaseEngine.MsSqlCe)
                return "TempDB.sdf";
            else // we want to have a test DB and a real db, and we really don't want to override on by mistake
                return DeriveSqlServerDatabaseNameFrom(assembly) + "_Test";
        }


        public static string DeriveSqlServerDatabaseNameFrom(Assembly assembly)
        {
            string[] assemblyNameParts = assembly.GetName().Name.Split('.');
            if (assemblyNameParts.Length > 1)
                //assumes that the first part of the assmebly name is the Company name
                //and the second part is the Project name
                return assemblyNameParts[1];
            else
                return assemblyNameParts[0];
        }

        #endregion
    }
}