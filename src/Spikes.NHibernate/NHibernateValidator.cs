using System;
using System.Reflection;
using Eca.Commons.Data.NHibernate.Cfg;
using Eca.Commons.Data.NHibernate.ForTesting;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Engine;
using NUnit.Framework;

namespace Eca.Spikes.NHibernate
{
    [TestFixture]
    public class NHibernateValidator
    {
        private ValidatorEngine _validatorEngine;


        #region Setup/Teardown

        [TestFixtureSetUp]
        public virtual void ThisProjectDatabaseTestsBaseFixtureSetup()
        {
            var validatorConfiguration = new FluentConfiguration();
            validatorConfiguration
                .Register(Assembly.GetExecutingAssembly().ValidationDefinitions())
                .SetDefaultValidatorMode(ValidatorMode.UseExternal)
                .IntegrateWithNHibernate.ApplyingDDLConstraints();

            _validatorEngine = new ValidatorEngine();
            _validatorEngine.Configure(validatorConfiguration);

            NHibernateTestContext.Initialize(DbFixture.MappingInfo)
                .Using(DatabaseEngine.MsSql2005, null)
                .AdditionalConfiguration(cfg => cfg.Initialize(_validatorEngine))
                .AfterInitialization(factory => {
                    Nh.SessionFactory = factory;
                }).Now();
        }

        #endregion


        [Test]
        public void CreateDatabase()
        {
            //notice how the tblCustomer.Number column is created with a Constraint
            NHibernateTestContext.CurrentContext.SetupDatabase(true);
        }
    }



    public class CustomerValidator : ValidationDef<Customer>
    {
        #region Constructors

        public CustomerValidator()
        {
            Define(x => x.Name).NotNullableAndNotEmpty().And.MaxLength(10).And.MinLength(5);
            Define(x => x.Number).IncludedBetween(10, 15);
        }

        #endregion
    }
}