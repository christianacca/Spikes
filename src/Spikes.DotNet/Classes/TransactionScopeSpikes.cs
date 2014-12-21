using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using NUnit.Framework;

namespace Spikes.DotNet
{
    [TestFixture]
    public class TransactionScopeSpikes
    {
        #region Test helpers

        private void AssertLocalAmbientTransactionStarted()
        {
            Assert.That(Transaction.Current, Is.Not.Null, "Ambient tx has already started");
            Assert.That(Transaction.Current.TransactionInformation.DistributedIdentifier,
                        Is.EqualTo(Guid.Empty),
                        "Transaction is local");
        }


        private void VerifyLocalTransactionPromotedToDistributedWhenConnectionOpened(IDbConnection connection)
        {
            AssertLocalAmbientTransactionStarted(); // clarifying assumptions

            try
            {
                connection.Open();
                Assert.That(Transaction.Current.TransactionInformation.DistributedIdentifier,
                            Is.Not.EqualTo(Guid.Empty),
                            "Transaction has become distributed");
            }
            catch (SqlException e)
            {
                Assert.That(e.Message,
                            Text.StartsWith("MSDTC on server ").And.EndsWith(" is unavailable."),
                            "Attempt was made to start distributed transaction");
            }
        }

        #endregion


        [Test]
        public void CreatingDefaultTransactionScopeWillCreateAmbientTransaction()
        {
            using (new TransactionScope()) AssertLocalAmbientTransactionStarted();
        }


        [Test]
        public void OpeningTwoConnections_EvenWithSameConnectionString_WillPromoteLocalTransactionToDistributed()
        {
            const string connectionString =
                @"Data Source=.\SQL2008R2;Initial Catalog=tempdb;Integrated Security=True";
            using (new TransactionScope())
            using (SqlConnection firstConnection = new SqlConnection(connectionString))
            using (SqlConnection secondConnection = new SqlConnection(connectionString))
            {
                firstConnection.Open();
                VerifyLocalTransactionPromotedToDistributedWhenConnectionOpened(secondConnection);
            }
        }
    }
}