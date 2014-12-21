namespace Eca.Commons.AddressLookup
{
    public class ExtendedAddress : LookupAddress
    {
        #region AddressLayOut enum

        public enum AddressLayOut
        {
            AddressL1 = 0,
            AddressL2,
            AddressL3,
            Town,
            County,
            Postcode,
            Country,
            SubBuildingName,
            BuildingNumberName,
            Thoroughfare,
            Locality,
            LocalAuthorityCode,
            LocalAuthorityName,
            GovOfficeForRegionName
        }

        #endregion


        #region Member Variables

        private string _country;
        private string _county;
        private string _flatPlot;
        private string _govOfficeForRegionName;
        private string _line2;
        private string _line3;
        private string _localAuthorityCode;
        private string _localAuthorityName;
        private string _locality;
        private string _numberName;
        private string _street;
        private string _workAddressUprn;

        #endregion


        #region Constructors

        public ExtendedAddress() {}


        /// <summary>
        /// Creates this object adding a Flat QAS address line and its moniker, with 0 as the score
        /// </summary>
        /// <param name="singleLine">QAS flat address line</param>
        /// <param name="moniker">The moniker associated with the flat address line</param>
        public ExtendedAddress(string singleLine, string moniker)
            : base(singleLine, moniker, 0) {}


        /// <summary>
        /// Creates this object adding a Flat QAS address line, its moniker and score
        /// </summary>
        /// <param name="singleLine">QAS flat address line</param>
        /// <param name="moniker">The moniker associated with the flat address line</param>
        /// <param name="score">The score of the address line</param>
        public ExtendedAddress(string singleLine, string moniker, int score) : base(singleLine, moniker, score) {}


        public ExtendedAddress(string line1,
                               string line2,
                               string line3,
                               string town,
                               string county,
                               string postcode,
                               string country,
                               string localAuthorityCode,
                               string localAuthorityName,
                               string govOfficeForRegionName,
                               string flatPlot,
                               string numberName,
                               string street,
                               string locality,
                               string workAddressUprn)
            : base(line1,
                   town,
                   postcode)

        {
            _line2 = line2;
            _line3 = line3;
            _county = county;
            _country = country;
            _localAuthorityCode = localAuthorityCode;
            _localAuthorityName = localAuthorityName;
            _govOfficeForRegionName = govOfficeForRegionName;
            _flatPlot = flatPlot;
            _workAddressUprn = workAddressUprn;
            _numberName = numberName;
            _street = street;
            _locality = locality;
        }

        #endregion


        #region Properties

        public virtual string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public virtual string County
        {
            get { return _county; }
            set { _county = value; }
        }

        public virtual string FlatPlot
        {
            get { return _flatPlot; }
            set { _flatPlot = value; }
        }

        public virtual string GovOfficeForRegionName
        {
            get { return _govOfficeForRegionName; }
            set { _govOfficeForRegionName = value; }
        }


        public virtual string Line2
        {
            get { return _line2; }
            set { _line2 = value; }
        }

        public virtual string Line3
        {
            get { return _line3; }
            set { _line3 = value; }
        }

        public virtual string LocalAuthorityCode
        {
            get { return _localAuthorityCode; }
            set { _localAuthorityCode = value; }
        }

        public virtual string LocalAuthorityName
        {
            get { return _localAuthorityName; }
            set { _localAuthorityName = value; }
        }

        public virtual string Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }

        public virtual string NumberName
        {
            get { return _numberName; }
            set { _numberName = value; }
        }

        public virtual string Street
        {
            get { return _street; }
            set { _street = value; }
        }

        public virtual string WorkAddressUprn
        {
            get { return _workAddressUprn; }
            set { _workAddressUprn = value; }
        }

        #endregion
    }
}