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
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;

namespace Eca.Commons.Data.NHibernate.Cfg
{
    /// <summary>
    /// Data holder for NHibernate mapping information
    /// </summary>
    public class MappingInfo
    {
        #region Constructors

        public MappingInfo() : this(Enumerable.Empty<Assembly>()) {}


        public MappingInfo(IEnumerable<Assembly> assembliesWithEmbeddedMapping)
        {
            AssembliesWithEmbeddedMapping = new List<Assembly>(assembliesWithEmbeddedMapping);
            QueryLanguageImports = new Dictionary<string, string>();
            AdditionalConfigurations = new List<Action<Configuration>>();
        }

        #endregion


        #region Properties

        public ICollection<Action<Configuration>> AdditionalConfigurations { get; private set; }


        /// <summary>
        /// Assemblies that contain one or more embedded hbm mapping files that will be added to NHibernate to tell it which
        /// classes are mapped to database tables
        /// </summary>
        public ICollection<Assembly> AssembliesWithEmbeddedMapping { get; private set; }

        public IDictionary<string, string> QueryLanguageImports { get; private set; }

        #endregion


        public MappingInfo AddQueryLanguageImport(string alias, Type type)
        {
            QueryLanguageImports.Add(alias,
                                     string.Format("{0}, {1}",
                                                   type.FullName,
                                                   type.Assembly.GetName().Name));

            return this;
        }


        /// <summary>
        /// Tell NHibernate about the types that are referenced in HQL queries.
        /// </summary>
        /// <remarks>
        /// This method makes a simple assumption: the name of a type referenced in HQL is the same as the 
        /// unqalified name of the actual class/struct. If alias names are used, then
        /// register these using <see cref="AddQueryLanguageImport"/>
        /// </remarks>
        public MappingInfo AddQueryLanguageImports(params Type[] types)
        {
            foreach (Type type in types)
                AddQueryLanguageImport(type.Name, type);

            return this;
        }


        #region Class Members

        /// <summary>
        /// Shorthand for creating mapping information with the assembly containing the type <typeparamref name="T"/> added to 
        /// <see cref="AssembliesWithEmbeddedMapping"/>
        /// </summary>
        /// <typeparam name="T">A type from an assembly that contains embedded hbm mappings</typeparam>
        public static MappingInfo ForAssemblyContaining<T>()
        {
            return new MappingInfo(new[] {typeof (T).Assembly});
        }

        #endregion
    }
}