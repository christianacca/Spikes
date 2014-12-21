using System;
using System.Collections.Generic;
using Eca.Commons.Extensions;

//
// Class: SelectQueryBuilder
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework. This framework also contains
// the UpdateQueryBuilder, InsertQueryBuilder and DeleteQueryBuilder.
// You can download the framework DLL at http://www.code-engine.com/
// 

namespace Eca.Commons.Data.SelectQueryBuilder
{
    [Serializable]
    public class SelectQueryBuilder : IQueryBuilder
    {
        #region Member Variables

        protected bool _distinct;
        protected List<string> _groupByColumns = new List<string>(); // array of string
        protected WhereStatement _havingStatement = new WhereStatement();
        protected List<JoinClause> _joins = new List<JoinClause>(); // array of JoinClause
        protected List<OrderByClause> _orderByStatement = new List<OrderByClause>(); // array of OrderByClause
        protected List<string> _selectedColumns = new List<string>(); // array of string
        protected List<string> _selectedTables = new List<string>(); // array of string
        protected TopClause _topClause = new TopClause(100, TopUnit.Percent);
        protected WhereStatement _whereStatement = new WhereStatement();

        #endregion


        #region Properties

        public bool Distinct
        {
            get { return _distinct; }
            set { _distinct = value; }
        }

        public bool HasOrderBy
        {
            get { return _orderByStatement.Count > 0; }
        }

        public WhereStatement Having
        {
            get { return _havingStatement; }
            set { _havingStatement = value; }
        }

        public IEnumerable<JoinClause> Joins
        {
            get { return _joins; }
        }

        public string OrderByClause
        {
            get { return BuildOrderBySql(); }
        }

        public string[] SelectedColumns
        {
            get
            {
                if (_selectedColumns.Count > 0)
                    return _selectedColumns.ToArray();
                else
                    return new[] {"*"};
            }
        }

        public string[] SelectedTables
        {
            get { return _selectedTables.ToArray(); }
        }

        public TopClause TopClause
        {
            get { return _topClause; }
            set { _topClause = value; }
        }

        public int TopRecords
        {
            get { return _topClause.Quantity; }
            set
            {
                _topClause.Quantity = value;
                _topClause.Unit = TopUnit.Records;
            }
        }

        public WhereStatement Where
        {
            get { return _whereStatement; }
            set { _whereStatement = value; }
        }

        internal WhereStatement WhereStatement
        {
            get { return _whereStatement; }
            set { _whereStatement = value; }
        }

        #endregion


        #region IQueryBuilder Members

        /// <summary>
        /// Builds the select query
        /// </summary>
        /// <returns>Returns a string containing the query, or a DbCommand containing a command with parameters</returns>
        public string BuildQuery()
        {
            // Check if a Having clause set without a Group By Clause
            if (_havingStatement.ClauseLevels > 0 && _groupByColumns.Count == 0)
            {
                throw new Exception("Having statement was set without Group By");
            }

            string result = BuildSelectPreambleSql() +
                            _topClause.BuildSql() +
                            BuildColumnNamesSql() +
                            BuildTableNamesSql() +
                            BuildJoinSql() +
                            BuildWhereSql() +
                            BuildGroupBySql() +
                            BuildHavingSql() +
                            BuildOrderBySql();
            return result;
        }

        #endregion


        public void AddHaving(WhereClause clause)
        {
            AddHaving(clause, 1);
        }


        public void AddHaving(WhereClause clause, int level)
        {
            _havingStatement.Add(clause, level);
        }


        public WhereClause AddHaving(string field, Comparison @operator, object compareValue)
        {
            return AddHaving(field, @operator, compareValue, 1);
        }


        public WhereClause AddHaving(Enum field, Comparison @operator, object compareValue)
        {
            return AddHaving(field.ToString(), @operator, compareValue, 1);
        }


        public WhereClause AddHaving(string field, Comparison @operator, object compareValue, int level)
        {
            var newWhereClause = new WhereClause(field, @operator, compareValue);
            _havingStatement.Add(newWhereClause, level);
            return newWhereClause;
        }


        public void AddJoin(JoinClause newJoin)
        {
            AddJoin(newJoin, false);
        }


        public void AddJoin(JoinClause newJoin, bool ignoreIfAlreadyAdded)
        {
            if (ignoreIfAlreadyAdded && _joins.Contains(newJoin)) return;

            _joins.Add(newJoin);
        }


        public void AddJoin(JoinType join,
                            string toTableName,
                            string toColumnName,
                            Comparison @operator,
                            string fromTableName,
                            string fromColumnName)
        {
            AddJoin(join, toTableName, toColumnName, @operator, fromTableName, fromColumnName, false);
        }


        public void AddJoin(JoinType join,
                            string toTableName,
                            string toColumnName,
                            Comparison @operator,
                            string fromTableName,
                            string fromColumnName,
                            bool ignoreIfAlreadyAdded)
        {
            var newJoin = new JoinClause(join, toTableName, toColumnName, @operator, fromTableName, fromColumnName);
            AddJoin(newJoin, ignoreIfAlreadyAdded);
        }


        public void AddOrderBy(OrderByClause clause)
        {
            _orderByStatement.Add(clause);
        }


        public void AddOrderBy(Enum field, Sorting order)
        {
            AddOrderBy(field.ToString(), order);
        }


        public void AddOrderBy(string field, Sorting order)
        {
            var newOrderByClause = new OrderByClause(field, order);
            _orderByStatement.Add(newOrderByClause);
        }


        public void AddWhere(WhereClause clause)
        {
            AddWhere(clause, 1);
        }


        public void AddWhere(WhereClause clause, int level)
        {
            _whereStatement.Add(clause, level);
        }


        public WhereClause AddWhere(string field, Comparison @operator, object compareValue)
        {
            return AddWhere(field, @operator, compareValue, 1);
        }


        public WhereClause AddWhere(Enum field, Comparison @operator, object compareValue)
        {
            return AddWhere(field.ToString(), @operator, compareValue, 1);
        }


        public WhereClause AddWhere(string field, Comparison @operator, object compareValue, int level)
        {
            var newWhereClause = new WhereClause(field, @operator, compareValue);
            _whereStatement.Add(newWhereClause, level);
            return newWhereClause;
        }


        private string BuildColumnNamesSql()
        {
            string columnNamesSql = String.Empty;
            if (_selectedColumns.Count == 0)
            {
                if (_selectedTables.Count == 1)
                    columnNamesSql += _selectedTables[0] + ".";
                // By default only select * from the table that was selected. If there are any joins, it is the responsibility of the user to select the needed columns.

                columnNamesSql += "*";
            }
            else
            {
                foreach (string columnName in _selectedColumns)
                {
                    columnNamesSql += columnName + ',';
                }
                columnNamesSql = columnNamesSql.TrimEnd(','); // Trim de last comma inserted by foreach loop
                columnNamesSql += ' ';
            }
            return columnNamesSql;
        }


        private string BuildGroupBySql()
        {
            if (_groupByColumns.Count == 0) return String.Empty;

            string result = string.Format(" GROUP BY {0} ", _groupByColumns.Join(","));
            return result;
        }


        private string BuildHavingSql()
        {
            if (_havingStatement.ClauseLevels == 0) return String.Empty;

            return string.Format(" HAVING {0}", _havingStatement.BuildSql());
        }


        private string BuildJoinSql()
        {
            if (_joins.Count <= 0) return string.Empty;

            string result = string.Format(" {0}", _joins.Join(" "));
            return result;
        }


        private string BuildOrderBySql()
        {
            if (_orderByStatement.Count == 0) return String.Empty;

            string result = string.Format(" ORDER BY {0} ", _orderByStatement.Join(","));
            return result;
        }


        private string BuildSelectPreambleSql()
        {
            string sql = "SELECT ";

            if (_distinct)
            {
                sql += "DISTINCT ";
            }
            return sql;
        }


        private string BuildTableNamesSql()
        {
            if (_selectedTables.Count == 0) return String.Empty;

            string tableNames = string.Format(" FROM {0}", _selectedTables.Join(","));
            return tableNames;
        }


        private string BuildWhereSql()
        {
            if (_whereStatement.ClauseLevels == 0) return String.Empty;

            return " WHERE " + _whereStatement.BuildSql();
        }


        public SelectQueryBuilder Clone()
        {
            return DeepCloner.Clone(this);
        }


        public void GroupBy(params string[] columns)
        {
            foreach (string column in columns)
            {
                _groupByColumns.Add(column);
            }
        }


        public void RemoveWhere(WhereClause clause)
        {
            _whereStatement.Remove(clause);
        }


        public void RemoveWhereForField(string fieldName)
        {
            _whereStatement.RemoveForField(fieldName);
        }


        public void SelectAllColumns()
        {
            _selectedColumns.Clear();
        }


        public void SelectColumn(string column)
        {
            _selectedColumns.Clear();
            _selectedColumns.Add(column);
        }


        public void SelectColumns(params string[] columns)
        {
            _selectedColumns.Clear();
            foreach (string column in columns)
            {
                _selectedColumns.Add(column);
            }
        }


        public void SelectCount()
        {
            SelectColumn("count(1)");
        }


        public void SelectFromTable(string table)
        {
            _selectedTables.Clear();
            _selectedTables.Add(table);
        }


        public void SelectFromTables(params string[] tables)
        {
            _selectedTables.Clear();
            foreach (string table in tables)
            {
                _selectedTables.Add(table);
            }
        }
    }
}