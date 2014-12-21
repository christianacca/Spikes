using System;

namespace Eca.Spikes.NHibernate
{
    public class Order : EntityBase
    {
        private string _name;
        private Customer _customer;

        public virtual Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
            }
        }
        public virtual string Name
        {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
    }
}