using System;
using Eca.Commons.Data.NHibernate.Cfg;

namespace Eca.Commons.Data.NHibernate
{
    public abstract class DbMediaBuilder
    {
        #region Constructors

        protected DbMediaBuilder(DbConnectionInfo dbConnectionInfo)
        {
            DbConnectionInfo = dbConnectionInfo;
        }

        #endregion


        #region Properties

        public DbConnectionInfo DbConnectionInfo { get; set; }

        #endregion


        public abstract void CreateDatabaseMedia();


        #region Class Members

        public static DbMediaBuilder For(DatabaseEngine databaseEngine, DbConnectionInfo dbConnectionInfo)
        {
            switch (databaseEngine)
            {
                case DatabaseEngine.SQLite:
                    return new SqliteDbMediaBuilder(dbConnectionInfo);
                case DatabaseEngine.MsSqlCe:
                    return new SqlCeDbMediaBuilder(dbConnectionInfo);
                case DatabaseEngine.MsSql2005:
                    return new MsSqlServerDbMediaBuilder(dbConnectionInfo);
                default:
                    throw new ArgumentOutOfRangeException("databaseEngine");
            }
        }

        #endregion


        private class MsSqlServerDbMediaBuilder : DbMediaBuilder
        {
            #region Constructors

            public MsSqlServerDbMediaBuilder(DbConnectionInfo dbConnectionInfo) : base(dbConnectionInfo) {}

            #endregion


            public override void CreateDatabaseMedia()
            {
                SqlAdminQueries.CreateDatabase(DbConnectionInfo.ServerName, DbConnectionInfo.DatabaseName, true);
            }
        }



        private class SqlCeDbMediaBuilder : DbMediaBuilder
        {
            #region Constructors

            public SqlCeDbMediaBuilder(DbConnectionInfo dbConnectionInfo) : base(dbConnectionInfo) {}

            #endregion


            public override void CreateDatabaseMedia()
            {
                string filename;
                if (!DbConnectionInfo.DatabaseName.EndsWith(".sdf"))
                    filename = String.Format("{0}.sdf", DbConnectionInfo.DatabaseName);
                else
                {
                    filename = DbConnectionInfo.DatabaseName;
                }

                SqlCeDbHelper.CreateDatabaseFile(filename);
            }
        }



        private class SqliteDbMediaBuilder : DbMediaBuilder
        {
            #region Constructors

            public SqliteDbMediaBuilder(DbConnectionInfo dbConnectionInfo) : base(dbConnectionInfo) {}

            #endregion


            public override void CreateDatabaseMedia()
            {
                // nothing to do
            }
        }
    }
}