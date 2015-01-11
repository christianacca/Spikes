using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Design;
using System.IO;
using System.Linq;
using System.Reflection;
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
            [Description(StartUpConfigFileHelp)] string startUpConfigurationFile,
            [Description(StartUpDataDirHelp)]string startUpDataDirectory,
            [Description(ConnectionStrNameHelp)]string connectionStringName,
            [Description(SkippedMigrationsHelp)]string[] skippedMigrations)
        {
            List<DelegatedMigrator> ms =
                migrators.Select(
                    c =>
                        CreateDelegatedMigrator(_startUpDirectory, startUpConfigurationFile, startUpDataDirectory,
                            connectionStringName, c)).ToList();

            using (var migrationRunner = new MultiMigrateDbToLastestVsRunner(ms)
            {
                SkippedMigrations = skippedMigrations
            })
            {
                migrationRunner.Run();
            }
        }

        private static DelegatedMigrator CreateDelegatedMigrator(string startUpDirectory,
            string startUpConfigurationFile,
            string startUpDataDirectory, string connectionStringName, MigratorConfig config)
        {
            ToolingFacade facade = CreateToolingFacade(config, startUpDirectory, startUpConfigurationFile,
                startUpDataDirectory, connectionStringName);
            return new DelegatedMigrator(facade.GetPendingMigrations, migration => facade.Update(migration, true),
                () => facade.Dispose())
            {
                IsAutoMigrationsEnabled = config.AutomaticMigrationsEnabled
            };
        }

        private static ToolingFacade CreateToolingFacade(MigratorConfig config, string startUpDirectory, string startUpConfigurationFile, string startUpDataDirectory, string connectionStringName)
        {
            string workingDirectory = null;
            if (!String.IsNullOrWhiteSpace(startUpDirectory))
            {
                workingDirectory = Path.IsPathRooted(startUpDirectory)
                    ? startUpDirectory
                    : Path.Combine(Environment.CurrentDirectory, startUpDirectory);
            }

            return new ToolingFacade(config.Assembly, config.ContextAssembly, config.ConfigurationType, workingDirectory,
                Path.Combine(workingDirectory, startUpConfigurationFile), startUpDataDirectory,
                new DbConnectionInfo(connectionStringName)
                );
        }
    }
}