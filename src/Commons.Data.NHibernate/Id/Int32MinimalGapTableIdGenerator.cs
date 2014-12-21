using System;
using NHibernate;
using NHibernate.Engine;

namespace Eca.Commons.Data.NHibernate.Id
{
    public class Int32MinimalGapTableIdGenerator : MinimalGapTableIdGenerator, INHibernateIdGenerator<int>
    {
        #region Properties

        #endregion


        #region IIdGenerator<int> Members

        public int NextId(ISession currentSession)
        {
            return NextId((ISessionImplementor) currentSession);
        }


        public int NextId(ISessionImplementor session)
        {
            object id = Generate(session, null);
            return Convert.ToInt32(id);
        }


        public int PeekNextId(ISession currentSession)
        {
            long id = PeekNextId((ISessionImplementor) currentSession);
            return Convert.ToInt32(id);
        }


        public void Reseed(int nextId, ISession currentSession)
        {
            Reseed(nextId, (ISessionImplementor) currentSession);
        }


        public void SaveChangesToId(ISession currentSession)
        {
            SaveChangesToId((ISessionImplementor) currentSession);
        }

        #endregion


        /// <summary>
        /// Method exposed only only for testing
        /// </summary>
        /// <returns></returns>
        protected internal int GetNextIdStoredInDatabase(ISession currentSession)
        {
            long id = GetNextIdStoredInDatabase((ISessionImplementor) currentSession);
            return Convert.ToInt32(id);
        }
    }
}