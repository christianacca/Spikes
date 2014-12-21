using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eca.Commons.Data.NHibernate.Cfg;
using Eca.Commons.Extensions;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Engine;

namespace Eca.Commons.Data.NHibernate
{
    public class NHibernateDbBuilder : INHibernateDbBuilder
    {
        #region Constructors

        public NHibernateDbBuilder(DbMediaBuilder dbMediaBuilder, INHibernateConfigurator nhConfigurator)
        {
            DbMediaBuilder = dbMediaBuilder;
            NhConfigurator = nhConfigurator;
        }

        #endregion


        #region Properties

        private DbMediaBuilder DbMediaBuilder { get; set; }


        private INHibernateConfigurator NhConfigurator { get; set; }

        #endregion


        #region INHibernateDbBuilder Members

        /// <summary>
        /// Creates the database, deleting any existing database with the same name
        /// </summary>
        public virtual void CreateDatabase()
        {
            CreateDatabaseMedia();
            CreateDatabaseSchema();
        }


        /// <summary>
        /// Creates the database, deleting any existing database with the same name
        /// </summary>
        /// <param name="connectionOverride">The database name and/or server that should override the current NHibernate configuration</param>
        public virtual void CreateDatabase(DbConnectionInfo connectionOverride)
        {
            DbConnectionInfo overriddenConnectionInfo = DbMediaBuilder.DbConnectionInfo.Merge(connectionOverride);
            DbMediaBuilder.DbConnectionInfo = overriddenConnectionInfo;
            NhConfigurator.NHibernateConfiguration.SetDbConnectionInfo(overriddenConnectionInfo);
            CreateDatabaseMedia();
            CreateDatabaseSchema();
        }


        public virtual Configuration NHibernateConfiguration
        {
            get { return NhConfigurator.NHibernateConfiguration; }
        }

        #endregion


        /// <summary>
        /// Instructs the builder that there are assemblies containing NHibernate.Validator 
        /// definitions whose DDL constraints are to be used when creating the database schema
        /// </summary>
        public void AddValidationDatabaseContraints(IEnumerable<string> validationAssemblies)
        {
            var assembliesNames = validationAssemblies.Safe().Where(name => !String.IsNullOrEmpty(name));
            var assemblies = assembliesNames.Select(n => Assembly.Load(n));
            List<Type> validationDefinitionTypes =
                assemblies.SelectMany(assembly => assembly.ValidationDefinitions()).ToList();

            if (validationDefinitionTypes.Count == 0) return;

            var validatorConfiguration = new FluentConfiguration();
            validatorConfiguration
                .Register(validationDefinitionTypes)
                .SetDefaultValidatorMode(ValidatorMode.UseExternal)
                .IntegrateWithNHibernate.ApplyingDDLConstraints();

            var validatorEngine = new ValidatorEngine();
            validatorEngine.Configure(validatorConfiguration);

            NHibernateConfiguration.Initialize(validatorEngine);
        }


        public virtual void CreateDatabaseMedia()
        {
            DbMediaBuilder.CreateDatabaseMedia();
        }


        /// <summary>
        /// Create the database schema, dropping any existing tables and other database objects that
        /// NHibernate has been made aware of via mapping
        /// </summary>
        public void CreateDatabaseSchema()
        {
            new SchemaExport(NhConfigurator.NHibernateConfiguration).Execute(true, true, false);
        }


        #region Class Members

        private static string ConvertToAbsoluteConfigFilePath(string filePath)
        {
            if (String.IsNullOrEmpty(filePath)) return GetDefaultConfigFilePath();
            if (Path.IsPathRooted(filePath)) return filePath;

            //must be a relative path
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        }


        public static NHibernateDbBuilder For(DatabaseEngine databaseEngine,
                                              DbConnectionInfo dbConnectionInfo,
                                              MappingInfo mappingInfo)
        {
            return new NHibernateDbBuilder(DbMediaBuilder.For(databaseEngine, dbConnectionInfo),
                                           NHibernateConfigurator.Create(databaseEngine, dbConnectionInfo, mappingInfo));
        }


        /// <summary>
        /// Create a <see cref="NHibernateDbBuilder"/> configured with the nhibernate config file supplied
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a relative path is supplied, the path is considered relative to the executable file that is calling
        /// this method
        /// </para>
        /// <para>
        /// If <paramref name="filePath"/> is null or empty, then a search will be made for configuration
        /// in the following locations: <em>hibernate.config</em>, <em>hibernate.cfg.xml</em>; the first file found 
        /// in the order specified will be selected
        /// </para>
        /// </remarks>
        public static NHibernateDbBuilder For(string filePath)
        {
            filePath = ConvertToAbsoluteConfigFilePath(filePath);

            Configuration cfg = new Configuration().Configure(filePath);

            DatabaseEngine? engine = cfg.GetDatabaseEngine();
            if (engine == null)
            {
                throw new InvalidOperationException(
                    string.Format("Database engine cannot be determined from the config file '{0}'", filePath));
            }

// ReSharper disable ConstantNullCoalescingCondition
            DbMediaBuilder dbMediaBuilder = DbMediaBuilder.For(engine ?? DatabaseEngine.MsSql2005,
                                                               cfg.GetDbConnectionInfo());
// ReSharper restore ConstantNullCoalescingCondition
            return new NHibernateDbBuilder(dbMediaBuilder, new NHibernateSimpleConfigurator(cfg));
        }


        /// <summary>
        /// Create a <see cref="NHibernateDbBuilder"/> configured with the default nhibernate config file
        /// selected according to the rules specified by <see cref="For(string)"/>
        /// </summary>
        /// <seealso cref="For(string)"/>
        public static NHibernateDbBuilder ForDefaultConfigFile()
        {
            return For(null);
        }


        private static string GetDefaultConfigFilePath()
        {
            string configFileName = new[] {"hibernate.config", "hibernate.cfg.xml"}
                .FirstOrDefault(fileName => File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)));

            if (String.IsNullOrEmpty(configFileName)) return null;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
        }

        #endregion
    }
}