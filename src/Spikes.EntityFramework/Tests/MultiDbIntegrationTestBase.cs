using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Eca.Commons.Data;
using MiscUtil.Collections.Extensions;
using NUnit.Framework;

namespace Spikes.EntityFramework.Tests
{
    /// <summary>
    /// Base class that will enclose database interaction within a transaction that will be rolled back
    /// at the end of every test
    /// </summary>
    public abstract class MultiDbIntegrationTestBase
    {
        private Dictionary<Type, Tuple<DbConnection, DbTransaction>> _connAndTxs;

        protected MultiDbIntegrationTestBase()
        {
            IsRollbackEnabled = true;
        }

        /// <summary>
        /// Whether data changes made by each test should be rolled back at the end of that test (defaults to true)
        /// </summary>
        public bool IsRollbackEnabled { get; set; }

        [SetUp]
        public void SetupDbTransaction()
        {
            _connAndTxs = new Dictionary<Type, Tuple<DbConnection, DbTransaction>>();
        }

        [TearDown]
        public void RollbackTransation()
        {
            if (IsRollbackEnabled)
            {
                _connAndTxs.Values.ToList().ForEach(x => x.Item2.Rollback());
            }
            else
            {
                _connAndTxs.Values.ToList().ForEach(x => x.Item2.Commit());
            }
            _connAndTxs.Values.ToList().ForEach(x =>
            {
                x.Item2.Dispose();
                x.Item1.Dispose();
            });
        }

        public void RebuildDatabases(params DbContext[] dbs)
        {
            foreach (var db in dbs)
            {
                SqlAdminQueries.DropDatabase(db.Database.Connection.DataSource, db.Database.Connection.Database);
                db.Database.Initialize(true);
                // DropDatabase kills existing connections so make sure to remove the dead ones we're holding onto
                if (_connAndTxs.ContainsKey(db.GetType()))
                {
                    _connAndTxs.Remove(db.GetType());
                }
            }
        }

        public void CommitChanges()
        {
            _connAndTxs.Values.ToList().ForEach(x => x.Item2.Commit());
            // begin a new transaction for existing connections
            _connAndTxs = _connAndTxs.ToDictionary(x => x.Key,
                x => new Tuple<DbConnection, DbTransaction>(x.Value.Item1, x.Value.Item1.BeginTransaction()));
        }

        /// <summary>
        /// Implementors of subclass to create the connection that will be supplied
        /// to <see cref="CreateDbContextFromConnection{T}"/>
        /// </summary>
        /// <example>
        /// <code>
        /// protected override DbConnection CreateConnection&lt;YourDbContext>()
        /// {
        ///     return new YourDbContext().Database.Connection;
        /// }
        /// </code>
        /// </example>
        protected abstract DbConnection CreateConnection<T>() where T: DbContext;

        /// <summary>
        /// Implementors of subclass to create a <see cref="DbContext"/> from an
        /// existing <see cref="DbConnection"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="CreateConnection{T}"/> will be used to supply the <see cref="DbConnection"/>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override YourDbContext CreateDbContextFromConnection&lt;YourDbContext>(DbConnection cnn)
        /// {
        ///     return new YourDbContext(cnn, contextOwnsConnection: false);
        /// }
        /// </code>
        /// </example>
        protected abstract T CreateDbContextFromConnection<T>(DbConnection cnn) where T : DbContext;

        /// <summary>
        /// Returns a <see cref="DbContext"/> that will use the existing transaction
        /// that will rolled back at the end of the each test
        /// </summary>
        /// <remarks>
        /// <para>
        /// Every <see cref="DbContext"/> created within each test case method
        /// will share the same connection and therefore transaction.
        /// </para>
        /// <para>
        /// The connection that's used is the one returned by <see cref="CreateConnection{T}"/>
        /// </para>
        /// </remarks>
        protected T CreateDbContext<T>() where T: DbContext
        {
            var entry = _connAndTxs.GetOrCreate(typeof (T), () =>
            {
                var c = CreateConnection<T>();
                c.Open();
                var t = c.BeginTransaction();
                return new Tuple<DbConnection, DbTransaction>(c, t);
            });
            var db = CreateDbContextFromConnection<T>(entry.Item1);
            db.Database.UseTransaction(entry.Item2);
            return db;
        }
    }
}