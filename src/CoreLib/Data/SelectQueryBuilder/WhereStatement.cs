using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;

//
// Class: WhereStatement
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Eca.Commons.Data.SelectQueryBuilder
{
    [Serializable]
    public class WhereStatement : List<List<WhereClause>>
    {
        // The list in this container will contain lists of clauses, and 
        // forms a where statement alltogether!


        #region Properties

        public int ClauseLevels
        {
            get { return Count; }
        }

        #endregion


        public void Add(WhereClause clause)
        {
            Add(clause, 1);
        }


        public void Add(WhereClause clause, int level)
        {
            AddWhereClauseToLevel(clause, level);
        }


        public WhereClause Add(string field, Comparison @operator, object compareValue)
        {
            return Add(field, @operator, compareValue, 1);
        }


        public WhereClause Add(Enum field, Comparison @operator, object compareValue)
        {
            return Add(field.ToString(), @operator, compareValue, 1);
        }


        public WhereClause Add(string field, Comparison @operator, object compareValue, int level)
        {
            var newWhereClause = new WhereClause(field, @operator, compareValue);
            AddWhereClauseToLevel(newWhereClause, level);
            return newWhereClause;
        }


        private void AddWhereClauseToLevel(WhereClause clause, int level)
        {
            // Add the new clause to the array at the right level
            AssertLevelExistance(level);
            this[level - 1].Add(clause);
        }


        private IEnumerable<WhereClause> AllPredicates()
        {
            return this.SelectMany(clauses => clauses);
        }


        private void AssertLevelExistance(int level)
        {
            if (Count < (level - 1))
            {
                throw new Exception("Level " + level + " not allowed because level " + (level - 1) + " does not exist.");
            }
                // Check if new level must be created
            else if (Count < level)
            {
                Add(new List<WhereClause>());
            }
        }


        public string BuildSql()
        {
            string result = "";
            foreach (List<WhereClause> whereStatement in this) // Loop through all statement levels, OR them together
            {
                string levelWhere = "";
                foreach (WhereClause clause in whereStatement) // Loop through all conditions, AND them together
                {
                    levelWhere += string.Format("({0}) AND ", clause);
                }
                levelWhere = levelWhere.Substring(0, levelWhere.Length - 5);
                // Trim de last AND inserted by foreach loop
                if (whereStatement.Count > 1)
                {
                    result += " (" + levelWhere + ") ";
                }
                else
                {
                    result += " " + levelWhere + " ";
                }
                result += " OR";
            }
            result = result.Substring(0, result.Length - 2); // Trim de last OR inserted by foreach loop

            return result;
        }


        public bool ContainsPredicate(WhereClause predicate)
        {
            return AllPredicates().Contains(predicate);
        }


        public bool ContainsPredicateFor(string fieldName)
        {
            return AllPredicates().Any(predicate => predicate.FieldName == fieldName);
        }


        public WhereStatement Copy()
        {
            return DeepCloner.Clone(this);
        }


        public void Remove(WhereClause clause)
        {
            Remove(clause, 1);
        }


        public void Remove(WhereClause clause, int level)
        {
            RemoveWhereClauseFromLevel(clause, level);
        }


        public void RemoveForField(string fieldName)
        {
            RemoveForField(fieldName, 1);
        }


        public void RemoveForField(string fieldName, int level)
        {
            RemoveWhereClauseFromLevelForField(fieldName, 1);
        }


        private void RemoveWhereClauseFromLevel(WhereClause clause, int level)
        {
            AssertLevelExistance(level);
            this[level - 1].Remove(clause);
        }


        private void RemoveWhereClauseFromLevelForField(string fieldName, int level)
        {
            AssertLevelExistance(level);
            this[level - 1].RemoveAll(clause => clause.FieldName == fieldName);
        }


        #region Class Members

        /// <summary>
        /// This static method combines 2 where statements with eachother to form a new statement
        /// </summary>
        /// <param name="statement1"></param>
        /// <param name="statement2"></param>
        /// <returns></returns>
        public static WhereStatement CombineStatements(WhereStatement statement1, WhereStatement statement2)
        {
            // statement1: {Level1}((Age<15 OR Age>=20) AND (strEmail LIKE 'e%') OR {Level2}(Age BETWEEN 15 AND 20))
            // Statement2: {Level1}((Name = 'Peter'))
            // Return statement: {Level1}((Age<15 or Age>=20) AND (strEmail like 'e%') AND (Name = 'Peter'))

            // Make a copy of statement1
            WhereStatement result = statement1.Copy();

            // Add all clauses of statement2 to result
            for (int i = 0; i < statement2.ClauseLevels; i++) // for each clause level in statement2
            {
                List<WhereClause> level = statement2[i];
                foreach (WhereClause clause in level) // for each clause in level i
                {
                    for (int j = 0; j < result.ClauseLevels; j++) // for each level in result, add the clause
                    {
                        result.AddWhereClauseToLevel(clause, j);
                    }
                }
            }

            return result;
        }


        internal static string CreateComparisonClause(string fieldName, Comparison comparisonOperator, object value)
        {
            string output = "";
            if (value != null && value != DBNull.Value)
            {
                switch (comparisonOperator)
                {
                    case Comparison.Equals:
                        output = fieldName + " = " + FormatSqlValue(value);
                        break;
                    case Comparison.NotEquals:
                        output = fieldName + " <> " + FormatSqlValue(value);
                        break;
                    case Comparison.GreaterThan:
                        output = fieldName + " > " + FormatSqlValue(value);
                        break;
                    case Comparison.GreaterOrEquals:
                        output = fieldName + " >= " + FormatSqlValue(value);
                        break;
                    case Comparison.LessThan:
                        output = fieldName + " < " + FormatSqlValue(value);
                        break;
                    case Comparison.LessOrEquals:
                        output = fieldName + " <= " + FormatSqlValue(value);
                        break;
                    case Comparison.Like:
                        output = fieldName + " LIKE " + FormatSqlValue(value);
                        break;
                    case Comparison.NotLike:
                        output = "NOT " + fieldName + " LIKE " + FormatSqlValue(value);
                        break;
                    case Comparison.In:
                        output = fieldName + " IN (" + FormatSqlValue(value) + ")";
                        break;
                }
            }
            else // value==null	|| value==DBNull.Value
            {
                if ((comparisonOperator != Comparison.Equals) && (comparisonOperator != Comparison.NotEquals))
                {
                    throw new Exception("Cannot use comparison operator " + comparisonOperator +
                                        " for NULL values.");
                }
                else
                {
                    switch (comparisonOperator)
                    {
                        case Comparison.Equals:
                            output = fieldName + " IS NULL";
                            break;
                        case Comparison.NotEquals:
                            output = "NOT " + fieldName + " IS NULL";
                            break;
                    }
                }
            }
            return output;
        }


        /// <summary>
        /// Formats <paramref name="someValue"/> into a string suitable for use as an argument to a stored proc or sql where clause
        /// </summary>
        /// <remarks>
        /// Where the <paramref name="someValue"/> supplied is a string, the value will be delimited as a string literal 
        /// and protected against sql injection
        /// </remarks>
        public static string FormatSqlValue(object someValue)
        {
            if (someValue == null)
            {
                return "NULL";
            }

            if (someValue.GetType().IsCollectionType())
            {
                var values = (IEnumerable) someValue;
                string result = values.Cast<object>().Join(",", FormatSqlValue);
                return result;
            }

            string formattedValue;
            if (someValue.GetType().IsTypeOf(typeof (Enum)))
            {
                return "'" + someValue + "'";
            }
            switch (someValue.GetType().Name)
            {
                case "String":
                    formattedValue = "'" + ((string) someValue).Replace("'", "''") + "'";
                    break;
                case "Guid":
                    formattedValue = "'" + someValue + "'";
                    break;
                case "DateTime":
                    formattedValue = "'" + ((DateTime) someValue).ToString("dd MMM yyy HH:mm:ss.fff") + "'";
                    break;
                case "DBNull":
                    formattedValue = "NULL";
                    break;
                case "Boolean":
                    formattedValue = (bool) someValue ? "1" : "0";
                    break;
                case "SqlLiteral":
                    formattedValue = ((SqlLiteral) someValue).Value;
                    break;
                default:
                    formattedValue = someValue.ToString();
                    break;
            }
            return formattedValue;
        }

        #endregion
    }
}