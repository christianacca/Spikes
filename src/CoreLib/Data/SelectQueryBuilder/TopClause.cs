using System;

//
// Class: TopClause
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This class is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
//

namespace Eca.Commons.Data.SelectQueryBuilder
{
    /// <summary>
    /// Represents a TOP clause for SELECT statements
    /// </summary>
    [Serializable]
    public struct TopClause
    {
        public int Quantity;
        public TopUnit Unit;


        public TopClause(int nr)
        {
            Quantity = nr;
            Unit = TopUnit.Records;
        }


        public TopClause(int nr, TopUnit aUnit)
        {
            Quantity = nr;
            Unit = aUnit;
        }


        public string BuildSql()
        {
            if (Quantity == 100 & Unit == TopUnit.Percent) return String.Empty;

            string topClauseSql = "TOP " + Quantity;
            if (Unit == TopUnit.Percent)
            {
                topClauseSql += " PERCENT";
            }
            topClauseSql += " ";
            return topClauseSql;
        }
    }
}