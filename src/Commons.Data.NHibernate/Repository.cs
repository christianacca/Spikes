using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eca.Commons.DomainLayer;
using NHibernate;
using NHibernate.Linq;

namespace Eca.Commons.Data.NHibernate
{
    public class Repository<TEntity, TId> : ILinqRepository<TEntity, TId> where TEntity : class
    {
        #region Constructors

        public Repository(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        #endregion


        #region Properties

        public ISession CurrentSession
        {
            get { return SessionFactory.GetCurrentSession(); }
        }

        private ISessionFactory SessionFactory { get; set; }

        #endregion


        #region ILinqRepository<TEntity,TId> Members

        public int Count()
        {
            return CurrentSession.Query<TEntity>().Count();
        }


        public Type ElementType
        {
            get { return CurrentSession.Query<TEntity>().ElementType; }
        }


        public bool Exists(TId id)
        {
            return Count() > 0;
        }


        public Expression Expression
        {
            get { return CurrentSession.Query<TEntity>().Expression; }
        }


        public TEntity GetById(TId id)
        {
            return CurrentSession.Get<TEntity>(id);
        }


        public IEnumerator<TEntity> GetEnumerator()
        {
            return CurrentSession.Query<TEntity>().GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public IQueryProvider Provider
        {
            get { return CurrentSession.Query<TEntity>().Provider; }
        }

        #endregion
    }
}