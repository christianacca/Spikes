using NHibernate.Cfg;

namespace Eca.Commons.Data.NHibernate
{
    public interface INHibernateDbBuilder
    {
        Configuration NHibernateConfiguration { get; }


        /// <summary>
        /// Creates the database, deleting any existing database with the same name
        /// </summary>
        void CreateDatabase();


        /// <summary>
        /// Creates the database, deleting any existing database with the same name
        /// </summary>
        /// <param name="connectionOverride">The database name and/or server that should override the current NHibernate configuration</param>
        void CreateDatabase(DbConnectionInfo connectionOverride);
    }
}