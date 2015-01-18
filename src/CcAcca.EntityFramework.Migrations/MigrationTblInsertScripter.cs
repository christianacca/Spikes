using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    /// Extracts the SQL INSERT statement that inserts a specific migration into the MigrationsHistory table
    /// </summary>
    public class MigrationTblInsertScripter
    {
        private readonly Func<string, string, string> _scriptUpdateImpl;
        private string _defaultSchemaName;

        public MigrationTblInsertScripter(Func<string, string, string> scriptUpdateImpl)
        {
            _scriptUpdateImpl = scriptUpdateImpl;
        }

        public string GetMigrationHistoryInsertSql(string previousMigrationName, string migrationName)
        {
            // note: detecting and fixing default schema name is a workaround to a 
            // bug in ef (see: https://entityframework.codeplex.com/workitem/1871)
            DetectDefaultSchameName();

            List<string> migrationTblInsertLines = GetMigrationHistoryInsertSqlLines(previousMigrationName, migrationName);
            migrationTblInsertLines = FixupDefaultSchemaName(migrationTblInsertLines).ToList();

            var migrationTblInsertSql = new StringBuilder(1000);
            migrationTblInsertLines.ForEach(line => migrationTblInsertSql.AppendLine(line));

            return migrationTblInsertSql.ToString();
        }

        private IEnumerable<string> FixupDefaultSchemaName(IList<string> migrationTblInsertSql)
        {
            string schemaName = GetSchemaName(migrationTblInsertSql);
            if (String.Equals(schemaName, _defaultSchemaName))
            {
                yield return migrationTblInsertSql.First();
            }
            else
            {
                yield return
                    migrationTblInsertSql.First()
                        .Replace(string.Format("[{0}]", schemaName), string.Format("[{0}]", _defaultSchemaName));
            }
            yield return migrationTblInsertSql.Last();
        }

        private void DetectDefaultSchameName()
        {
            if (_defaultSchemaName != null) return;

            List<string> sqlLines = GetMigrationHistoryInsertSqlLines(null, null);
            _defaultSchemaName = GetSchemaName(sqlLines);
        }

        private List<string> GetMigrationHistoryInsertSqlLines(string previousMigrationName, string migrationName)
        {
            string sqlScript = _scriptUpdateImpl(previousMigrationName, migrationName);
            string[] allLines = sqlScript.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            List<string> migrationTblInsertLines =
                allLines.SliceWhen((prev, current) => current.ToLower().Contains("insert")).Last().ToList();
            return migrationTblInsertLines;
        }

        private static string GetSchemaName(IEnumerable<string> sqlLines)
        {
            string containsSchemaName = sqlLines.First();
            Match matches = Regex.Match(containsSchemaName, @"INSERT\s+\[(\w+)\]");
            return matches.Success ? matches.Groups[1].Value : null;
        }
    }
}