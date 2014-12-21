using System.Collections.Generic;
using Eca.Commons.Data;
using Eca.Commons.Data.NHibernate;
using Eca.Commons.Data.NHibernate.Cfg;
using Eca.Commons.Testing;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Eca.Spikes.NHibernate
{
    [TestFixture]
    public class MyFirstNHibernateTest
    {
        [Test]
        public void TemplateFor_CanInsertCustomer()
        {
            //given
            //0.drop the physical database if already exists
            //1.create the physical database
            //2.configure nhibernate
            //3.create the database schema
            //4.setup session factory

            //5.Create a new customer

            //when
            //6.open a session
            //7.make customer a persistent instance (using the session)
            //8.open transaction
            //9.flush the session to the database
            //10.commit transaction

            //then
            //11.open a new session
            //12.retrieve persistent customer as stored in the database
            //13.recursively compare all the 'relevant' properties of:
            //   - the customer constructed in the test
            //   - the customer as retrieved from the database
        }


        [Test]
        public void CanInsertCustomer()
        {
            //given
            //0.drop the physical database if already exists AND 1.create the physical database
            var connectionInfo = new DbConnectionInfo {DatabaseName = "MyFirstNHibernateTest"};
            DbMediaBuilder mediaBuilder
                = DbMediaBuilder.For(DatabaseEngine.MsSql2005, connectionInfo);
            mediaBuilder.CreateDatabaseMedia();
            //2.configure nhibernate
            IDictionary<string, string> sqlServerConfigs
                = NhCommonConfigurations.MsSqlServer(connectionInfo);
            var configuration = new Configuration { Properties = sqlServerConfigs };
            configuration.AddAssembly(typeof(Customer).Assembly);
            //3.create the database schema
            new SchemaExport(configuration).Execute(true, true, false);

            //4.setup session factory
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();

            //5.Create a new customer
            var customer = new Customer("Brian Allen");

            //when
            //6.open a session
            ISession session = sessionFactory.OpenSession();
            //7.make customer a persistent instance (using the session)
            session.Save(customer);
            //8.open transaction
            using (ITransaction tx = session.BeginTransaction())
            {
                //9.flush the session to the database
                //  ??
                //10.commit transaction
                tx.Commit();
            }

            //11.open a new session
            using (ISession newSession = sessionFactory.OpenSession())
            {
                //12.retrieve persistent customer as stored in the database
                var customerFromDb = newSession.Get<Customer>(customer.Id);

                //13.recursively compare all the 'relevant' properties of:
                //   - the customer constructed in the test
                //   - the customer as retrieved from the database
                EquivalenceComparer comparer = EquivalenceComparer.For<Customer>();
                IEnumerable<string> propertiesNotEqual = comparer.PropertiesNotEqual(customer, customerFromDb);
                Assert.That(propertiesNotEqual, Is.Empty);
            }
        }
    }
}