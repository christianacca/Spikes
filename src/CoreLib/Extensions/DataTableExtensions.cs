using System;
using System.Data;
using System.IO;

namespace Eca.Commons.Extensions
{
    public static class DataTableExtensions
    {
        #region Class Members

        /// <summary>
        /// Writes contents of datatable to a delimited stringwriter
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static StringWriter Delimit(this DataTable dt, string delimiter)
        {
            // Vars
            StringWriter sw;

            // Write table contents into string
            using (sw = new StringWriter())
            {
                // Write out header
                var headerColumns = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    headerColumns[i] = dt.Columns[i].ColumnName;
                }
                sw.WriteLine(String.Join(delimiter, headerColumns));

                // Write out each row
                foreach (DataRow row in dt.Rows)
                {
                    var rowValues = new string[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        rowValues[i] = (string) EnhancedConvertor.ChangeType(row[i], typeof (string));
                    }
                    sw.WriteLine(String.Join(delimiter, rowValues));
                }
            }

            return sw;
        }

        #endregion
    }
}