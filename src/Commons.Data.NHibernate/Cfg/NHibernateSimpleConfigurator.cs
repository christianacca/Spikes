using System;
using NHibernate.Cfg;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    public class NHibernateSimpleConfigurator : INHibernateConfigurator
    {
        #region Member Variables

        private readonly Configuration _configuration;

        #endregion


        #region Constructors

        public NHibernateSimpleConfigurator(Configuration configuration)
        {
            _configuration = configuration;
        }

        #endregion


        #region INHibernateConfigurator Members

        public Configuration NHibernateConfiguration
        {
            get { return _configuration; }
        }

        #endregion
    }
}