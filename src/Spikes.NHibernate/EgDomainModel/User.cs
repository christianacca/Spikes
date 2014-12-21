using System.Collections.Generic;

namespace Eca.Spikes.NHibernate
{
    public class User : EntityBase
    {
        #region Member Variables

        private readonly IDictionary<string, UserAddress> _otherAddresses = new Dictionary<string, UserAddress>();

        #endregion


        #region Constructors

        /// <summary>
        /// required by NHibernate
        /// </summary>
        private User() {}


        public User(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            HomeAddress = UserAddress.NullObject;
        }

        #endregion


        #region Properties

        public string FirstName { get; set; }

        public UserAddress HomeAddress { get; set; }

        public string LastName { get; set; }

        public int Number { get; set; }


        public virtual IDictionary<string, UserAddress> OtherAddresses
        {
            get { return new Dictionary<string, UserAddress>(_otherAddresses); }
        }

        public string ShortCode { get; set; }

        #endregion


        public virtual void AddOtherAddress(string addressType, UserAddress otherAddress)
        {
            _otherAddresses.Add(addressType, otherAddress);
        }


        public virtual void AddOtherAddresss(IDictionary<string, UserAddress> otherAddresses)
        {
            foreach (KeyValuePair<string, UserAddress> entry in otherAddresses)
            {
                AddOtherAddress(entry.Key, entry.Value);
            }
        }


        public virtual bool HasOtherAddressFor(string addressType)
        {
            return _otherAddresses.ContainsKey(addressType);
        }


        public virtual int NumberOfOtherAddresses()
        {
            return _otherAddresses.Count;
        }


        public virtual UserAddress OtherAddressFor(string addressType)
        {
            return _otherAddresses[addressType];
        }


        public virtual void RemoveOtherAddress(string addressType)
        {
            _otherAddresses.Remove(addressType);
        }
    }
}