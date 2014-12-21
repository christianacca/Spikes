using System.Collections.Generic;

namespace Eca.Commons.AddressLookup
{
    public interface IAddressLookup<T> where T : LookupAddress
    {
        T GetSingleFormattedAddress(string postCode,
                                    out string errorReason,
                                    out int score,
                                    out bool mustRefine);


        T GetSingleFormattedAddress(string house,
                                    string postCode,
                                    out string errorReason,
                                    out int score,
                                    out bool mustRefine);


        T GetSingleFormattedAddress(string house,
                                    string street,
                                    string town,
                                    out string errorReason,
                                    out int score,
                                    out bool mustRefine);


        T GetSingleFormattedAddress(string moniker);


        IList<T> GetAddressesFor(string postCode,
                                 out string errorReason,
                                 out int score,
                                 out bool mustRefine,
                                 bool includeFormatedAddress);


        IList<T> GetAddressesFor(string houseNumberOrName,
                                 string postCode,
                                 out string errorReason,
                                 out int score,
                                 out bool mustRefine,
                                 bool includeFormatedAddress);


        IList<T> GetAddressesFor(string houseNumberOrName,
                                 string street,
                                 string town,
                                 out string errorReason,
                                 out int score,
                                 out bool mustRefine,
                                 bool includeFormatedAddress);


        int NumberOfMatchingAddressesFor(string postCode,
                                         out string errorReason,
                                         out int score,
                                         out bool mustRefine);


        int NumberOfMatchingAddressesFor(string house,
                                         string postCode,
                                         out string errorReason,
                                         out int score,
                                         out bool mustRefine);


        int NumberOfMatchingAddressesFor(string house,
                                         string street,
                                         string town,
                                         out string errorReason,
                                         out int score,
                                         out bool mustRefine);
    }
}