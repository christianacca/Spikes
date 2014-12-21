using System;

//
// Class: OrderByClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Eca.Commons.Data.SelectQueryBuilder
{
    /// <summary>
    /// Represents a ORDER BY clause to be used with SELECT statements
    /// </summary>
    [Serializable]
    public struct OrderByClause
    {
        public string FieldName;
        public Sorting SortOrder;


        public OrderByClause(string field)
        {
            FieldName = field;
            SortOrder = Sorting.Ascending;
        }


        public OrderByClause(string field, Sorting order)
        {
            FieldName = field;
            SortOrder = order;
        }


        public override string ToString()
        {
            return BuildSql();
        }


        public string BuildSql()
        {
            string orderByClause = "";
            switch (SortOrder)
            {
                case Sorting.Ascending:
                    orderByClause = FieldName + " ASC";
                    break;
                case Sorting.Descending:
                    orderByClause = FieldName + " DESC";
                    break;
            }
            return orderByClause;
        }
    }
}