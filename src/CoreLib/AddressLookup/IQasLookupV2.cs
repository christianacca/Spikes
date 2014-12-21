using System.Collections.Generic;

namespace Eca.Commons.AddressLookup
{
    public interface IQasLookupV2 : IQasLookupBase
    {
        ExtendedAddress GetSingleFormattedAddress(string postCode,
                                                  out string errorReason,
                                                  out int score,
                                                  out bool mustRefine);


        ExtendedAddress GetSingleFormattedAddress(string house,
                                                  string postCode,
                                                  out string errorReason,
                                                  out int score,
                                                  out bool mustRefine);


        ExtendedAddress GetSingleFormattedAddress(string house,
                                                  string street,
                                                  string town,
                                                  out string errorReason,
                                                  out int score,
                                                  out bool mustRefine);


        ExtendedAddress GetSingleFormattedAddress(string moniker);


        IList<ExtendedAddress> GetAddressesFor(string postCode,
                                               out string errorReason,
                                               out int score,
                                               out bool mustRefine,
                                               bool includeFormatedAddress);


        IList<ExtendedAddress> GetAddressesFor(string houseNumberOrName,
                                               string postCode,
                                               out string errorReason,
                                               out int score,
                                               out bool mustRefine,
                                               bool includeFormatedAddress);


        IList<ExtendedAddress> GetAddressesFor(string houseNumberOrName,
                                               string street,
                                               string town,
                                               out string errorReason,
                                               out int score,
                                               out bool mustRefine,
                                               bool includeFormatedAddress);
    }
}