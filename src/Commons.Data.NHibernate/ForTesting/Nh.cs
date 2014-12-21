using System;
using System.Collections;
using System.Linq;
using Eca.Commons.Reflection;
using NHibernate;

namespace Eca.Commons.Data.NHibernate.ForTesting
{
    public class Nh
    {
        #region Class Members

        public static ISessionFactory SessionFactory;
        private static ISession _currentSession;


        public static ISession CurrentSession
        {
            get { return _currentSession ?? (_currentSession = CreateSession()); }
            set { _currentSession = value; }
        }

        public static bool DoNotClearSession { get; set; }

        public static bool DoNotFlushAndClearSession { get; set; }

        /// <summary>
        /// <para>
        /// A Session that is used by <see cref="CommitInSameTempSession(System.Func{ISession,System.Guid})"/> to perform any
        /// work against the database.
        /// </para>
        /// <para>
        /// <see cref="FlushInThisSession"/> will also asign the session it has been supplied to this property. 
        /// </para>
        /// <para>
        /// This means that if <see cref="FlushInThisSession"/> is called and is supplied a delegate that in turn calls
        /// <see cref="CommitInSameTempSession(System.Func{ISession,System.Guid})"/>, then
        /// <see cref="CommitInSameTempSession(System.Func{ISession,System.Guid})"/> will not start its own session but will
        /// use the session supplied to <see cref="FlushInThisSession"/>. 
        /// </para>
        /// <para>
        /// This is intentional and is meant for advanced scenarios where client code wants to reuse code that makes use of
        /// <see cref="CommitInSameTempSession(System.Func{ISession,System.Guid})"/>, but where that client code does not want
        /// the entities being inserted and/or updated in the database to become detatched instances.
        /// </para>
        /// </summary>
        public static ISession TempSession { get; set; }


        /// <seealso cref="CommitInSameTempSession(System.Func{ISession,System.Guid})"/>
        public static void CommitInSameTempSession(Action<ISession> code)
        {
            CommitInSameTempSession(session => {
                code(session);
                return Guid.Empty;
            });
        }


        /// <summary>
        /// Execute operations against NHibernate using a session that is opened at the start
        /// and flushed and closed at the end.
        /// </summary>
        /// <param name="code">A delegate that will receive the temp session, and will perform one or more operations against this instance</param>
        /// <returns>The value returned by <paramref name="code"/></returns>
        /// <remarks>
        /// <para>
        /// In addition to the session being passed to <paramref name="code"/> as an argument, the session will also
        /// be assigned to the <see cref="TempSession"/> for the duration of the call to this method.
        /// This makes the session available globally, but only for the duration of the call to this method.
        /// </para>
        /// <para>
        /// It may occur that this method is called by code executing within <paramref name="code"/> delegate. 
        /// In this scenario, only the first call to this method will result in a <see cref="ISession"/>
        /// being created. The nested method call will use the existing session already started and assigned to 
        /// <see cref="TempSession"/>. The session created in this scenario will be committed and closed
        /// only at the end of the code executing in the most outer call to this method.
        /// </para>
        /// <para>
        /// Any object that is attached to this temporariy session (<see cref="ISession.Lock(object,LockMode)"/>,
        /// retreived (<see cref="ISession.Get{T}(object)"/>, etc), saved (<see cref="ISession.Save(object)"/>, 
        /// or otherwise added to the temp session (eg <see cref="ISession.Update(object)"/> will become detatched instances.
        /// </para>
        /// </remarks>
        public static Guid CommitInSameTempSession(Func<ISession, Guid> code)
        {
            bool isTempSessionOwner = TempSession == null;

            TempSession = TempSession ?? CreateSession();

            try
            {
                Guid id = code(TempSession);
                if (isTempSessionOwner)
                {
                    TempSession.TransactionalFlush();
                }
                return id;
            }
            finally
            {
                if (isTempSessionOwner)
                {
                    TempSession.Dispose();
                    TempSession = null;
                }
            }
        }


        public static long Count(IQuery query)
        {
            Check.Require(query.QueryString.ToLower().Contains("count(*)"), "Query does not count instances");

            object countMayBe_Int32_Or_Int64_DependingOnDatabase = query.UniqueResult<long>();
            return Convert.ToInt64(countMayBe_Int32_Or_Int64_DependingOnDatabase);
        }


        public static ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }


        public static int DeleteFromDb<T>(Guid id)
        {
            int numberDeleted = 0;
            CommitInSameTempSession(session => {
                string queryString = string.Format("from {0} as e where e.Id= ?", typeof (T).FullName);
                numberDeleted = session.Delete(queryString, id, NHibernateUtil.Guid);
            });

            return numberDeleted;
        }


        public static void DisposeCurrentSession()
        {
            if (_currentSession != null)
            {
                _currentSession.Dispose();
                _currentSession = null;
            }
        }


        public static bool ExistsInDb(params object[] objects)
        {
            return ExistsInDb((IEnumerable) objects);
        }


        public static bool ExistsInDb(IEnumerable objects)
        {
            return objects.Cast<object>().All(o => ExistsInDb(o.GetType(), GetIdOf(o)));
        }


        public static bool ExistsInDb<T>(Guid entityID)
        {
            return ExistsInDb(typeof (T), entityID);
        }


        public static bool ExistsInDb(Type type, Guid entityID)
        {
            string queryString = string.Format("select count(*) from {0} as e where e.Id=:entityId", type.FullName);
            IQuery query = CurrentSession.CreateQuery(queryString).SetGuid("entityId", entityID);
            return Count(query) == 1;
        }


        private static bool ExistsInSession<T>(Guid id)
        {
            return CurrentSession.ExistsInSession<T>(id);
        }


        /// <summary>
        /// Pass the <paramref name="session"/> supplied to <paramref name="code"/> which is then expected to perform one or more operations using this session.
        /// The <paramref name="session"/> will be flushed at the end, but not cleared or closed.
        /// </summary>
        /// <param name="session">The session that will be supplied to <paramref name="code"/>, and that will be flushed at the end</param>
        /// <param name="code">A delegate that will receive the <paramref name="session"/></param>
        /// <remarks>
        /// See <see cref="TempSession"/> to understand how this method interacts and changes the behaviour of 
        /// <see cref="CommitInSameTempSession(System.Action{ISession})"/>
        /// </remarks>
        public static void FlushInThisSession(ISession session, Action<ISession> code)
        {
            bool isTempSessionOwner = TempSession == null;

            TempSession = session;
            try
            {
                code(session);
                session.TransactionalFlush();
            }
            finally
            {
                if (isTempSessionOwner)
                {
                    TempSession = null;
                }
            }
        }


        public static void FlushSessionToDb()
        {
            CurrentSession.TransactionalFlush();
        }


        public static void FlushSessionToDbAndClear()
        {
            CurrentSession.Flush();
            if (DoNotClearSession) return;
            CurrentSession.Clear();
        }


        /// <summary>
        /// Get entity specified by <paramref name="id"/> directly from the database using the <see cref="CurrentSession"/>
        /// </summary>
        /// <remarks>
        /// The entity retrieved from the database will be attached to the <see cref="CurrentSession"/>
        /// </remarks>
        /// <exception cref="InvalidOperationException">where entity is already in the <see cref="CurrentSession"/></exception>
        public static T GetFromDb<T>(Guid id)
        {
            if (ExistsInSession<T>(id))
            {
                const string msg =
                    "Entity requested you want to Get from the database is already in the NHibernate session. To get the entity from the database you must first clear the session";
                throw new InvalidOperationException(msg);
            }

            return CurrentSession.Get<T>(id);
        }


        public static Guid GetIdOf(object obj)
        {
            return obj.GetField<Guid>("_id");
        }


        /// <summary>
        /// Insert the current state of the <paramref name="entity"/> into the database by using a temporary session that is disposed of at the end
        /// </summary>
        /// <returns>The id of the <paramref name="entity"/> that has been inserted</returns>
        /// <remarks>
        /// <para>
        /// Internally <see cref="CommitInSameTempSession(System.Func{ISession,System.Guid})"/> will be used to create and manage 
        /// the <see cref="ISession"/> used to perform the insert.
        /// </para>
        /// <para>
        /// This means that you can have several calls to <see cref="InsertIntoDb(object)"/> use the same temporary <see cref="ISession"/>
        /// by wrapping these multiple calls in a delegate that you supply to <see cref="CommitInSameTempSession(System.Action{ISession})"/>.
        /// </para>
        /// <para>
        /// <strong>Warning:</strong> Once the insert is complete, the <paramref name="entity"/> will become a detatched instance
        /// and so therefore lazy loading for the <paramref name="entity"/> will fail unless it is reattached to another session
        /// </para>
        /// </remarks>
        public static Guid InsertIntoDb(object entity)
        {
            return CommitInSameTempSession(session => {
                session.SaveOrUpdate(entity);
                return GetIdOf(entity);
            });
        }


        /// <summary>
        /// Inserts the current state of <paramref name="entities"/> into the database, using a temporary <see cref="ISession"/> 
        /// that is disposed of at the end.
        /// </summary>
        /// <remarks>
        /// See <see cref="InsertIntoDb(object)"/> for more details about how this will be performed
        /// </remarks>
        public static void InsertIntoDb(IEnumerable entities)
        {
            CommitInSameTempSession(session => {
                foreach (object entity in entities)
                {
                    session.SaveOrUpdate(entity);
                }
            });
        }


        /// <summary>
        /// Updates the database with the current state of <paramref name="entities"/>, using a temporary <see cref="ISession"/> 
        /// that is disposed of at the end.
        /// </summary>
        /// <remarks>
        /// See <see cref="InsertIntoDb(object)"/> for more details about how this will be performed
        /// </remarks>
        public static void UpdateInDb(IEnumerable entities)
        {
            CommitInSameTempSession(session => {
                foreach (object entity in entities)
                {
                    session.SaveOrUpdate(entity);
                }
            });
        }


        /// <summary>
        /// Updates the database with the current state of the <paramref name="entity"/>, using a temporary <see cref="ISession"/>
        /// that is disposed of at the end
        /// </summary>
        /// <remarks>
        /// See <see cref="InsertIntoDb(object)"/> for more details about how this will be performed
        /// </remarks>
        public static void UpdateInDb(object entity)
        {
            CommitInSameTempSession(session => session.Update(entity));
        }

        #endregion
    }
}