using System;
using Eca.Commons.Testing;
using Eca.Spikes.NHibernate;
using NUnit.Framework;

namespace Eca.NHibernateUtils.IntegrationTests
{
    /// <summary>
    /// These class provides an excellent example of a pragmatic set of tests
    /// required to verify
    /// - value equivalence (documents properties that are excluded in
    ///   comparison and the object-graph depth of the comparison)
    /// - bidirectional code is in place
    /// </summary>
    /// <remarks>
    /// They are pragmatic in the sense that:
    /// - they assume that the already well tested EquivalenceComparer class is
    ///   being used to perform the equivalence comparison
    /// - it recognises that the set of tests required to verify bidirection
    ///   scalfolding code is in place is unrealistically large and therefore
    ///   verifies just the basics as a way of documenting the classes
    /// </remarks>
    [TestFixture]
    public class BidirAssociationScalffoldingCodeTests
    {
        #region Test helpers

        private static EquivalenceComparer Comparer
        {
            get
            {
                return EquivalenceComparer.For<Customer>()
                    .Excludes(x => x.Number)
                    .With(EquivalenceComparer.For<Address>()
                              .Excludes(x => x.Customer));
            }
        }

        #endregion


        [Test]
        public void CustomersWithEqualValuesAreEquivalent()
        {
            Customer c1 = Om.CreateCustomer();
            Customer c2 = Om.CreateCustomer();

            Assert.That(Comparer.Equals(c1, c2), Is.True);
        }


        [Test]
        public void EquivalenceIgnoresNumberProperty()
        {
            Customer c1 = Om.CreateCustomer();
            Customer c2 = Om.CreateCustomer();

            c1.Number = 10;
            c2.Number = 11;

            Assert.That(Comparer.Equals(c1, c2), Is.True);
        }


        [Test]
        public void CustomersWithDifferentAddressAreNotEquivalent()
        {
            Customer c1 = Om.CreateCustomer();
            Customer c2 = Om.CreateCustomerWithOneAddress();

            Assert.That(Comparer.Equals(c1, c2), Is.False);
        }


        [Test]
        public void AddingAddressTakesCareOfBidirectionalAssociation()
        {
            Customer c = Om.CreateCustomer();

            Address a = Om.CreateAddress();
            c.AddAddress(a);

            Assert.That(c.Addresses, Is.EqualTo(new[] {a}));
        }


        [Test]
        public void NullAddressIsIgnored()
        {
            Customer c = Om.CreateCustomer();
            c.AddAddress(null);

            Assert.That(c.Addresses, Is.Empty);
        }


        [Test]
        public void RemoveAddressTakesCareOfBidirectionalAssociation()
        {
            Customer c = Om.CreateCustomerWithOneAddress();

            c.RemoveAddress(c.AddressAt(0));

            Assert.That(c.Addresses, Is.Empty);
        }
    }
}