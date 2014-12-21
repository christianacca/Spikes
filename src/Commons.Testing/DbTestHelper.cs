using System.Data;
using Eca.Commons.Data;
using Eca.Commons.Data.SelectQueryBuilder;
using Eca.Commons.Extensions;

namespace Eca.Commons.Testing
{
    public class DbTestHelper
    {
        #region Class Members

        static DbTestHelper()
        {
            Db = new DbInstance();
        }


        public static DbInstance Db { get; set; }

        #endregion


        public class DbInstance
        {
            #region Properties

            public string Connection { get; set; }

            #endregion


            public int GetRowCount(string table, params WhereClause[] whereClauses)
            {
                var sql = new SelectQueryBuilder();
                sql.SelectColumn("*");
                sql.SelectFromTable(table);
                whereClauses.ForEach(w => sql.AddWhere(w));

                DataTable executeDataTable
                    = SimpleDataAccess.ExecuteDataTable(cmd => {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql.BuildQuery();
                    },
                                                        Connection);

                return executeDataTable.Rows.Count;
            }
        }
    }
}