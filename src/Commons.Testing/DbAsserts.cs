using System;
using Eca.Commons.Data.SelectQueryBuilder;
using Eca.Commons.Extensions;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    public class DbAsserts
    {
        #region Class Members

        static DbAsserts()
        {
            Db = new DbInstance();
        }


        public static DbInstance Db { get; set; }

        #endregion


        public class DbInstance
        {
            #region Member Variables

            private readonly DbTestHelper.DbInstance _dbTestHelper;

            #endregion


            #region Constructors

            public DbInstance()
            {
                _dbTestHelper = new DbTestHelper.DbInstance();
            }

            #endregion


            #region Properties

            public string Connection
            {
                get { return _dbTestHelper.Connection; }
                set { _dbTestHelper.Connection = value; }
            }

            #endregion


            public void RowsExist(string tableName, int rowsExpected, params WhereClause[] where)
            {
                string printedWhereClauses = where.Safe().Join(" And ", w => w.BuildSql());

                Assert.That(_dbTestHelper.GetRowCount(tableName, where),
                            Is.EqualTo(rowsExpected),
                            String.Format("Row count in table '{0}' for Where: {1}", tableName, printedWhereClauses));
            }
        }
    }
}