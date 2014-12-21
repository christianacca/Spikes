namespace Eca.Spikes.NHibernate
{
    public class Address : EntityBase
    {
        #region Member Variables

        private Customer _customer;

        #endregion


        //required by NHibernate


        #region Constructors

        private Address() {}


        public Address(Customer customer) : this()
        {
            _customer = customer;
        }

        #endregion


        #region Properties

        public string County { get; set; }

        public virtual Customer Customer
        {
            get { return _customer; }
            set
            {
                if (_customer != null)
                {
                    _customer.FriendAddresses.Remove(this);
                }
                if (value != null)
                {
                    value.FriendAddresses.Add(this);
                }
                _customer = value;
            }
        }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string PostCode { get; set; }

        public string Town { get; set; }

        #endregion
    }
}