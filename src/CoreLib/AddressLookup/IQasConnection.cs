using System;

namespace Eca.Commons.AddressLookup
{
    public interface IQasConnection
    {
        string AddressLayout { get; }
        string ServerUrl { get; }
    }
}