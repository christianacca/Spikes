namespace Eca.Commons.AddressLookup
{
    public interface IQasLookupBase
    {
        string ProwebLayout { get; set; }
        string ProwebServerUrl { get; set; }


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