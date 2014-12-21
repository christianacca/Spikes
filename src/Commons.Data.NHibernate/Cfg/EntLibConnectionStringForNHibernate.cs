using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NHibernate.Connection;
using Environment = NHibernate.Cfg.Environment;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    public class EntLibConnectionStringForNHibernate : DriverConnectionProvider
    {
        protected override string GetNamedConnectionString(IDictionary<string, string> settings)
        {
            if (!settings.ContainsKey(Environment.ConnectionStringName)) return base.GetNamedConnectionString(settings);

            string result = GetConnectionStringFromEntLibConfigFile(settings[Environment.ConnectionStringName]);
            return result ?? base.GetNamedConnectionString(settings);
        }


        #region Class Members

        public static string GetConnectionStringFromEntLibConfigFile(string databaseConfigKey)
        {
            string entLibConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                       "EnterpriseLibrary.config");

            if (!File.Exists(entLibConfigFilePath)) return null;

            var entLibConfigDocument = new XmlDocument();
            entLibConfigDocument.Load(entLibConfigFilePath);

            string connectionXPath =
                string.Format(@"/configuration/connectionStrings/add[@name='{0}']/@connectionString",
                              databaseConfigKey);
            var databaseConfig = entLibConfigDocument.SelectSingleNode(connectionXPath);
            return databaseConfig != null ? databaseConfig.Value : null;
        }

        #endregion
    }
}