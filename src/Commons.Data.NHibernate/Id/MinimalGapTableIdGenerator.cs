using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;
using log4net;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace Eca.Commons.Data.NHibernate.Id
{
    /// <summary>
    /// <p>
    /// An <see cref="IIdentifierGenerator" /> that uses a database table to store the last
    /// generated value.
    /// </p>
    /// <p>
    /// Similar to the <see cref="TableHiLoGenerator"/> it allows for blocks of ids to be generated 
    /// in memory without round tripping to the database for each id. However, the implementation
    /// is designed to minimise gaps in the sequence of ids
    /// </p>
    /// </summary>
    /// <remarks>
    /// <p>
    /// To guarrantee no gaps ensure the following:
    /// <list type="bullet">
    /// <item>
    /// <see cref="Generate"/> is called inside a transaction along with the entities 
    /// for whom the id's are being generated; AND <b>either</b> of the following:
    /// </item>
    /// <item>choose an in memory increment size of 0</item>
    /// <item>call <see cref="SaveChangesToId"/> inside the same transaction</item>
    /// </list>
    /// </p>
    /// <p>
    /// The return type is <c>System.Int64</c>
    /// </p>
    /// <p>
    /// The parameters <c>table</c> and <c>column</c> are required, <c>max_lo</c> is optional
    /// </p>
    /// </remarks>
    public class MinimalGapTableIdGenerator : TableGenerator
    {
        #region Member Variables

        private bool _idGenerationHasStarted;
        private long _nextIdStoredInDatabase;

        #endregion


        #region Properties

        public string ColumnName { get; private set; }
        protected internal long CurrentId { get; internal set; }
        private string FetchSql { get; set; }
        private long InMemoryIncrements { get; set; }
        private long InMemoryIncrementsAllowed { get; set; }


        private SqlType[] ParameterTypes { get; set; }
        public string TableName { get; private set; }
        private SqlString UpdateSql { get; set; }

        #endregion


        #region IIdentifierGenerator interface members

        /// <summary>
        /// Configures the TableGenerator by reading the value of <c>table</c>, 
        /// <c>column</c>, and <c>schema</c> from the <c>parms</c> parameter.
        /// </summary>
        /// <param name="type">The <see cref="IType"/> the identifier should be.</param>
        /// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
        /// <param name="dialect">The <see cref="Dialect"/> to help with Configuration.</param>
        public override void Configure(IType type, IDictionary<string, string> parms, Dialect dialect)
        {
            base.Configure(type, parms, dialect);
            TableName = this.GetField<string>("tableName");
            ColumnName = this.GetField<string>("columnName");
            FetchSql = this.GetField<string>("query");
            UpdateSql = this.GetField<SqlString>("updateSql");
            ParameterTypes = this.GetField<SqlType[]>("parameterTypes");

            InMemoryIncrementsAllowed = PropertiesHelper.GetInt64(TableHiLoGenerator.MaxLo, parms, Int16.MaxValue);
            InMemoryIncrements = InMemoryIncrementsAllowed; // so we goto the database on the first invocation
        }


        /// <summary>
        /// Generate a <see cref="short"/>, <see cref="int"/>, or <see cref="long"/> 
        /// for the identifier by a combination of incrementing a counter in memory and
        /// selecting and updating a value in a table.
        /// </summary>
        /// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
        /// <param name="obj">The entity for which the id is being generated.</param>
        /// <returns>The new identifier as a <see cref="short"/>, <see cref="int"/>, or <see cref="long"/>.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override object Generate(ISessionImplementor session, object obj)
        {
            if (InMemoryIncrements == InMemoryIncrementsAllowed)
            {
                object id = GetNextIdBlockFromDatabase(session);
                CurrentId = Math.Max(Convert.ToInt64(id), ++CurrentId);
                InMemoryIncrements = 0;
            }
            else
            {
                CurrentId++;
                InMemoryIncrements++;
            }

            _idGenerationHasStarted = true;
            return CurrentId;
        }

        #endregion


        private IDbCommand CreateReadNextIdSqlCommand(ISessionImplementor session)
        {
            IDbCommand command = session.Connection.CreateCommand();
            EnlistInTransactionInPorgress(session, command);
            command.CommandText = FetchSql;
            command.CommandType = CommandType.Text;
            return command;
        }


        private void EnlistInTransactionInPorgress(ISessionImplementor session, IDbCommand ups)
        {
            if (session.TransactionInProgress)
            {
                ((ISession) session).Transaction.Enlist(ups);
            }
        }


        private object GetNextIdBlockFromDatabase(ISessionImplementor session)
        {
            long id;
            int rowsEffected;
            do
            {
                //the loop ensure atomicitiy of the 
                //select + uspdate even for no transaction
                //or read committed isolation level (needed for .net?)

                id = ReadNextIdInDatabase(session);
                _nextIdStoredInDatabase = id + (InMemoryIncrementsAllowed == 0 ? 1 : InMemoryIncrementsAllowed + 1);
                rowsEffected = UpdateNextIdInDatabase(session, id, _nextIdStoredInDatabase);
            } while (rowsEffected == 0);

            return id;
        }


        protected long GetNextIdStoredInDatabase(ISessionImplementor session)
        {
            return _idGenerationHasStarted ? _nextIdStoredInDatabase : ReadNextIdInDatabase(session);
        }


        /// <summary>
        /// See <see cref="IIdGenerator{T}.PeekNextId"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long PeekNextId(ISessionImplementor session)
        {
            if (!_idGenerationHasStarted) return Convert.ToInt64(ReadNextIdInDatabase(session));

            return Convert.ToInt64(CurrentId + 1);
        }


        private long ReadNextIdInDatabase(ISessionImplementor session)
        {
            using (IDbCommand qps = CreateReadNextIdSqlCommand(session))
            {
                log.Debug(string.Format("Reading high value:{0}", qps.CommandText));
                using (IDataReader rs = qps.ExecuteReader())
                {
                    try
                    {
                        if (!rs.Read())
                        {
                            string err = "could not read a hi value - you need to populate the table: " + TableName;
                            log.Error(err);
                            throw new IdentifierGenerationException(err);
                        }
                        return Convert.ToInt64(columnType.Get(rs, 0));
                    }
                    catch (Exception e)
                    {
                        log.Error("could not read a hi value", e);
                        throw;
                    }
                }
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reseed(long nextId, ISessionImplementor session)
        {
            long idExpectedInDatabase = GetNextIdStoredInDatabase(session); //concurrency control check
            int rowsAffected = UpdateNextIdInDatabase(session, idExpectedInDatabase, nextId);
            if (rowsAffected == 0) throw new StaleObjectStateException(this.ClassName(), Guid.Empty);
        }


        /// <summary>
        /// See <see cref="IIdGenerator{T}.SaveChangesToId"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SaveChangesToId(ISessionImplementor session)
        {
            if (!_idGenerationHasStarted) return;

            Int64 idExpectedInDatabase = GetNextIdStoredInDatabase(session); //concurrency control check
            UpdateNextIdInDatabase(session, idExpectedInDatabase, CurrentId + 1);
        }


        /// <summary>
        /// Update the next id in the database
        /// </summary>
        /// <remarks>
        /// The update will only happen when the value of the id in the database is equal to 
        /// <paramref name="currentNextId"/>. In other words concurrency control is being performed.
        /// </remarks>
        /// <param name="session">The session containing the database connection</param>
        /// <param name="currentNextId">the value of id that is expected to be in the database</param>
        /// <param name="nextId">the next id to save to the database</param>
        /// <returns>The number of rows affected by this update</returns>
        private int UpdateNextIdInDatabase(ISessionImplementor session, long currentNextId, long nextId)
        {
            try
            {
                using (
                    IDbCommand ups = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text,
                                                                                               UpdateSql,
                                                                                               ParameterTypes))
                {
                    ups.Connection = session.Connection;
                    EnlistInTransactionInPorgress(session, ups);
                    columnType.Set(ups, nextId, 0);
                    columnType.Set(ups, currentNextId, 1);

                    log.Debug(string.Format("Updating high value:{0}", ups.CommandText));
                    return ups.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("could not update hi value in: " + TableName, e);
                throw;
            }
        }


        #region Class Members

        private static readonly ILog log = LogManager.GetLogger(typeof (MinimalGapTableIdGenerator));

        #endregion
    }
}