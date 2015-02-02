using System.Data.Common;
using System.Data.Entity;
using NUnit.Framework;

namespace Spikes.EntityFramework.Tests
{
    /// <summary>
    /// Base class that will enclose database interaction within a transaction that will be rolled back
    /// at the end of every test
    /// </summary>
    public abstract class DbIntegrationTestBase<T> where T : DbContext
    {
        private DbConnection _conn ;
        private DbTransaction _transaction;

        [SetUp]
        public void SetupDbTransaction()
        {
            _conn = CreateConnection();
            _conn.Open();
            _transaction = _conn.BeginTransaction();
        }

        [TearDown]
        public void RollbackTransation()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _conn.Dispose();
        }

        /// <summary>
        /// Implementors of subclass to create the connection that will be supplied
        /// to <see cref="CreateDbContextFromConnection"/>
        /// </summary>
        /// <example>
        /// <code>
        /// protected override DbConnection CreateConnection()
        /// {
        ///     return new YourDbContext().Database.Connection;
        /// }
        /// </code>
        /// </example>
        protected abstract DbConnection CreateConnection();

        /// <summary>
        /// Implementors of subclass to create a <see cref="DbContext"/> from an
        /// existing <see cref="DbConnection"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="CreateConnection"/> will be used to supply the <see cref="DbConnection"/>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override YourDbContext CreateDbContextFromConnection(DbConnection cnn)
        /// {
        ///     return new YourDbContext(cnn, contextOwnsConnection: false);
        /// }
        /// </code>
        /// </example>
        protected abstract T CreateDbContextFromConnection(DbConnection cnn);

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
        /// The connection that's used is the one returned by <see cref="CreateConnection"/>
        /// </para>
        /// </remarks>
        protected T CreateDbContext()
        {
            var db = CreateDbContextFromConnection(_conn);
            db.Database.UseTransaction(_transaction);
            return db;
        }
    }
}