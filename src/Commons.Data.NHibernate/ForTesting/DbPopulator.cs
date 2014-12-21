using NHibernate;

namespace Eca.Commons.Data.NHibernate.ForTesting
{
    /// <summary>
    /// Populates an NHibernate mapped database
    /// </summary>
    /// <remarks>
    /// You will need to inherit from this class if you want to use Eca.NHibernate.Data.Console.exe to populate the database
    /// </remarks>
    public abstract class DbPopulator
    {
        #region Constructors

        protected DbPopulator(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        #endregion


        #region Properties

        public ISessionFactory SessionFactory { get; private set; }

        #endregion


        public abstract void Execute();
    }
}