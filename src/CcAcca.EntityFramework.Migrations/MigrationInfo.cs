using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    /// Represent the information about a individual migration
    /// </summary>
    public class MigrationInfo
    {
        /// <summary>
        /// The created date of this migration
        /// </summary>
        public DateTime CreatedOn { get; set; }

        public string FullName { get; set; }
        /// <summary>
        /// Whether this migration represents a migration that will be created by auto-migrations
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// Whether this migration should be skipped when updating a database
        /// </summary>
        public bool IsSkipped { get; set; }

        /// <summary>
        /// The friendly name of a migration
        /// </summary>
        public string Name { get; set; }

        public static MigrationInfo Auto
        {
            get { return new MigrationInfo { IsAuto = true, CreatedOn = DateTime.MaxValue }; }
        }

        public override string ToString()
        {
            const string format = "Name: {0}, CreatedOn: {1}, IsAuto: {2}, IsSkipped: {3}";
            return String.Format(format, Name, CreatedOn, IsAuto, IsSkipped);
        }

        public static Parser CreateParser(IEnumerable<string> skippedMigrations)
        {
            return new Parser(skippedMigrations);
        }

        public class Parser
        {
            public Parser(IEnumerable<string> skippedMigrations)
            {
                SkippedMigrations = skippedMigrations ?? Enumerable.Empty<string>();
            }

            private IEnumerable<string> SkippedMigrations { get; set; }

            public MigrationInfo Parse(string rawName)
            {
                string[] nameParts = rawName.Split(new[] { "_" }, StringSplitOptions.None);
                return new MigrationInfo
                {
                    Name = nameParts[1],
                    FullName = rawName,
                    CreatedOn = DateTime.ParseExact(nameParts[0], "yyyyMMddHHmmssFFF", CultureInfo.InvariantCulture),
                    IsSkipped = SkippedMigrations.Contains(rawName)
                };
            }
        }
    }
}