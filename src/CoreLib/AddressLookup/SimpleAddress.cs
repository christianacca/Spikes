namespace Eca.Commons.AddressLookup
{
    /// <summary>
    /// Holds address line data from QAS
    /// ***The address layout out in the QAS database needs to be in the order of the enum AddressLayOut***
    /// </summary>
    public class SimpleAddress : LookupAddress
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
            LocalAuthorityCode,
            LocalAuthorityName,
            GovOfficeForRegionName
        }

        #endregion


        #region Member Variables

        private string _country;
        private string _county;
        private string _govOfficeForRegionName;
        private string _line2;
        private string _line3;
        private string _localAuthorityCode;
        private string _localAuthorityName;

        #endregion


        #region Constructors

        public SimpleAddress() {}


        /// <summary>
        /// Creates this object adding a Flat QAS address line and its moniker, with 0 as the score
        /// </summary>
        /// <param name="singleLine">QAS flat address line</param>
        /// <param name="moniker">The moniker associated with the flat address line</param>
        public SimpleAddress(string singleLine, string moniker)
            : this(singleLine, moniker, 0) {}


        /// <summary>
        /// Creates this object adding a Flat QAS address line, its moniker and score
        /// </summary>
        /// <param name="singleLine">QAS flat address line</param>
        /// <param name="moniker">The moniker associated with the flat address line</param>
        /// <param name="score">The score of the address line</param>
        public SimpleAddress(string singleLine, string moniker, int score) : base(singleLine, moniker, score) {}


        public SimpleAddress(string line1,
                             string line2,
                             string line3,
                             string town,
                             string county,
                             string postcode,
                             string country,
                             string localAuthorityCode,
                             string localAuthorityName,
                             string govOfficeForRegionName) : base(line1, town, postcode)
        {
            _line2 = line2;
            _line3 = line3;
            _county = county;
            _country = country;
            _localAuthorityCode = localAuthorityCode;
            _localAuthorityName = localAuthorityName;
            _govOfficeForRegionName = govOfficeForRegionName;
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

        #endregion
    }
}