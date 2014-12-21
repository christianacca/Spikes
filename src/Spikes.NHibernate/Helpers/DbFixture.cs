using System;
using Eca.Commons.Data.NHibernate.Cfg;
using Eca.Commons.Data.NHibernate.ForTesting;

namespace Eca.Spikes.NHibernate
{
    /// <summary>
    /// Provides services to test cases and database load scripts to create domain objects in the
    /// database.
    /// </summary>
    /// <remarks>
    /// <p>
    /// It is the intent that this class should encapsulate the mechanics of persisting domain
    /// objects behind a simple interface that test classes and database load scripts can make calls
    /// to. This has the effect of simplifying and reducing the amount code:
    /// - required to setup test fixtures
    /// - that perform the *mechanics* of loading objects into a database
    /// This should result in tests and load scripts becoming more focused on their respective jobs.
    /// An equally important benefit will be evident when the domain objects evolve - new
    /// associations are added, existing ones removed, etc. The impact on persistent code is largely
    /// isolated to this single class (or maybe a group of DbFixture classes)
    /// </p>
    /// <p>
    /// Please note however, that this class should not be directly responsible for constructing
    /// domain objects, rather this reponsibility should be delegated to an <see
    /// cref="Om"/> class. This allows this class to concentrate on persistence while the
    /// <see cref="Om"/> to concentrate on domain object construction.
    /// </p>
    /// </remarks>
    public class DbFixture
    {
        #region Class Members

        /// <summary>
        /// Returns the NHibernate mapping meta-data for this test project
        /// </summary>
        public static MappingInfo MappingInfo
        {
            get { return MappingInfo.ForAssemblyContaining<EntityBase>(); }
        }


        public static bool AddressExits(Guid addressID)
        {
            return Nh.ExistsInDb<Address>(addressID);
        }


        public static bool CustomerExists(Customer c)
        {
            return Nh.ExistsInDb<Customer>(c.Id);
        }


        public static Guid Insert(EntityBase entity)
        {
            return Nh.InsertIntoDb(entity);
        }


        public static Guid Insert(Customer customer)
        {
            foreach (var salesRepresentative in customer.CustomerRepresentatives)
            {
                if (salesRepresentative.IsNew) Insert(salesRepresentative);
            }
            return Nh.InsertIntoDb(customer);
        }


        public static Guid InsertCustomer()
        {
            return Insert(Om.CreateCustomer());
        }


        public static Guid InsertCustomerWithAddress()
        {
            return Insert(Om.CreateCustomerWithOneAddress());
        }

        #endregion
    }
}