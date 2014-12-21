using NHibernate.Cfg;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    /// <summary>
    /// Returns the <see cref="Configuration"/> object used to configure NHiberante
    /// </summary>
    public interface INHibernateConfigurator
    {
        /// <summary>
        /// Note to implementors: this property could be called multiple times, 
        /// so you may want to build the <see cref="Configuration"/> object once and then cache this in memory
        /// </summary>
        Configuration NHibernateConfiguration { get; }
    }
}