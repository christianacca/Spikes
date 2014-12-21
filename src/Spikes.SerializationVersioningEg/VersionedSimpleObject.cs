using System;

namespace Eca.Spikes.SerializationVersioningEg
{
    [Serializable]
    public class VersionedSimpleObject
    {
        private string _firstName;
        private string _lastName;


        public VersionedSimpleObject(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
        }


        public string LastName
        {
            get { return _lastName; }
        }

        public string FirstName
        {
            get { return _firstName; }
        }
    }
}