namespace Eca.Spikes.NHibernate
{
    public class CustomerRepresentative : EntityBase
    {
        #region Member Variables

        private string _firstName;

        private string _lastName;

        #endregion


        #region Properties

        public virtual string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public virtual string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        #endregion
    }
}