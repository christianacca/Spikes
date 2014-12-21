using System;

namespace Eca.Commons.AddressLookup
{
    public class ImisDefaultAddress : LookupAddress
    {
        #region AddressLayOut enum

        public enum AddressLayOut
        {
            Line1 = 0,
            Line2,
            Line3,
            Town,
            County,
            Postcode
        }

        #endregion


        #region Properties

        public virtual string County { get; set; }
        public virtual string Line2 { get; set; }
        public virtual string Line3 { get; set; }

        #endregion
    }
}