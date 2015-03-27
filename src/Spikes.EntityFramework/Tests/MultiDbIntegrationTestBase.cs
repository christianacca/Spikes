using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
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

        [SetUp]
        public void SetupDbTransaction()
        {
            _connAndTxs = new Dictionary<Type, Tuple<DbConnection, DbTransaction>>();
        }

        [TearDown]
        public void RollbackTransation()
        {
            _connAndTxs.Values.ToList().ForEach(x =>
            {
                x.Item2.Rollback();
                x.Item2.Dispose();
                x.Item1.Dispose();
            });
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