using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;

//
// Class: WhereClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Eca.Commons.Data.SelectQueryBuilder
{
    /// <summary>
    /// Represents a WHERE clause on 1 database column, containing 1 or more comparisons on 
    /// that column, chained together by logic operators: eg (UserID=1 or UserID=2 or UserID>100)
    /// This can be achieved by doing this:
    /// WhereClause myWhereClause = new WhereClause("UserID", Comparison.Equals, 1).Or(2).Or(3);
    /// </summary>
    [Serializable]
    public struct WhereClause : IEquatable<WhereClause>
    {
        private readonly string _rawSql;
        private string _fieldName;
        private Comparison _comparisonOperator;
        private object _value;



        [Serializable]
        internal struct SubClause : IEquatable<SubClause>
        {
            public LogicOperator LogicOperator;
            public Comparison ComparisonOperator;
            public object Value;


            public SubClause(LogicOperator logic, Comparison compareOperator, object compareValue)
            {
                LogicOperator = logic;
                ComparisonOperator = compareOperator;
                Value = compareValue;
            }


            public bool Equals(SubClause other)
            {
                return Equals(other.LogicOperator, LogicOperator) &&
                       Equals(other.ComparisonOperator, ComparisonOperator) && Equals(other.Value, Value);
            }


            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof (SubClause)) return false;
                return Equals((SubClause) obj);
            }


            public override int GetHashCode()
            {
                unchecked
                {
                    int result = LogicOperator.GetHashCode();
                    result = (result*397) ^ ComparisonOperator.GetHashCode();
                    result = (result*397) ^ (Value != null ? Value.GetHashCode() : 0);
                    return result;
                }
            }
        }



        internal List<SubClause> SubClauses; // Array of SubClause

        /// <summary>
        /// Gets/sets the name of the database column this WHERE clause should operate on
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        /// <summary>
        /// Gets/sets the comparison method
        /// </summary>
        public Comparison ComparisonOperator
        {
            get { return _comparisonOperator; }
            set { _comparisonOperator = value; }
        }

        /// <summary>
        /// Gets/sets the value that was set for comparison
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public WhereClause(string field, Comparison firstCompareOperator, object firstCompareValue)
        {
            _rawSql = null;
            _fieldName = field;
            _comparisonOperator = firstCompareOperator;
            _value = firstCompareValue;
            SubClauses = new List<SubClause>();
        }


        public WhereClause(string rawSql) : this()
        {
            _rawSql = rawSql;
            SubClauses = new List<SubClause>();
        }


        public WhereClause Or(object compareValue)
        {
            return AddClause(LogicOperator.Or, _comparisonOperator, compareValue);
        }


        public WhereClause And(object compareValue)
        {
            return AddClause(LogicOperator.And, _comparisonOperator, compareValue);
        }


        public WhereClause AddClause(LogicOperator logic, Comparison compareOperator, object compareValue)
        {
            var newSubClause = new SubClause(logic, compareOperator, compareValue);
            SubClauses.Add(newSubClause);
            return this;
        }


        public override string ToString()
        {
            return BuildSql();
        }


        public bool Equals(WhereClause other)
        {
            return Equals(other._rawSql, _rawSql) && Equals(other._fieldName, _fieldName) &&
                   Equals(other._comparisonOperator, _comparisonOperator) && Equals(other._value, _value) &&
                   SubClauses.SequenceEqual(other.SubClauses);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (WhereClause)) return false;
            return Equals((WhereClause) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_rawSql != null ? _rawSql.GetHashCode() : 0);
                result = (result*397) ^ (_fieldName != null ? _fieldName.GetHashCode() : 0);
                result = (result*397) ^ _comparisonOperator.GetHashCode();
                result = (result*397) ^ (_value != null ? _value.GetHashCode() : 0);
                result = (result*397) ^ SubClauses.GetSequenceHashCode();
                return result;
            }
        }


        public string BuildSql()
        {
            if (!String.IsNullOrEmpty(_rawSql)) return _rawSql;

            string whereClause = WhereStatement.CreateComparisonClause(FieldName, ComparisonOperator, Value);

            foreach (SubClause subWhereClause in SubClauses)
            {
                switch (subWhereClause.LogicOperator)
                {
                    case LogicOperator.And:
                        whereClause += " AND ";
                        break;
                    case LogicOperator.Or:
                        whereClause += " OR ";
                        break;
                }
                whereClause += WhereStatement.CreateComparisonClause(FieldName,
                                                                     subWhereClause.ComparisonOperator,
                                                                     subWhereClause.Value);
            }
            return whereClause;
        }


        public WhereClause Copy()
        {
            return DeepCloner.Clone(this);
        }


        #region Class Members

        public static WhereClause Eq(string fieldName, object value)
        {
            return new WhereClause(fieldName, Comparison.Equals, value);
        }


        public static WhereClause Like(string fieldName, object value)
        {
            return new WhereClause(fieldName, Comparison.Like, value);
        }


        public static WhereClause In<T>(string fieldName, IEnumerable<T> values)
        {
            return new WhereClause(fieldName, Comparison.In, values);
        }


        public static WhereClause NotEq(string fieldName, object value)
        {
            return new WhereClause(fieldName, Comparison.NotEquals, value);
        }

        #endregion
    }
}