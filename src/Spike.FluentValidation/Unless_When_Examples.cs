using FluentValidation;
using NUnit.Framework;
using Spike.FluentValidation.ExampleModel;

namespace Spike.FluentValidation
{
    [TestFixture]
    public class Unless_When_Examples
    {
        [Test]
        public void UnlessAppliesToAllContraintsForSamePropertyUpToUnless()
        {
            var badCustomer = new Customer {Age = -5};

            var validator = new CustomerValidator();
            validator.ShouldHaveValidationErrorFor(c => c.Age, badCustomer);
            validator.ShouldNotHaveValidationErrorFor(c => c.Address.HouseNumber, badCustomer);
            badCustomer.Address = new Address {HouseNumber = -1};
            validator.ShouldHaveValidationErrorFor(c => c.Address.HouseNumber, badCustomer);

            var goodCustomer = new Customer {Age = -5, Name = "Christian", Address = new Address {HouseNumber = -1}};
            validator.ShouldNotHaveValidationErrorFor(c => c.Age, goodCustomer);
            validator.ShouldNotHaveValidationErrorFor(c => c.Address.HouseNumber, goodCustomer);
        }


        [Test]
        public void UnlessDoesNotApplyToContraintsAfterUnless()
        {
            var badCustomer = new Customer {Name = "", Address = new Address()};

            var validator = new CustomerValidator();
            validator.ShouldHaveValidationErrorFor(c => c.Name, badCustomer);

            badCustomer = new Customer {Name = "Therese"};
            validator.ShouldHaveValidationErrorFor(c => c.Name, badCustomer);

            var goodCustomer = new Customer {Name = ""};
            validator.ShouldNotHaveValidationErrorFor(c => c.Name, goodCustomer);
        }


        [Test]
        public void WhenAppliesToAllContraintsForSamePropertyUpToUnless()
        {
            var badCustomer = new Customer {Address = new Address {Postcode = "BR6"}};

            var validator = new CustomerValidator();
            validator.ShouldHaveValidationErrorFor(c => c.Address.Postcode, badCustomer);

            var goodCustomer = new Customer();
            validator.ShouldNotHaveValidationErrorFor(c => c.Address.Postcode, goodCustomer);
        }



        private class CustomerValidator : AbstractValidator<Customer>
        {
            #region Constructors

            public CustomerValidator()
            {
                RuleFor(customer => customer.Age).Cascade(CascadeMode.Continue)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(100)
                    .Unless(customer => customer.Name == "Christian");

                RuleFor(customer => customer.Address.HouseNumber).Cascade(CascadeMode.Continue)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(16)
                    .Unless(customer => customer.Name == "Christian" || customer.Address == null);

                RuleFor(customer => customer.Address.Postcode)
                    .NotEqual("BR6")
                    .NotEqual("ME5 8HU")
                    .When(customer => customer.Address != null);

                RuleFor(customer => customer.Name)
                    .NotEmpty()
                    .Unless(customer => customer.Address == null)
                    .NotEqual("Therese");
            }

            #endregion
        }
    }
}