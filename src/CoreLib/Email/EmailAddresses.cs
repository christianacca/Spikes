using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;

namespace Eca.Commons.Email
{
    public class EmailAddresses
    {
        #region Member Variables

        private const string EmailAddressSeperator = ";";


        private ICollection<string> _bccList;
        private ICollection<string> _ccList;
        private ICollection<string> _recipientList;

        #endregion


        #region Constructors

        public EmailAddresses() : this(null) {}
        public EmailAddresses(string from) : this(from, String.Empty) {}
        public EmailAddresses(string from, string to) : this(SplitEmailAddresses(to), from) {}


        public EmailAddresses(IEnumerable<string> recipientList, string from)
        {
            RecipientList =
                recipientList.Where(a => !StringComparer.InvariantCultureIgnoreCase.Equals(a, "IGNORE")).SafeToList();
            CcList = new List<string>();
            BccList = new List<string>();
            From = from;
        }

        #endregion


        #region Properties

        public ICollection<string> BccList
        {
            get { return _bccList; }
            set { _bccList = value.SafeToList(); }
        }

        public ICollection<string> CcList
        {
            get { return _ccList; }
            set { _ccList = value.SafeToList(); }
        }

        public string From { get; set; }

        public ICollection<string> RecipientList
        {
            get { return _recipientList; }
            set { _recipientList = value.SafeToList(); }
        }

        public string ReplyTo { get; set; }

        public string To
        {
            get { return RecipientList.Join(EmailAddressSeperator); }
        }

        #endregion


        public void AddRecipientIfMissing(string emailAddress)
        {
            if (RecipientList.Contains(emailAddress)) return;

            RecipientList.Add(emailAddress);
        }


        #region Class Members

        public static List<string> SplitEmailAddresses(string addresses)
        {
            if (String.IsNullOrEmpty(addresses)) addresses = String.Empty;

            return addresses.Split(new[] {EmailAddressSeperator}, StringSplitOptions.RemoveEmptyEntries)
                .Where(r => r.Trim() != String.Empty).ToList();
        }

        #endregion
    }
}