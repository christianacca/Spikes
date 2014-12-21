using System;
using System.Linq.Expressions;
using Eca.Commons.Reflection;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Id;
using NHibernate.Type;
using NValidate.Framework;

namespace Eca.Commons.Data.NHibernate.Id
{
    public enum IdGeneratorStrategy
    {
        /// <summary>
        /// Specifies a <see cref="TableHiLoGenerator"/>
        /// </summary>
        /// <remarks>
        /// Use this when you want a table backed numeric id generated that is 
        /// optimised to generated most, if not all, id's in memory without 
        /// roundtripping to the  database. 
        /// <p>
        /// The downside is that the sequence of id's will have gaps unless 
        /// you choose an in memory increment ('max_lo') of 0
        /// </p>
        /// </remarks>
        TableHiLow,
        /// <summary>
        /// Specifies a <see cref="MinimalGapTableIdGenerator"/> (or one of its derivatives)
        /// </summary>
        /// <remarks>
        /// See the api document for <see cref="MinimalGapTableIdGenerator"/> to 
        /// learn how to ensure that gaps in the generated sequence of id's 
        /// can be minimised (or entirely eliminated)
        /// </remarks>
        MinimalGap
    }



    [SkipFormatting]
    public class IdGeneratorParams
    {
        public IdGeneratorParams()
        {
            _strategy = IdGeneratorStrategy.MinimalGap;
        }


        protected IdGeneratorStrategy _strategy;

        public virtual IdGeneratorStrategy Strategy
        {
            get { return _strategy; }
            set
            {
                var tableStrategies = new[]
                                          {
                                              IdGeneratorStrategy.MinimalGap,
                                              IdGeneratorStrategy.TableHiLow
                                          };
                string errorMessage =
                    string.Format("The strategy {0} should be configured using a TableIdGeneratorParams", value);
                Check.Require(() => Demand.The.Param(() => value).IsNotOneOf(tableStrategies, errorMessage));
                _strategy = value;
            }
        }
    }



    public class TableIdGeneratorParams : IdGeneratorParams
    {
        #region Constructors

        public TableIdGeneratorParams()
        {
            ColumnType = NHibernateUtil.Int32;
            Dialect = new MsSql2005Dialect();
        }

        #endregion


        #region Properties

        /// <summary>
        /// The column to store the next id. If not supplied, it will default to 'next_hi'
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The type of the column used to store the id
        /// </summary>
        public IType ColumnType { get; set; }

        /// <summary>
        /// The sql dialect that should be used to create the sql necessary to fetch and generate id's.
        /// If not supplied will default to <see cref="MsSql2005Dialect"/>
        /// </summary>
        public Dialect Dialect { get; set; }

        /// <summary>
        /// The number of increments of the id to perform in memory, before roundtripping to the database
        /// to generate an id
        /// </summary>
        public int InMemoryIncrementSize { get; set; }

        public override IdGeneratorStrategy Strategy
        {
            get { return base.Strategy; }
            set { _strategy = value; }
        }

        /// <summary>
        /// The table to store the next id. If not supplied, it will default to 'hibernate_unique_key'
        /// </summary>
        public string TableName { get; set; }

        #endregion


        #region Class Members

        protected static void AddTableAndColumnName<T>(Expression<Func<T, object>> propertyAccessor,
                                                       string columnNameSuffix,
                                                       TableIdGeneratorParams result)
        {
            string columnName = "Next_" + ReflectionUtil.GetAccessor(propertyAccessor).Name;
            if (!String.IsNullOrEmpty(columnNameSuffix)) columnName += columnNameSuffix;
            string tableName = "tbl" + typeof (T).Name + "_NextNumber";
            result.ColumnName = columnName;
            result.TableName = tableName;
        }


        /// <summary>
        /// Derive the parameters for a table backed id generator from the <paramref name="propertyAccessor"/>
        /// supplied
        /// </summary>
        /// <remarks>
        /// <example>
        /// <code>
        /// var paramseter = TableIdGeneratorParams.For&lt;Customer>(x => x.Reference, IdGeneratorStrategy.MinimalGap);
        /// </code>
        /// would create the parameters for a table backed <see cref="IIdGenerator{T}"/> whose id's would be stored in a table
        /// column 'tblCustomer_NextNumber.Next_Reference'
        /// </example>
        /// </remarks>
        /// <typeparam name="T">The entity type that is the subject of the id's that are to be generated</typeparam>
        /// <param name="propertyAccessor">An expression that identifies the property on the subject which is to have id's generated</param>
        /// <param name="columnNameSuffix">An optional suffix for the final column name</param>
        /// <param name="strategy">The id generating strategy</param>
        /// <returns></returns>
        public static TableIdGeneratorParams For<T>(Expression<Func<T, object>> propertyAccessor,
                                                    string columnNameSuffix,
                                                    IdGeneratorStrategy strategy)
        {
            var tableStrategies = new[]
                                      {
                                          IdGeneratorStrategy.MinimalGap,
                                          IdGeneratorStrategy.TableHiLow
                                      };
            Check.Require(() => Demand.The.Param(() => strategy).IsOneOf(tableStrategies));
            var result = new TableIdGeneratorParams();
            AddTableAndColumnName(propertyAccessor, columnNameSuffix, result);
            return result;
        }


        /// <summary>
        /// Derive the parameters for a table backed id generator from the <paramref name="propertyAccessor"/>
        /// supplied
        /// </summary>
        /// <remarks>
        /// <example>
        /// <code>
        /// var paramseter = TableIdGeneratorParams.For&lt;Customer>(x => x.Reference, IdGeneratorStrategy.MinimalGap);
        /// </code>
        /// would create the parameters for a table backed <see cref="IIdGenerator{T}"/> whose id's would be stored in a table
        /// column 'tblCustomer_NextNumber.Next_Reference'
        /// </example>
        /// </remarks>
        /// <typeparam name="T">The entity type that is the subject of the id's that are to be generated</typeparam>
        /// <param name="propertyAccessor">An expression that identifies the property on the subject which is to have id's generated</param>
        /// <param name="strategy">The id generating strategy</param>
        /// <returns></returns>
        public static TableIdGeneratorParams For<T>(Expression<Func<T, object>> propertyAccessor,
                                                    IdGeneratorStrategy strategy)
        {
            return For(propertyAccessor, null, strategy);
        }

        #endregion
    }
}