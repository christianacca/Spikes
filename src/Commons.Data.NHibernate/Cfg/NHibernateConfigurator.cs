#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion


using System;
using System.Collections.Generic;
using Castle.Core;
using NHibernate.Cfg;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    /// <summary>
    /// Builds NHibernate <see cref="Configuration"/> from properties and <see cref="MappingInfo"/> supplied
    /// </summary>
    public class NHibernateConfigurator : INHibernateConfigurator
    {
        #region Member Variables

        private Configuration _configuration;

        #endregion


        #region Constructors

        protected NHibernateConfigurator(MappingInfo mappingInfo, IDictionary<string, string> nhibernateProperties)
        {
            MappingInfo = mappingInfo;
            NHibernateProperties = new Dictionary<string, string>(nhibernateProperties);
        }

        #endregion


        #region Properties

        private MappingInfo MappingInfo { get; set; }

        private IDictionary<string, string> NHibernateProperties { get; set; }

        #endregion


        #region INHibernateConfigurator Members

        public Configuration NHibernateConfiguration
        {
            get { return _configuration = _configuration ?? CreateNHibernateConfig(); }
        }

        #endregion


        public void AdditionalConfiguration(Action<Configuration> configure)
        {
            configure(NHibernateConfiguration);
        }


        private Configuration CreateNHibernateConfig()
        {
            var cfg = new Configuration {Properties = NHibernateProperties};

            MappingInfo.QueryLanguageImports.ForEach(import => cfg.Imports[import.Key] = import.Value);
            MappingInfo.AssembliesWithEmbeddedMapping.ForEach(assembly => cfg.AddAssembly(assembly));
            MappingInfo.AdditionalConfigurations.ForEach(configure => configure(cfg));

            return cfg;
        }


        #region Class Members

        public static NHibernateConfigurator Create(DatabaseEngine databaseEngine,
                                                    DbConnectionInfo dbConnectionInfo,
                                                    MappingInfo mappingInfo)
        {
            IDictionary<string, string> properties = NhCommonConfigurations.For(databaseEngine, dbConnectionInfo, true);
            return new NHibernateConfigurator(mappingInfo, properties);
        }

        #endregion
    }
}