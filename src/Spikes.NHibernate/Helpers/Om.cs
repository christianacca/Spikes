using System;

namespace Eca.Spikes.NHibernate
{
    public class Om
    {
        #region Class Members

        private static SimpleIdGenerator idGenerator = new SimpleIdGenerator();

        private static int NextId
        {
            get { return idGenerator.NextId; }
        }


        public static UserAddress AddAddressTo(User user, string addressType)
        {
            UserAddress result = CreateUserAddress();
            user.AddOtherAddress(addressType, result);

            return result;
        }


        public static void AddCustomerRepToCustomer(Customer customer)
        {
            customer.AddCustomerRepresentative(CreateCustomerRep());
        }


        public static Address CreateAddress()
        {
            var a = new Address(CreateCustomer())
                        {
                            Line1 = "Flat 46 Swallow Close",
                            Line2 = "Greenhithe",
                            Town = "Darford",
                            County = "Kent",
                            PostCode = "BR6 0JA"
                        };
            return a;
        }


        public static Address CreateAddress(string postCode)
        {
            Address a = CreateAddress();
            a.PostCode = postCode;
            return a;
        }


        public static Customer CreateCustomer()
        {
            return CreateCustomer(NextId);
        }


        public static Customer CreateCustomer(int number)
        {
            return new Customer("George") {Number = number, ShortCode = "CCCCC"};
        }


        public static CustomerRepresentative CreateCustomerRep()
        {
            return new CustomerRepresentative {FirstName = "Christian", LastName = "Crowhurst"};
        }


        public static Customer CreateCustomerWithAddresses(int numberOfAddresses)
        {
            Customer result = CreateCustomer();

            for (int i = 0; i < numberOfAddresses; i++)
                result.AddAddress(CreateAddress());

            return result;
        }


        public static Customer CreateCustomerWithOneAddress()
        {
            return CreateCustomerWithAddresses(1);
        }


        public static Customer CreateCustomerWithOneRep()
        {
            Customer customer = CreateCustomer();
            AddCustomerRepToCustomer(customer);
            return customer;
        }


        public static Customer CreateInvalidCustomer()
        {
            Customer result = CreateCustomer(NextId);
            result.ShortCode = "CCCCCCCCCC"; // this is too long for the column
            return result;
        }


        public static UserAddress CreateInvalidUserAddress()
        {
            return CreateUserAddress(null);
        }


        public static Order CreateOrder()
        {
            return CreateOrder("Priority");
        }


        public static Order CreateOrder(string name)
        {
            return new Order {Name = name};
        }


        public static Order CreateOrderWith(Customer customer)
        {
            Order order = CreateOrder();
            order.Customer = customer;
            return order;
        }


        public static User CreateUser()
        {
            return CreateUser("ABCDE");
        }


        public static User CreateUser(string shortCode)
        {
            var u = new User("C", "C");
            u.ShortCode = shortCode;
            u.Number = NextId;
            return u;
        }


        public static UserAddress CreateUserAddress()
        {
            return CreateUserAddress("DA9 9PT");
        }


        public static UserAddress CreateUserAddress(string postCode)
        {
            return new UserAddress("Flat 46 Swallow Close"
                                   ,
                                   "Greenhithe"
                                   ,
                                   "Dartford"
                                   ,
                                   "Kent",
                                   postCode);
        }


        public static void SetIdGenerator(SimpleIdGenerator generator)
        {
            idGenerator = generator;
        }

        #endregion
    }



    public class SimpleIdGenerator
    {
        #region Member Variables

        private int nextID;

        #endregion


        #region Properties

        public int NextId
        {
            get { return ++nextID; }
        }

        #endregion
    }
}