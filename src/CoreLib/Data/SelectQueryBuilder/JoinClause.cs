using System;

//
// Class: JoinClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Eca.Commons.Data.SelectQueryBuilder
{
    /// <summary>
    /// Represents a JOIN clause to be used with SELECT statements
    /// </summary>
    [Serializable]
    public struct JoinClause
    {
        public JoinType JoinType;
        public string FromTable;
        public string FromTableAlias;
        public string FromColumn;
        public Comparison ComparisonOperator;
        public string ToTable;
        public string ToTableAlias;
        public string ToColumn;
        public string OnClause;


        public JoinClause(JoinType join, string toTableName, string onClause) : this()
        {
            JoinType = join;
            ToTable = toTableName;
            OnClause = onClause ?? String.Empty;
        }


        public JoinClause(JoinType join,
                          string toTableName,
                          string toColumnName,
                          Comparison @operator,
                          string fromTableName,
                          string fromColumnName) : this()
        {
            JoinType = join;
            FromTable = fromTableName ?? String.Empty;
            FromTableAlias = null;
            FromColumn = fromColumnName;
            ComparisonOperator = @operator;
            ToTable = toTableName ?? String.Empty;
            ToTableAlias = null;
            ToColumn = toColumnName;

            if (FromTable.Contains(" "))
            {
                var nameAndAlias = FromTable.Split(new[] {' '});
                FromTableAlias = nameAndAlias[1];
            }

            if (ToTable.Contains(" "))
            {
                var nameAndAlias = ToTable.Split(new[] {' '});
                ToTableAlias = nameAndAlias[1];
            }
        }


        public override string ToString()
        {
            return BuildSql();
        }


        public string BuildSql()
        {
            string result = "";
            switch (JoinType)
            {
                case JoinType.InnerJoin:
                    result = "INNER JOIN";
                    break;
                case JoinType.OuterJoin:
                    result = "OUTER JOIN";
                    break;
                case JoinType.LeftJoin:
                    result = "LEFT JOIN";
                    break;
                case JoinType.RightJoin:
                    result = "RIGHT JOIN";
                    break;
            }
            result += " " + ToTable + " ON ";
            result += BuildOnClause();
            return result;
        }


        private string BuildOnClause()
        {
            if (!String.IsNullOrEmpty(OnClause)) return OnClause;
            return WhereStatement.CreateComparisonClause(FriendlyFromTableName + '.' + FromColumn,
                                                         ComparisonOperator,
                                                         new SqlLiteral(FriendlyToTableName + '.' +
                                                                        ToColumn));
        }


        private string FriendlyFromTableName
        {
            get { return FromTableAlias ?? FromTable; }
        }

        private string FriendlyToTableName
        {
            get { return ToTableAlias ?? ToTable; }
        }
    }
}