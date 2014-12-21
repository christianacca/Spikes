using System;
using System.Collections;
using System.Collections.Generic;
using Eca.Commons.Data.NHibernate.ForTesting;
using Eca.Commons.Testing;
using Iesi.Collections;
using NHibernate;
using NHibernate.Criterion;
using NUnit.Framework;

namespace Eca.Spikes.NHibernate
{
    [TestFixture]
    public class NHibernateCriteriaSpikes : ThisProjectDatabaseTestsBase
    {
        private List<Customer> _customersInDb;


        #region Setup/Teardown

        [SetUp]
        public void NHibernateCriteriaSpikesSetup()
        {
            Customer c1 = Om.CreateCustomerWithOneAddress();
            Customer c2 = Om.CreateCustomerWithOneAddress();
            c2.AddAddress(Om.CreateAddress("ME5 8HU"));
            Customer c3 = Om.CreateCustomer();
            c3.Name = "Mervyn Ramos";
            Customer c4 = Om.CreateCustomer();
            c4.ShortCode = "009";

            _customersInDb = new List<Customer>();
            _customersInDb.AddRange(new[] {c1, c2, c3, c4});

            Nh.InsertIntoDb(_customersInDb);
        }

        #endregion


        #region Test helpers

        protected override EquivalenceComparer CreateComparer()
        {
            return EquivalenceComparer.Default;
        }


        private static ICollection ExpectedList(params object[] items)
        {
            return items;
        }

        #endregion


        [Test]
        public void CanRetrieveAllCustomers()
        {
            ICriteria criteria = Nh.CurrentSession.CreateCriteria(typeof (Customer));
            criteria.SetResultTransformer(CriteriaSpecification.DistinctRootEntity);
            IList loadedList = criteria.List();

            Assert.That(loadedList.Count, Is.EqualTo(_customersInDb.Count));
        }


        [Test]
        public void CanFilterOnEntityProperty()
        {
            ICriteria criteria = Nh.CurrentSession.CreateCriteria(typeof (Customer));
            criteria.Add(Expression.Eq("Name", "George"));
            criteria.SetResultTransformer(CriteriaSpecification.DistinctRootEntity);

            IList loadedList = criteria.List();

            Assert.That(loadedList.Count, Is.EqualTo(3));
        }


        [Test]
        public void ExistingReferencesWillBeReturned()
        {
            Customer c1 = _customersInDb[0];
            Customer c2 = _customersInDb[1];
            Customer c4 = _customersInDb[3];

            //attach instances to session
            Nh.CurrentSession.Lock(c1, LockMode.None);
            Nh.CurrentSession.Lock(c2, LockMode.None);
            Nh.CurrentSession.Lock(c4, LockMode.None);

            ICriteria criteria = Nh.CurrentSession.CreateCriteria(typeof (Customer));
            criteria.Add(Expression.Eq("Name", "George"));
            criteria.SetResultTransformer(CriteriaSpecification.DistinctRootEntity);

            IList loadedCustomers = criteria.List();

            Assert.That(loadedCustomers, Is.EquivalentTo(ExpectedList(c1, c2, c4)));
        }


        [Test]
        public void CanFilterOnEntityProperty_UsingHQL()
        {
            //we have to explicitly ask that addresses be eagerly
            //fetched and not rely on a outer-join="true" setting in the
            //mapping file if we *want* eager fetching of address instances
            const string hql =
                @"
							from Customer c 
							left join fetch c.Addresses 
							where c.Name = 'George'";
            IQuery q = Nh.CurrentSession.CreateQuery(hql);

            //we're using a set here so that duplicate references
            //to the returned entity are stripped out
            ISet loadedList = new HashedSet(q.List());
            Assert.That(loadedList.Count, Is.EqualTo(3));
        }


        [Test]
        public void CanFilterOnAssociatedEntityProperties()
        {
            ICriteria criteria = Nh.CurrentSession.CreateCriteria(typeof (Customer));
            ICriteria addressCriteria = criteria.CreateCriteria("Addresses");
            addressCriteria.Add(Expression.Eq("PostCode", "ME5 8HU"));
            criteria.SetResultTransformer(CriteriaSpecification.DistinctRootEntity);

            Assert.That(criteria.List().Count, Is.EqualTo(1), "one customer returned");

            var c = (Customer) criteria.List()[0];
            Assert.That(c.Addresses.Count, Is.EqualTo(2), "customer loaded with all its addresses");
        }


        [Test]
        public void CanCountCustomersWithoutLoadingThemIntoSession()
        {
            Nh.CurrentSession.CreateQuery("select count(*) from Customer").UniqueResult();

            Assert.That(Nh.CurrentSession.Contains(_customersInDb[0]), Is.False);
        }


        [Test]
        public void CanCountChildEntitiesWithoutLoadingThenIntoSession()
        {
            var customer = Nh.CurrentSession.Get<Customer>(_customersInDb[0].Id);
            Address address = customer.AddressAt(0);

            using (ISession newSession = Nh.CreateSession())
            {
                newSession.Get<Customer>(customer.Id);
                Assert.That(newSession.Contains(address), Is.False, "Not testing yet just clarifying assumptions");

                const string querySring = "select count(*) from Address as a where a.Customer.Id=:custId";
                newSession.CreateQuery(querySring).SetGuid("custId", customer.Id).UniqueResult();

                Assert.That(newSession.Contains(address), Is.False, "child object not loaded");
            }
        }
    }
}