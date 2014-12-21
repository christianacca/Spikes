namespace Eca.Spikes.NHibernate
{
    public class UserAddress
    {
        #region Member Variables

        private readonly string _county;
        private readonly bool _isNull;
        private readonly string _line1;
        private readonly string _line2;
        private readonly string _postCode;
        private readonly string _town;

        #endregion


        //required by NHibernate


        #region Constructors

        public UserAddress() :
            this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
            _isNull = true;
        }


        public UserAddress(string line1, string line2, string town, string county, string postCode)
        {
            _line1 = line1;
            _line2 = line2;
            _town = town;
            _county = county;
            _postCode = postCode;
        }

        #endregion


        #region Properties

        public string County
        {
            get { return _county; }
        }

        public bool IsNull
        {
            get { return _isNull; }
        }

        public string Line1
        {
            get { return _line1; }
        }

        public string Line2
        {
            get { return _line2; }
        }

        public string PostCode
        {
            get { return _postCode; }
        }

        public string Town
        {
            get { return _town; }
        }

        #endregion


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var userAddress = obj as UserAddress;
            if (userAddress == null) return false;
            if (!Equals(_line1, userAddress._line1)) return false;
            if (!Equals(_line2, userAddress._line2)) return false;
            if (!Equals(_town, userAddress._town)) return false;
            if (!Equals(_county, userAddress._county)) return false;
            if (!Equals(_postCode, userAddress._postCode)) return false;
            if (!Equals(_isNull, userAddress._isNull)) return false;
            return true;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = _line1.GetHashCode();
                result = 29*result + _line2.GetHashCode();
                result = 29*result + _town.GetHashCode();
                result = 29*result + _county.GetHashCode();
                result = 29*result + _postCode.GetHashCode();
                result = 29*result + _isNull.GetHashCode();
                return result;
            }
        }

        #endregion


        #region Class Members

        private static UserAddress nullObject;

        public static UserAddress NullObject
        {
            get
            {
                if (nullObject == null) nullObject = new UserAddress();
                return nullObject;
            }
        }

        #endregion
    }
}