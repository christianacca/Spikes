using System;

//
// Interface: IQueryBuilder
// Copyright 2006 by Ewout Stortenbeker
// Email: 4ewout@gmail.com
//
// This interface is part of the CodeEngine Framework.
// You can download the framework DLL at http://www.code-engine.com/
// 

namespace Eca.Commons.Data.SelectQueryBuilder
{
    public interface IQueryBuilder
    {
        string BuildQuery();
    }
}