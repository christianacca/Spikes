using System;
using Eca.Commons.Extensions;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NValidate.Framework;

namespace Eca.Commons.Data.NHibernate
{
    public static class NHibernateSessionExtensions
    {
        #region Class Members

        private static Func<Exception, Exception> _flushExceptionConvertor;

        /// <summary>
        /// The delegate that will be called whenever <see cref="TransactionalFlush"/> encounters an exception.
        /// This delegate is then given a chance to convert the exception as required
        /// </summary>
        /// <value></value>
        public static Func<Exception, Exception> FlushExceptionConvertor
        {
            set
            {
                _flushExceptionConvertor = value;
                if (value == null) return;

                _flushExceptionConvertor = originalNHibernateException => {
                    try
                    {
                        return value(originalNHibernateException);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            "Problem with the exception convertor registered with NHibernateSessionExtensions.SetExceptionConvertor. See the inner exception for details.",
                            ex);
                    }
                };
            }
        }


        /// <summary>
        /// Reattaches each element in <paramref name="objects"/> with the <paramref name="session"/> without performing
        /// a concurrency control check
        /// </summary>
        public static void Attatch(this ISession session, params object[] objects)
        {
            Attatch(session, false, objects);
        }


        /// <summary>
        /// Reattatch each element in <paramref name="objects"/> with the <paramref name="session"/>
        /// </summary>
        /// <param name="session">session to attatch <paramref name="objects"/></param>
        /// <param name="checkConcurrency">perform a optimistic concurrency control check on each element 
        /// in <paramref name="objects"/></param>
        /// <param name="objects">elements to attatch</param>
        /// <exception cref="StaleObjectStateException">where <paramref name="checkConcurrency"/> is true 
        /// and the state of an element in <paramref name="objects"/> is no longer current</exception>
        public static void Attatch(this ISession session, bool checkConcurrency, params object[] objects)
        {
            Check.Require(() => {
                Demand.The.Param(() => session).IsNotNull();
                Demand.The.Param(() => objects).IsNotNull();
            });

            objects.ForEach(o => session.Lock(o, LockMode.None));
        }


        private static EntityKey EntityKeyFor<T>(ISession session, object id)
        {
            var factoryImpl =
                (ISessionFactoryImplementor) session.SessionFactory;
            IEntityPersister persister = factoryImpl.GetEntityPersister(typeof (T).FullName);
            return new EntityKey(id, persister, EntityMode.Poco);
        }


        public static bool ExistsInSession<T>(this ISession session, object id)
        {
            Check.Require(() => Demand.The.Param(() => session).IsNotNull());

            EntityKey key = EntityKeyFor<T>(session, id);
            return session.GetSessionImplementation().GetEntityUsingInterceptor(key) != null;
        }


        public static void TransactionalFlush(this ISession session)
        {
            ITransaction tx = session.BeginTransaction();

            try
            {
                session.Flush();
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                if (_flushExceptionConvertor == null)
                {
                    throw;
                }

                var possiblyConverted = _flushExceptionConvertor(ex);
                if (ReferenceEquals(possiblyConverted, ex))
                {
                    throw;
                }
                else
                {
                    throw possiblyConverted;
                }
            }
            finally
            {
                tx.Dispose();
            }
        }

        #endregion
    }
}