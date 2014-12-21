using System;
using System.Collections.Generic;
using System.Reflection;
using Eca.Commons.Data.NHibernate;
using Eca.Commons.Data.NHibernate.ForTesting;
using Eca.Commons.Extensions;
using Eca.Commons.Testing;
using Eca.Commons.Testing.NHibernate;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace Eca.Spikes.NHibernate
{
    [TestFixture]
    public class NHiberanteSession : ThisProjectDatabaseTestsBase
    {
        #region Test helpers

        protected void CommitSessionExpectingException()
        {
            bool exceptionThrow = false;
            try
            {
                Nh.CurrentSession.BeginTransaction().Commit();
            }
            catch (Exception)
            {
                exceptionThrow = true;
            }
            finally
            {
                Nh.DisposeCurrentSession();
            }
            if (!exceptionThrow)
            {
                Assert.Fail("Committing session expected to throw");
            }
        }


        protected override EquivalenceComparer CreateComparer()
        {
            return EquivalenceComparer.Default;
        }


        protected void PersistUsingNewSession(params object[] entities)
        {
            using (ISession s = Nh.CreateSession())
            {
                foreach (object entity in entities)
                {
                    s.SaveOrUpdate(entity);
                }
                s.BeginTransaction().Commit();
            }
        }


        private static void ResetVersionPropertyTo(object entity, int versionNumber)
        {
            Type t = entity.GetType();
            FieldInfo f =
                t.GetField("_concurrencyId",
                           BindingFlags.Instance |
                           BindingFlags.Public |
                           BindingFlags.NonPublic);
            f.SetValue(entity, versionNumber);
        }

        #endregion


        #region NHibernate.Transaction behaviour

        [Test]
        public void SafeToCallBeginTransactionMultipleTimes()
        {
            Nh.CurrentSession.BeginTransaction();
            Nh.CurrentSession.BeginTransaction();
            Nh.CurrentSession.BeginTransaction();
        }


        [Test]
        public void TransactionPropertyAvailableEvenWhenNoTransactionStarted()
        {
            Assert.That(Nh.CurrentSession.Transaction, Is.Not.Null);
            Assert.That(Nh.CurrentSession.Transaction.IsActive, Is.False);
        }


        [Test]
        public void TransactionIsPartiallyAwareOfSessionClosing()
        {
            Nh.CurrentSession.BeginTransaction();

            Nh.CurrentSession.Close();

            Assert.That(Nh.CurrentSession.Transaction.IsActive,
                        Is.False,
                        "transaction knows its no longer active");
            Assert.That(Nh.CurrentSession.Transaction.WasRolledBack,
                        Is.False,
                        "transaction not marked as having rolled back");
            Assert.That(Nh.CurrentSession.Transaction.WasCommitted,
                        Is.False,
                        "transaction not marked as having been committed");
        }


        [Test]
        public void CanReachTransactionFromSessionAfterSessionIsClosed()
        {
            Nh.CurrentSession.BeginTransaction();

            Nh.CurrentSession.Close();

            Assert.That(Nh.CurrentSession.Transaction, Is.Not.Null, "Can still reach txn object");
        }


        [Test]
        public void CannotCallCommitOnceSessionClosed()
        {
            ITransaction t1 = Nh.CurrentSession.BeginTransaction();
            Nh.CurrentSession.Close();

            Assert.Throws<ObjectDisposedException>(
                t1.Commit);
        }


        [Test]
        public void CannotCallRollbackOnceSessionClosed()
        {
            ITransaction t1 = Nh.CurrentSession.BeginTransaction();
            Nh.CurrentSession.Close();

            Assert.Throws<ObjectDisposedException>(
                t1.Rollback);
        }


        [Test]
        public void TransactionNotMarkedAsCommittedIfCommitFails()
        {
            Customer c = Om.CreateInvalidCustomer();

            Nh.CurrentSession.SaveOrUpdate(c);
            ITransaction tx = Nh.CurrentSession.BeginTransaction();

            NUnitTestsBase.ExecuteAndExpect<Exception>(tx.Commit);

            Assert.That(Nh.CurrentSession.Transaction.WasCommitted, Is.False);
        }


        [Test]
        public void FlushInsideATransactionWillNotBeCommittedUnlessTransactionCommitted()
        {
            Customer c = Om.CreateCustomer();

            using (ITransaction transaction = Nh.CurrentSession.BeginTransaction())
            {
                Nh.CurrentSession.SaveOrUpdate(c);
                Nh.FlushSessionToDbAndClear();
                Assert.That(Nh.CurrentSession.Get<Customer>(c.Id),
                            Is.Not.Null,
                            "Customer saved in database");
                transaction.Rollback();
            }

            Nh.CurrentSession.Clear();
            Assert.That(Nh.CurrentSession.Get<Customer>(c.Id),
                        Is.Null,
                        "Customer not found in database");
        }


        [Test]
        public void DisposingOfTransactionWillRollbackTransactionIfNotCommitted()
        {
            Customer c = Om.CreateCustomer();

            using (Nh.CurrentSession.BeginTransaction())
            {
                Nh.CurrentSession.SaveOrUpdate(c);
                Nh.FlushSessionToDbAndClear();
            }

            Assert.That(Nh.CurrentSession.Get<Customer>(c.Id),
                        Is.Null,
                        "Customer not found in database");
        }

        #endregion


        #region NHibernate.Session connection behaviour

        [Test]
        public void WillNotAutomaticallyReconnectAfterExplicitCallToDisconnect()
        {
            //setup
            Customer customer = Om.CreateCustomer();
            Nh.CurrentSession.Save(customer);
            Nh.CurrentSession.Flush();

            Nh.CurrentSession.Clear();
            Nh.CurrentSession.Disconnect();

            //run test
            Exception ex = Assert.Throws<HibernateException>(() => Nh.CurrentSession.Get<Customer>(customer.Id));
            Assert.That(ex.Message, Is.EqualTo("Session is currently disconnected"));
        }


        [Test]
        public void ReconnectWhenAlreadyConnectedWillThrowException()
        {
            Exception ex = Assert.Throws<HibernateException>(
                Nh.CurrentSession.Reconnect);
            Assert.That(ex.Message, Is.EqualTo("session already connected"));
        }


        [Test]
        public void SafeToCallDisconnectAfterImplicitDisconnect()
        {
            Nh.CurrentSession.Save(Om.CreateCustomer());
            //automatically disconnects
            Nh.CurrentSession.BeginTransaction().Commit();

            Nh.CurrentSession.Disconnect();
        }


        [Test]
        public void DisconnectWhenAlreadyExplicitlyDisconnectedWillThrowException()
        {
            Nh.CurrentSession.Disconnect();

            Exception ex = Assert.Throws<HibernateException>(() => Nh.CurrentSession.Disconnect());
            Assert.That(ex.Message, Is.EqualTo("Session already disconnected"));
        }


        [Test]
        public void DisconnetWillFailOnceSessionClosed()
        {
            Nh.CurrentSession.Close();

            Assert.Throws<ObjectDisposedException>(() => Nh.CurrentSession.Disconnect());
        }


        [Test]
        public void SaveOrUpdateDoesNotRequireAnOpenConnection()
        {
            Nh.CurrentSession.Disconnect();

            Nh.CurrentSession.SaveOrUpdate(Om.CreateCustomer());
        }


        [Test]
        public void DeleteDoesNotRequireAnOpenConnection()
        {
            Nh.CurrentSession.Disconnect();

            Nh.CurrentSession.Delete(Om.CreateCustomer());
        }

        #endregion


        #region Save

        [Test]
        public void Save_WhenEntityReferencesTransientInstance_WillThrowImmediately()
        {
            Customer c = Om.CreateCustomer();
            Order o = Om.CreateOrder();

            //Order now has a reference to a transient (unsaved) Customer
            o.Customer = c;
            var ex = Assert.Throws<PropertyValueException>(() => Nh.CurrentSession.Save(o));
            Assert.That(ex.Message, Is.StringContaining("not-null property references a null or transient value"));
        }

        #endregion


        #region Lazy loading

        [Test]
        public void CanTestWhenAssociationIsLazyLoaded()
        {
            //given
            Customer customer = Om.CreateCustomerWithOneRep();
            DbFixture.Insert(customer);

            //when
            var loadedCustomer = Nh.GetFromDb<Customer>(customer.Id);

            //then
            NhAssert.IsLazyLoaded(loadedCustomer.CustomerRepresentatives);
        }

        #endregion


        #region NHibernate Detached objects

        [Test]
        public void CanDetatchObjectUsingEvict() {}


        [Test]
        public void CanDetactchObjectUsingClear() {}


        [Test]
        public void LazyLoadWillFailOnceObjectDetatched()
        {
            //given
            Guid id = DbFixture.Insert(Om.CreateCustomerWithOneRep());

            //when
            var detatchedInstance = Nh.CurrentSession.Get<Customer>(id);
            Nh.CurrentSession.Evict(detatchedInstance);

            //then
            Assert.Throws<LazyInitializationException>(() => detatchedInstance.CustomerRepresentatives.At(0));
        }

        #endregion


        #region Other


        #region Test helpers

        private EntityKey EntityKeyFor<T>(object id)
        {
            var factoryImpl =
                (ISessionFactoryImplementor) Nh.CurrentSession.SessionFactory;
            IEntityPersister persister = factoryImpl.GetEntityPersister(typeof (T).FullName);
            return new EntityKey(id, persister, EntityMode.Poco);
        }


        private void EvictObjectsInCurrentSessionFoundWithin(ISession session)
        {
            ISessionImplementor impl = session.GetSessionImplementation();
            IDictionary<EntityKey, object> entities = impl.PersistenceContext.EntitiesByKey;
            foreach (KeyValuePair<EntityKey, object> toEvict in entities)
            {
                object entity =
                    Nh.CurrentSession.GetSessionImplementation().GetEntityUsingInterceptor(toEvict.Key);
                Nh.CurrentSession.Evict(entity);
            }
        }

        #endregion


        [Test]
        public void SafeToCloseSessionFactoryTwice()
        {
            NHibernateTestContext.CurrentContext.SessionFactory.Close();
            NHibernateTestContext.CurrentContext.SessionFactory.Close();
        }


        [Test]
        public void SaveWillMakeObjectVisibleToGet()
        {
            //setup
            Customer customer = Om.CreateCustomer();
            Nh.CurrentSession.Save(customer);

            //run test...
            var customerFromSession = Nh.CurrentSession.Get<Customer>(customer.Id);
            Assert.That(customerFromSession, Is.Not.Null, "object expected to be visible to Get");
            Assert.That(customerFromSession, Is.SameAs(customer), "Get expected to return the object passed to Save");
        }


        [Test]
        public void DeleteCancelsSave()
        {
            //setup
            Customer customer = Om.CreateCustomer();
            Nh.CurrentSession.Save(customer);

            Nh.CurrentSession.Delete(customer);
            Assert.That(Nh.CurrentSession.Get<Customer>(customer.Id), Is.Null);
        }


        [Test]
        public void FlushWillPersistChangesMadeAfterCallingSaveAsASeperateUpdate()
        {
            Customer customer = Om.CreateCustomer();
            Nh.CurrentSession.Save(customer);

            customer.Name = "New name";
            Nh.FlushSessionToDbAndClear();

            Assert.That(customer.Name, Is.EqualTo(Nh.GetFromDb<Customer>(customer.Id).Name));
            Assert.That(customer.ConcurrencyId, Is.EqualTo(2));
        }


        [Test]
        public void CanGetObjectFromFirstLevelCacheByIdentifier()
        {
            Guid customerId = DbFixture.InsertCustomer();

            EntityKey key = EntityKeyFor<Customer>(customerId);

            Nh.CurrentSession.Get<Customer>(customerId);
            Assert.That(Nh.CurrentSession.GetSessionImplementation().GetEntityUsingInterceptor(key),
                        Is.Not.Null,
                        "entity found");

            Nh.CurrentSession.Clear();
            Assert.That(Nh.CurrentSession.GetSessionImplementation().GetEntityUsingInterceptor(key),
                        Is.Null,
                        "entity not found");
        }


        [Test]
        public void CanEvictObjectsContainedWithinAnotherSession()
        {
            Guid customerId = DbFixture.InsertCustomer();
            var customer = Nh.CurrentSession.Get<Customer>(customerId);

            using (ISession newSession = Nh.CreateSession())
            {
                newSession.Get<Customer>(customerId);

                //not testing yet just clarifying assumptions
                Assert.That(Nh.CurrentSession.Contains(customer), Is.True, "entity in session");

                EvictObjectsInCurrentSessionFoundWithin(newSession);
            }

            Assert.That(Nh.CurrentSession.Contains(customer),
                        Is.False,
                        "entity has been evicted from session");
        }

        #endregion


        #region NHibernate.Session incrementing of object version numbers, persistence by reachability, etc

        [Test]
        public void SaveWillSetVersionPropertyToOne()
        {
            Customer c = Om.CreateCustomer();
            Assert.That(c.ConcurrencyId, Is.Not.EqualTo(1), "not testing yet just clarifying assumptions");

            Nh.CurrentSession.Save(c);

            Assert.That(c.ConcurrencyId, Is.EqualTo(1), "version property incremented");
        }


        [Test]
        public void UpdateWillNotIncrementVersionProperty()
        {
            Guid id = DbFixture.InsertCustomer();
            var c = Nh.CurrentSession.Get<Customer>(id);

            int versionPropertyBeforeUpdate = c.ConcurrencyId;
            Nh.CurrentSession.Update(c);

            Assert.That(c.ConcurrencyId, Is.EqualTo(versionPropertyBeforeUpdate), "version property not incremented");
        }


        [Test]
        public void FlushWillIncrementVersionPropertyOnExistingObject()
        {
            Guid id = DbFixture.InsertCustomer();
            var c = Nh.CurrentSession.Get<Customer>(id);

            c.Name = "George, Katie";

            int versionPropertyBeforeFlush = c.ConcurrencyId;
            Nh.CurrentSession.Flush();

            Assert.That(c.ConcurrencyId,
                        Is.EqualTo(versionPropertyBeforeFlush + 1),
                        "version property incremented");
        }


        [Test]
        public void ExceptionFlushingAnExistingObjectWillStopIncrementOfVersion()
        {
            Guid id = DbFixture.InsertCustomer();
            var c = Nh.CurrentSession.Get<Customer>(id);

            c.ShortCode = "CCCCCCCCC"; //update will fail when saved to the db

            int versionPropertyBeforeFlush = c.ConcurrencyId;
            CommitSessionExpectingException();

            Assert.That(c.ConcurrencyId,
                        Is.EqualTo(versionPropertyBeforeFlush),
                        "ConcurrencyId has NOT been incremented");
        }


        [Test]
        public void ExceptionFlushingAnExistingObjectWillNotStopVsIncrementOfChildObject()
        {
            Guid id = DbFixture.InsertCustomerWithAddress();
            var c = Nh.CurrentSession.Get<Customer>(id);

            Address a = c.AddressAt(0);
            a.Line1 = "Another street";

            c.ShortCode = "CCCCCCCCC"; //update will fail when saved to the db

            int versionPropertyBeforeFlush = a.ConcurrencyId;
            CommitSessionExpectingException();

            Assert.That(a.ConcurrencyId,
                        Is.EqualTo(versionPropertyBeforeFlush + 1),
                        "version property incremented");
        }


        [Test]
        public void FlushWillReachNewObjectsAttachedToAPersistenceInstanceAfterSave()
        {
            Customer c = Om.CreateCustomer();

            Nh.CurrentSession.SaveOrUpdate(c);

            //note that customer already associated with session
            Address a = Om.CreateAddress();
            c.AddAddress(a);

            Nh.FlushSessionToDbAndClear();

            var loadedCustomer = Nh.CurrentSession.Get<Customer>(c.Id);

            Assert.That(loadedCustomer.Addresses.Count, Is.EqualTo(1));
        }


        [Test]
        public void MustFirstSaveReferencedObjectBeforeSavingRefereningObject()
        {
            //given
            Customer customer = Om.CreateCustomer();
            Order order = Om.CreateOrderWith(customer);

            //this would not happen if persistence cascaded from Order to Customer
            NhAssert.VerifyInsertThrowsBecausePropertyReferencesUnsavedInstance(order, x => x.Customer);
        }

        #endregion


        [Test]
        public void CanAttachAssociatedObjectToDifferentSessions()
        {
            //note: although this is possible would highly recommend against doing this

            //given
            Customer customer = Om.CreateCustomer();
            Order order1 = Om.CreateOrderWith(customer);
            Order order2 = Om.CreateOrderWith(customer);

            Nh.CommitInSameTempSession(session => {
                DbFixture.Insert(customer);
                DbFixture.Insert(order1);
                DbFixture.Insert(order2);
            });

            var order1FromDb = Nh.GetFromDb<Order>(order1.Id);
            var order2FromDb = Nh.GetFromDb<Order>(order2.Id);
            NHibernateUtil.Initialize(order1FromDb.Customer);
            Assert.That(order1FromDb.Customer, Is.SameAs(order2FromDb.Customer), "clarifying assumptions");

            //evict all our object we have just loaded
            Nh.CurrentSession.Clear();


            //run test...

            //attach first order to first session
            ISession firstSession = Nh.CreateSession();
            firstSession.Lock(order1FromDb, LockMode.None);
            Assert.That(firstSession.ExistsInSession<Order>(order1FromDb.Id), Is.True, "order not attached");
            Assert.That(firstSession.ExistsInSession<Customer>(order1FromDb.Customer.Id),
                        Is.False,
                        "customer not expected to attached");

            //attach customer to second session
            ISession secondSession = Nh.CreateSession();
            secondSession.Lock(order1FromDb.Customer, LockMode.None);
            Assert.That(secondSession.ExistsInSession<Customer>(order1FromDb.Customer.Id),
                        Is.True,
                        "customer not attached");

            //attach second order to third session
            ISession thirdSession = Nh.CreateSession();
            thirdSession.Lock(order2FromDb, LockMode.None);
            Assert.That(thirdSession.ExistsInSession<Order>(order2FromDb.Id), Is.True, "order not attached");
            Assert.That(secondSession.ExistsInSession<Customer>(order1FromDb.Customer.Id),
                        Is.True,
                        "customer no longer attached to first session");
        }


        #region Examining the behaviour of retrying NHibernate.Session.Flush()

        [Test]
        public void CanRetrySaveUsingAnotherSession()
        {
            //setup - trigger an exception when saving...

            Customer c = Om.CreateInvalidCustomer();

            Nh.CurrentSession.Save(c);

            CommitSessionExpectingException();

            //run test...

            ResetVersionPropertyTo(c, -1);
            c.ShortCode = "ABCDE"; //correct problem that was causing save to fail

            PersistUsingNewSession(c);

            Assert.That(c.ConcurrencyId, Is.EqualTo(1), "customer has been saved");
        }


        [Test]
        public void CanRetryUpdateWhenUsingAnotherSession()
        {
            //setup - trigger an exception when updating...

            Guid id = DbFixture.InsertCustomer();

            var c = Nh.CurrentSession.Get<Customer>(id);

            c.ShortCode = "12345678"; //update will fail when saved to the db

            int versionPropertyBeforeFlush = c.ConcurrencyId;
            CommitSessionExpectingException();

            //run test...

            c.ShortCode = "ABCDE"; //correct problem that was causing save to fail

            PersistUsingNewSession(c);

            Assert.That(c.ConcurrencyId,
                        Is.EqualTo(versionPropertyBeforeFlush + 1),
                        "customer has been updated");
        }


        [Test]
        public void CanRetryUpdateOfChild()
        {
            //setup...

            Guid id = DbFixture.InsertCustomerWithAddress();
            var c = Nh.CurrentSession.Get<Customer>(id);

            Address a = c.AddressAt(0);
            a.Line1 = "Another street";

            c.ShortCode = "CCCCCCCCC"; //update will fail when saved to the db

            CommitSessionExpectingException();

            //run test...

            c.ShortCode = "ABCDE"; //correct problem that was causing save to fail
            ResetVersionPropertyTo(a, 1);

            PersistUsingNewSession(c);

            Assert.That(c.ConcurrencyId, Is.EqualTo(2), "ConcurrencyId correctly incremented");
        }


        [Test]
        public void CanRetryDeleteUsingNewSession()
        {
            //setup - trigger a failure when deleting...

            Guid customerOneId = DbFixture.InsertCustomer();

            var c = Nh.CurrentSession.Get<Customer>(customerOneId);
            Nh.CurrentSession.Delete(c);

            //cause transaction to fail
            Nh.CurrentSession.Save(Om.CreateInvalidCustomer());

            CommitSessionExpectingException();

            //run test...

            using (ISession newSession = Nh.CreateSession())
            {
                newSession.Delete(c);
                newSession.Flush();
            }

            Assert.That(DbFixture.CustomerExists(c), Is.False);
        }


        [Test, Ignore("Currently unable to recover from an exception AND set enable deletion of orphans")]
        public void CanRetryDeletionOfOrphanUsingAnotherSession()
        {
            //setup - trigger an exception when updating...

            Guid customerId = DbFixture.InsertCustomerWithAddress();

            var c = Nh.CurrentSession.Get<Customer>(customerId);
            Guid addressID = c.AddressAt(0).Id;

            c.ShortCode = "12345678"; //update will fail when saved to the db
            c.RemoveAddress(c.AddressAt(0));

            CommitSessionExpectingException();

            Assert.That(DbFixture.AddressExits(addressID), Is.True, "address not deleted");

            //run test...

            c.ShortCode = "ABCDE"; //correct problem that was causing save to fail

            PersistUsingNewSession(c);

            Assert.That(DbFixture.AddressExits(addressID), Is.False, "address is deleted");
        }


        [Test]
        public void CanRetrySaveOfAnAggregateUsingAnotherSession()
        {
            //setup - trigger an exception when updating...

            Customer c = Om.CreateCustomer();
            Address a = Om.CreateAddress();

            c.AddAddress(a);

            Nh.CurrentSession.SaveOrUpdate(c);

            c.ShortCode = "12345678"; //update will fail when saved to the db

            CommitSessionExpectingException();

            //run test...

            c.ShortCode = "ABCDE"; //correct problem that was causing save to fail

            ResetVersionPropertyTo(c, -1);
            ResetVersionPropertyTo(a, -1);
            PersistUsingNewSession(c);

            Assert.That(c.ConcurrencyId, Is.EqualTo(1), "customer has been saved");
            Assert.That(a.ConcurrencyId, Is.EqualTo(1), "address has been saved");
        }


        [Test]
        public void CanRetrySaveOfAnEntityWithValueObjectCollection()
        {
            //setup - trigger an exception inserting objects...

            User u = Om.CreateUser();
            u.AddOtherAddress("Business", Om.CreateInvalidUserAddress());
            Nh.CurrentSession.SaveOrUpdate(u);

            CommitSessionExpectingException();

            //run test

            //fix problem that was causing save to fail
            u.RemoveOtherAddress("Business");
            u.AddOtherAddress("Business", Om.CreateUserAddress("ME5 8HU"));

            //add another address value for good measure
            u.AddOtherAddress("Other", Om.CreateUserAddress("ME5 8HU"));

            ResetVersionPropertyTo(u, -1);
            PersistUsingNewSession(u);

            Assert.That(u.ConcurrencyId, Is.EqualTo(1), "user vs property ok");
        }

        #endregion
    }
}