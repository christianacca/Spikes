using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Eca.Commons;
using NValidate.Framework;

namespace Eca.Spikes.NHibernate
{
    public class Customer : EntityBase
    {
        #region Member Variables

        private readonly IList<Address> _addresses = new List<Address>();

        #endregion


        #region Constructors

        public Customer()
        {
            ShortCode = String.Empty;
        }


        public Customer(string name)
        {
            ShortCode = String.Empty;
            Name = name;
        }

        #endregion


        #region Properties

        public ICollection<Address> Addresses
        {
            get { return new ReadOnlyCollection<Address>(_addresses); }
        }

        public ICollection<Address> FriendAddresses
        {
            get { return _addresses; }
        }


        public string Name { get; set; }

        public int Number { get; set; }

        public string ShortCode { get; set; }

        #endregion


        public virtual void AddAddress(Address address)
        {
            Check.Require(() => Demand.The.Param(() => address).IsNotOneOf(_addresses));

            if (address == null) return;
            address.Customer = this;
        }


        public virtual void AddAddresss(IEnumerable<Address> addresss)
        {
            foreach (Address address in addresss)
                AddAddress(address);
        }


        public virtual Address AddressAt(int index)
        {
            return _addresses[index];
        }


        public virtual bool HasAddress(Address address)
        {
            return _addresses.Contains(address);
        }


        public virtual int NumberOfAddresss()
        {
            return _addresses.Count;
        }


        public virtual void RemoveAddress(Address address)
        {
            if (address == null) return;
            address.Customer = null;
        }


        #region CustomerRepresentatives simple encapsulation


        #region Member Variables

        private readonly IList<CustomerRepresentative> _customerRepresentatives = new List<CustomerRepresentative>();

        #endregion


        #region Properties

        public virtual IEnumerable<CustomerRepresentative> CustomerRepresentatives
        {
            get { return _customerRepresentatives; }
        }

        public virtual int NumberOfCustomerRepresentatives
        {
            get { return _customerRepresentatives.Count; }
        }

        #endregion


        public virtual void AddCustomerRepresentative(CustomerRepresentative customerRepresentative)
        {
            //if CustomerRepresentative is a ValueObject this precondition may not make sense - please consult design 
            Check.Require(() => Demand.The.Param(() => _customerRepresentatives).DoesNotContain(customerRepresentative));

            _customerRepresentatives.Add(customerRepresentative);
        }


        public virtual void AddSalesRepresentatives(IEnumerable<CustomerRepresentative> salesRepresentatives)
        {
            foreach (var salesRepresentative in salesRepresentatives)
            {
                AddCustomerRepresentative(salesRepresentative);
            }
        }


        public virtual bool HasSalesRepresentative(CustomerRepresentative customerRepresentative)
        {
            return _customerRepresentatives.Contains(customerRepresentative);
        }


        public virtual void RemoveSalesRepresentative(CustomerRepresentative customerRepresentative)
        {
            _customerRepresentatives.Remove(customerRepresentative);
        }

        #endregion
    }
}