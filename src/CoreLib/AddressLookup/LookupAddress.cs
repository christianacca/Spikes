using System;

namespace Eca.Commons.AddressLookup
{
    public abstract class LookupAddress
    {
        #region Member Variables

        private string _line1;
        private string _moniker;
        private string _postcode;
        private int _score;
        private string _singleLine;
        private string _town;

        #endregion


        #region Constructors

        protected LookupAddress() {}


        protected LookupAddress(string line1, string town, string postcode)
        {
            _line1 = line1;
            _town = town;
            _postcode = postcode;
        }


        /// <summary>
        /// Creates this object adding a Flat QAS address line and its moniker, with 0 as the score
        /// </summary>
        /// <param name="singleLine">QAS flat address line</param>
        /// <param name="moniker">The moniker associated with the flat address line</param>
        protected LookupAddress(string singleLine, string moniker)
            : this(singleLine, moniker, 0) {}


        /// <summary>
        /// Creates this object adding a Flat QAS address line, its moniker and score
        /// </summary>
        /// <param name="singleLine">QAS flat address line</param>
        /// <param name="moniker">The moniker associated with the flat address line</param>
        /// <param name="score">The score of the address line</param>
        protected LookupAddress(string singleLine, string moniker, int score)
        {
            _singleLine = singleLine;
            _moniker = moniker;
            _score = score;
        }

        #endregion


        #region Properties

        public virtual string Line1
        {
            get { return _line1; }
            set { _line1 = value; }
        }

        public virtual string Moniker
        {
            get { return _moniker; }
            set { _moniker = value; }
        }

        public virtual string Postcode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }

        public virtual int Score
        {
            get { return _score; }
            set { _score = value; }
        }


        public virtual string SingleLine
        {
            get { return _singleLine; }
            set { _singleLine = value; }
        }

        public virtual string Town
        {
            get { return _town; }
            set { _town = value; }
        }

        #endregion
    }
}