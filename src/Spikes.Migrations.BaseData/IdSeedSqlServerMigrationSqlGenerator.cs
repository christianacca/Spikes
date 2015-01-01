using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Web.Script.Serialization;

namespace Spikes.Migrations.BaseData
{
    public class IdSeedSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        private const int StandardSeedValue = 1;
        private const string MigrationConfigFileName = "migrations.json";
        private static int? _customIdentitySeed;
        private int _identitySeed = StandardSeedValue;

/*
        protected override void Generate(AlterTableOperation alterTableOperation)
        {
            // WARNING: This does not work - you cannot call DBCC CHECKIDENT to reseed an existing table with data
            // MAYBE: consider the following as an alternative: http://romiller.com/2013/04/30/ef6-switching-identity-onoff-with-a-custom-migration-operation/

            AnnotationValues values;
            alterTableOperation.Annotations.TryGetValue("CustomIdentitySeed", out values);
            bool isCustomIdSeedRequired = values != null && values.NewValue.ToString() == "True" && alterTableOperation.Columns.Any(c => c.IsIdentity);
            if (isCustomIdSeedRequired)
            {
                using (IndentedTextWriter w = Writer())
                {
                    const string sql = "DBCC CHECKIDENT ( {0}, RESEED, {1} )";
                    w.Write(sql, alterTableOperation.Name, GetCustomSeedValue());
                    Statement(w);
                }
            }
        }
*/

        private void EnforceCustomIdentitySeed(IDictionary<string, object> annotations, Action scope)
        {
            _identitySeed = annotations.ContainsKey("CustomIdentitySeed") ? GetCustomSeedValue() : StandardSeedValue;
            try
            {
                scope();
            }
            finally
            {
                _identitySeed = StandardSeedValue;
            }
        }

        private static int GetCustomSeedValue()
        {
            if (_customIdentitySeed.HasValue) return _customIdentitySeed.Value;

            string probingPath = AppDomain.CurrentDomain.BaseDirectory;
            string configFilePath = GetMigrationsConfigFileProbingPaths(probingPath).Where(File.Exists).FirstOrDefault();
            if (String.IsNullOrEmpty(configFilePath))
            {
                const string msg =
                    "Cannot find or access file '{0}' or '{0}.user' in the directory '{1}' or any of it's parent directories. Check that the file exists and has suitable read permissions";
                throw new InvalidOperationException(string.Format(msg, MigrationConfigFileName, probingPath));
            }

            var config =
                new JavaScriptSerializer().Deserialize<MigrationsConfig>(File.ReadAllText(configFilePath));
            if (!config.identitySeed.HasValue)
            {
                throw new InvalidOperationException("{0} missing an 'identitySeed' value");
            }
            _customIdentitySeed = config.identitySeed;
            return _customIdentitySeed.Value;
        }

        protected override void Generate(CreateTableOperation createTableOperation)
        {
            EnforceCustomIdentitySeed(createTableOperation.Annotations, () => base.Generate(createTableOperation));
        }

        private static IEnumerable<string> GetMigrationsConfigFileProbingPaths(string probingPath)
        {
            var dir = new DirectoryInfo(probingPath);
            while (dir != null)
            {
                yield return Path.Combine(dir.FullName, "migrations.json.user");
                yield return Path.Combine(dir.FullName, MigrationConfigFileName);

                try
                {
                    dir = dir.Parent;
                }
                catch (SecurityException)
                {
                    dir = null;
                }
            }
        }

        protected override void Generate(MoveTableOperation moveTableOperation)
        {
            EnforceCustomIdentitySeed(moveTableOperation.CreateTableOperation.Annotations,
                () => base.Generate(moveTableOperation));
        }

        protected override void Generate(ColumnModel column, IndentedTextWriter writer)
        {
            if (!column.IsIdentity)
            {
                base.Generate(column, writer);
                return;
            }
            var backingStore = new StringBuilder();
            var inmemoryWriter = new IndentedTextWriter(new StringWriter(backingStore));
            base.Generate(column, inmemoryWriter);
            string sql = backingStore.ToString();
            sql = sql.Replace("IDENTITY", string.Format("IDENTITY({0}, 1)", _identitySeed));
            writer.Write(sql);
        }
    }
}