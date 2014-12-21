using Eca.Commons.Data.NHibernate.Cfg;
using Eca.Commons.Data.NHibernate.ForTesting;
using Eca.Commons.Testing.NHibernate;
using NUnit.Framework;

namespace Eca.Spikes.NHibernate
{
    [TestFixture]
    public class ThisProjectDatabaseTestsBase : NHibernateTestsBase
    {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public virtual void ThisProjectDatabaseTestsBaseFixtureSetup()
        {
            NHibernateTestContext.Initialize(DbFixture.MappingInfo)
                .Using(DefaultDatabaseEngine, null)
                .AfterInitialization(factory => {
                    Nh.SessionFactory = factory;
                }).Now();
        }

        #endregion


        public ThisProjectDatabaseTestsBase()
        {
            //note: DatabaseEngine.MsSqlCe is the only database engine I have found that will throw when trying to insert a string that gets truncated;
            //its probably possible to make other engines doing the same, but don't have time to find out how. Hence, for these test we need to use
            //DatabaseEngine.MsSqlCe 
            DefaultDatabaseEngine = DatabaseEngine.MsSqlCe;
        }
    }
}