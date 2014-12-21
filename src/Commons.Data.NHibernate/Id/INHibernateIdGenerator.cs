using NHibernate;

namespace Eca.Commons.Data.NHibernate.Id
{
    public interface INHibernateIdGenerator<T>
    {
        /// <summary>
        /// Generates and returns the next id
        /// </summary>
        /// <param name="currentSession"></param>
        T NextId(ISession currentSession);


        /// <summary>
        /// Return the next id without incrementing (ie does not use up the id)
        /// </summary>
        /// <param name="currentSession"></param>
        /// <returns></returns>
        T PeekNextId(ISession currentSession);


        /// <summary>
        /// Save changes to the id's that this generator has generated
        /// </summary>
        /// <param name="currentSession"></param>
        /// <remarks>
        /// Note to inheritor's: This method should be coded so that calls to it are optional.
        /// <p>
        /// The intent of this method is to allow a generator to persist the next id and thereby 
        /// <b>minimise</b> the number of gaps being introduced into the sequence of id's
        /// </p>
        /// However, <see cref="NextId"/> should be implemented to ensure that concurrent 
        /// generator's do not serve up the same id, rather than rely on clients' to call
        /// this method to ensure this.
        /// </remarks>
        void SaveChangesToId(ISession currentSession);


        /// <summary>
        /// Reset the sequence of id's to start at the <paramref name="nextId"/> supplied
        /// </summary>
        void Reseed(T nextId, ISession currentSession);

    }
}