using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using CcAcca.EntityFramework.Migrations;
using CLAP;

namespace MultiMigrate
{
    public class ConsoleApp
    {
        private static string _startUpDirectory;
        private const string MigratorsHelp = "Specifies the path to a file that contains an array of json object. Each json object should at a include properties for 'Assembly' and optionally 'ConfigurationType' and 'ContextAssembly'. These properties corrospond to the command line parameters expected by the EF migrate.exe";
        private const string StartUpDirHelp = "Specifies the working directory of your application. If a relative path is supplied, this will be relative to the windows current directory";
        private const string StartUpConfigFileHelp = "Specifies the Web.config or App.config file of your application";
        private const string StartUpDataDirHelp = "Specifies the directory to use when resolving connection strings containing the |DataDirectory| substitution string.";
        private const string ConnectionStrNameHelp = "Specifies the name of the connection string to use from the specified configuration file. If omitted, the context's default connection will be used";
        private const string SkippedMigrationsHelp = "Names of migrations that should be skipped; supply a comma seperated list or the path of a file that contains a json array of strings";
        private const string Help = "Multi-Configure Migrations Command Line Utility. Applies any pending migrations to the database";

        [Global(Description = StartUpDirHelp)]
        // ReSharper disable once UnusedMember.Local
        static void StartupDirectory(string value)
        {
            // todo: make startupDirectory optional and default to the current directory

            _startUpDirectory = value;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveEfAssembly;
        }


        private static Assembly ResolveEfAssembly(object sender, ResolveEventArgs args)
        {
            if (new AssemblyName(args.Name).Name == "EntityFramework")
            {
                var assemblyPath = Path.Combine(_startUpDirectory, @"EntityFramework.dll");

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
            }

            return null;

        }

        [Verb(IsDefault = true, Description = Help)]
        public static void Upgrade([Required, Description(MigratorsHelp)] MigratorConfig[] migrators,
            [Required, Description(ConnectionStrNameHelp)]string connectionStringName,
            [Required, Description(StartUpConfigFileHelp)] string startUpConfigurationFile,
            [Description(StartUpDataDirHelp)]string startUpDataDirectory,
            [Description(SkippedMigrationsHelp)]string[] skippedMigrations)
        {
            // todo: remove requirement that connectionStringName and therefore startUpConfigurationFile is required
            // by allowing connectionString and providerName to be supplied as an alternative
            using (DbConnection cnn = CreateDbConnection(startUpConfigurationFile, connectionStringName))
            {
                List<DelegatedMigrator> ms =
                    migrators.Select(
                        c =>
                            CreateMigrator(startUpConfigurationFile, startUpDataDirectory, connectionStringName, c, cnn)
                        ).ToList();

                using (var migrationRunner = new MultiMigrateDbToLastestVsRunner(ms)
                {
                    SkippedMigrations = skippedMigrations
                })
                {
                    migrationRunner.Run();
                }
            }
        }

        private static DbConnection CreateDbConnection(string startUpConfigurationFile, string connectionStringName)
        {
            ConnectionStringSettings connectionString = 
                GetConnectionStringSetting(startUpConfigurationFile, connectionStringName);
            return DelegatedMigrator.CreateDbConnection(connectionString);
        }

        private static DelegatedMigrator CreateMigrator(string startUpConfigurationFile,
            string startUpDataDirectory, string connectionStringName, MigratorConfig config, DbConnection connection)
        {
            ToolingFacade facade = 
                CreateToolingFacade(config, startUpConfigurationFile, startUpDataDirectory, connectionStringName);
            var migrator = new DelegatedMigrator(facade.GetPendingMigrations, facade.GetDatabaseMigrations,
                migration => facade.Update(migration, true), (s, t) => facade.ScriptUpdate(s, t, true), connection,
                facade.Dispose)
            {
                IsAutoMigrationsEnabled = config.AutomaticMigrationsEnabled
            };
            return migrator;
        }

        private static ConnectionStringSettings GetConnectionStringSetting(string startUpConfigurationFile, string connectionStringName)
        {
            Configuration appConfig = LoadAppConfig(startUpConfigurationFile);
            return appConfig.ConnectionStrings.ConnectionStrings[connectionStringName];
        }

        private static Configuration LoadAppConfig(string startUpConfigurationFile)
        {
            if (startUpConfigurationFile.ToLower().EndsWith("web.config"))
            {
                string configPath = Path.Combine(GetWorkingDirectory(), startUpConfigurationFile);
                return WebConfigurationManager.OpenWebConfiguration(configPath);
            }
            else
            {
                string configPath = Path.Combine(GetWorkingDirectory(), Path.GetFileNameWithoutExtension(startUpConfigurationFile));
                return ConfigurationManager.OpenExeConfiguration(configPath);
            }
        }

        private static ToolingFacade CreateToolingFacade(MigratorConfig config, string startUpConfigurationFile, string startUpDataDirectory, string connectionStringName)
        {
            string workingDirectory = null;
            if (!String.IsNullOrWhiteSpace(_startUpDirectory))
            {
                workingDirectory = GetWorkingDirectory();
            }

            return new ToolingFacade(config.Assembly, config.ContextAssembly, config.ConfigurationType, workingDirectory,
                Path.Combine(workingDirectory, startUpConfigurationFile), startUpDataDirectory,
                new DbConnectionInfo(connectionStringName)
                );
        }

        private static string GetWorkingDirectory()
        {
            return Path.IsPathRooted(_startUpDirectory)
                ? _startUpDirectory
                : Path.Combine(Environment.CurrentDirectory, _startUpDirectory);
        }
    }
}