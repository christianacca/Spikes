using System;
using System.Web;
using NHibernate;
using NHibernate.Context;

namespace Eca.Commons.Data.NHibernate
{
    public static class SessionFactoryExtensions
    {
        #region Class Members

        private static bool IsAspNetProcess
        {
            get { return HttpContext.Current != null; }
        }


        /// <summary>
        /// Disposes of the current session returned by <see cref="ISessionFactory.GetCurrentSession"/> and unbinds it
        /// from <see cref="CurrentSessionContext"/>. If the current session has an active transaction, this will be rolled back.
        /// </summary>
        /// <param name="sessionFactory">The session factory that will return the current session</param>
        public static void DisposeCurrentSession(this ISessionFactory sessionFactory)
        {
            ISession currentSession = sessionFactory.GetCurrentSession();
            //this will rollback any active NHibernate transaction
            currentSession.Dispose();
            CurrentSessionContext.Unbind(sessionFactory);
        }


        /// <summary>
        /// Opens a new <see cref="ISession"/> and binds it to the <see cref="CurrentSessionContext"/>. This new session 
        /// is then available by calling <see cref="ISessionFactory.GetCurrentSession"/>.
        /// <para>
        /// <strong>Important:</strong> changes to entities tracked by this session will only be written to the 
        /// database by calling <see cref="ISession.Flush"/>, or better still by calling 
        /// <see cref="NHibernateSessionExtensions.TransactionalFlush"/>.
        /// </para>
        /// </summary>
        /// <param name="sessionFactory">The session factory that will return the current session</param>
        /// <remarks>
        /// The newly created session is configured with a <see cref="ISession.FlushMode"/> of <see cref="FlushMode.Never"/>.
        /// This is intentionally opinionated. 
        /// <para>
        /// The opinion is that writing to the database should be initiated explicitly by the developer and not 
        /// left to happen in the HttpApplication.EndRequest or in an MVC ActionExecutedFilter which is usually the 
        /// recommended practice when implementing the session-per-request pattern. 
        /// </para>
        /// <para>
        /// When the developer is left to initiate the write to the database, he can then take control of how to 
        /// respond to exceptions. For example, to try and recover from a <see cref="StaleObjectStateException"/>, 
        /// to map the exception to view model properties so that they are displayed conveniently to the end user, 
        /// to redirect to another path in the application, etc.
        /// </para>
        /// <para>
        /// If it is detected that the main executable program is running within the ASP.Net host, a  
        /// <see cref="ITransaction"/> will be started. This is to ensure that by default any subsequent 
        /// communication within the database is performed within a transaction - an important NHibernate best practice. 
        /// </para>
        /// </remarks>
        public static void StartCurrentSession(this ISessionFactory sessionFactory)
        {
            ISession newSession = sessionFactory.OpenSession();
            newSession.FlushMode = FlushMode.Never;
            CurrentSessionContext.Bind(newSession);
            if (IsAspNetProcess) newSession.BeginTransaction();
        }


        /// <summary>
        /// Call <see cref="NHibernateSessionExtensions.TransactionalFlush"/> on the session returned by
        /// <see cref="ISessionFactory.GetCurrentSession"/>
        /// </summary>
        /// <param name="sessionFactory">The session factory that will return the current session</param>
        /// <param name="openNewSessionOnFailure">
        /// When true, <see cref="StartCurrentSession"/> will be called to create a new session when an exception is thrown
        /// </param>
        /// <remarks>
        /// <para>
        /// After a successfull flush, if it is detected that the main executable program is running within the ASP.Net host,
        /// a new <see cref="ITransaction"/> will be started.  This is to ensure that by any subsequent communication within 
        /// the database is performed within a transaction - an important NHibernate best practice. 
        /// </para>
        /// <para>
        /// However, a new transaction will not be opened if a an exception is thrown and <paramref name="openNewSessionOnFailure"/> 
        /// was set to false.
        /// </para>
        /// </remarks>
        public static void TransactionalFlushCurrentSession(this ISessionFactory sessionFactory,
                                                            bool openNewSessionOnFailure)
        {
            try
            {
                sessionFactory.GetCurrentSession().TransactionalFlush();
            }
            catch (Exception)
            {
                sessionFactory.DisposeCurrentSession();
                if (openNewSessionOnFailure)
                {
                    sessionFactory.StartCurrentSession();
                }
                throw;
            }
            if (IsAspNetProcess) sessionFactory.GetCurrentSession().BeginTransaction();
        }

        #endregion
    }
}