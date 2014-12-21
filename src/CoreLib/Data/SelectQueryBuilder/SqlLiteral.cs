using System;

//
// Class: SqlLiteral
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
// 

namespace Eca.Commons.Data.SelectQueryBuilder
{
    [Serializable]
    public class SqlLiteral
    {
        #region Member Variables

        private string _value;

        #endregion


        #region Constructors

        public SqlLiteral(string value)
        {
            _value = value;
        }

        #endregion


        #region Properties

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion


        #region Class Members

        public static string StatementRowsAffected = "SELECT @@ROWCOUNT";

        #endregion
    }
}