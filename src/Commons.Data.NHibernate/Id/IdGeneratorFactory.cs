using System;
using System.Collections.Generic;
using System.Data;
using Eca.Commons.Extensions;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Type;
using NValidate.Framework;

namespace Eca.Commons.Data.NHibernate.Id
{
    /// <summary>
    /// Creates various <see cref="INHibernateIdGenerator{T}"/> that serve up unique identifiers
    /// </summary>
    public class IdGeneratorFactory
    {
        /// <summary>
        /// Create the desired <see cref="INHibernateIdGenerator{T}"/> as determined by the <see cref="IdGeneratorParams"/>
        /// </summary>
        /// <remarks>
        /// To create a table backed id generator, you need to supply as the <paramref name="parameters"/>,
        /// an instance of the <see cref="TableIdGeneratorParams"/> class
        /// </remarks>
        /// <typeparam name="IdType">The type of the id that is to be generate</typeparam>
        /// <param name="parameters">Values that select and configure the a desired <see cref="INHibernateIdGenerator{T}"/></param>
        public virtual INHibernateIdGenerator<IdType> Create<IdType>(IdGeneratorParams parameters)
        {
            IConfigurable generator = CreateGenerator<IdType>(parameters);
            return (INHibernateIdGenerator<IdType>) generator;
        }


        #region Class Members

        private static TableGenerator CastToTableGenerator<T>(INHibernateIdGenerator<T> generator)
        {
            TableGenerator result;
            var nhibernateAdaptor = generator as IdGeneratorNHibernateAdaptor<T>;
            if (nhibernateAdaptor != null)
            {
                result = nhibernateAdaptor.Implementation as TableGenerator;
            }
            else
            {
                result = generator as TableGenerator;
            }

            return result;
        }


        private static void Configure(TableIdGeneratorParams parameters, IConfigurable generator)
        {
            var parms = new Dictionary<string, string>
                            {
                                {TableGenerator.TableParamName, parameters.TableName},
                                {TableGenerator.ColumnParamName, parameters.ColumnName},
                                {TableHiLoGenerator.MaxLo, parameters.InMemoryIncrementSize.ToString()}
                            };
            generator.Configure(parameters.ColumnType, parms, parameters.Dialect);
        }


        private static IConfigurable CreateGenerator<IdType>(IdGeneratorParams parameters)
        {
            switch (parameters.Strategy)
            {
                case IdGeneratorStrategy.TableHiLow:
                    {
                        TableHiLoGenerator generator = CreateTableHiLoGenerator((TableIdGeneratorParams) parameters);
                        return new IdGeneratorNHibernateAdaptor<IdType>(generator);
                    }
                case IdGeneratorStrategy.MinimalGap:
                    {
                        return CreateMinimalGapTableIdGenerator((TableIdGeneratorParams) parameters);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static Int32MinimalGapTableIdGenerator CreateMinimalGapTableIdGenerator(
            TableIdGeneratorParams parameters)
        {
            var generator = new Int32MinimalGapTableIdGenerator();
            Configure(parameters, generator);
            return generator;
        }


        /// <summary>
        /// Create the table for the <paramref name="generator"/> supplied. 
        /// </summary>
        /// <remarks>
        /// This method is only relevant for generator's that are backed by a table
        /// </remarks>
        public static void CreateTableFor<T>(INHibernateIdGenerator<T> generator, IDbConnection openConnection)
        {
            CreateTableFor(generator, openConnection, new MsSql2005Dialect());
        }


        /// <summary>
        /// Create the table for the <paramref name="generator"/> supplied. 
        /// </summary>
        /// <remarks>
        /// This method is only relevant for generator's that are backed by a table
        /// </remarks>
        public static void CreateTableFor<T>(INHibernateIdGenerator<T> generator,
                                             IDbConnection openConnection,
                                             Dialect dialect)
        {
            Check.Require(() => {
                Demand.The.Param(() => generator).IsNotNull();
                Demand.The.Param(() => openConnection).IsNotNull().IsOpen();
            });

            TableGenerator gen = CastToTableGenerator(generator);
            if (gen == null)
            {
                throw new ArgumentException("Generator does not support a backing store", "generator");
            }

            var dropTableSql = gen.SqlDropString(dialect).Join("\n");
            SimpleDataAccess.ExecuteSql(dropTableSql, openConnection);

            var createIdTableSql = gen.SqlCreateStrings(dialect).Join("\n");
            SimpleDataAccess.ExecuteSql(createIdTableSql, openConnection);
        }


        private static TableHiLoGenerator CreateTableHiLoGenerator(TableIdGeneratorParams parameters)
        {
            var generator = new TableHiLoGenerator();
            Configure(parameters, generator);
            return generator;
        }

        #endregion


        [SkipFormatting]
        private class IdGeneratorNHibernateAdaptor<T> : INHibernateIdGenerator<T>, IConfigurable
        {
            private readonly IIdentifierGenerator _nhibernateIdGenerator;


            public IdGeneratorNHibernateAdaptor(IIdentifierGenerator nhibernateIdGenerator)
            {
                _nhibernateIdGenerator = nhibernateIdGenerator;
            }


            public IIdentifierGenerator Implementation
            {
                get { return _nhibernateIdGenerator; }
            }


            public void Configure(IType type, IDictionary<string, string> parms, Dialect dialect)
            {
                ((IConfigurable) Implementation).Configure(type, parms, dialect);
            }


            public T NextId(ISession currentSession)
            {
                object nextIdObj = Implementation.Generate((ISessionImplementor) currentSession, null);
                object nextId = Convert.ChangeType(nextIdObj, typeof (T));
                return (T) nextId;
            }


            public T PeekNextId(ISession currentSession)
            {
                //it is conceivable that support could be added for the TableHiLowGenerator
                throw new InvalidOperationException(string.Format("PeekNextId is not currently supported by {0}",
                                                                  Implementation.GetType().FullName));
            }


            /// <summary>
            /// No op operation
            /// </summary>
            /// <param name="currentSession"></param>
            public void SaveChangesToId(ISession currentSession) {}


            public void Reseed(T nextId, ISession currentSession)
            {
                //it is conceivable that support could be added for the TableHiLowGenerator
                throw new InvalidOperationException(string.Format("Reseed is not currently supported by {0}",
                                                                  Implementation.GetType().FullName));
            }
        }
    }
}