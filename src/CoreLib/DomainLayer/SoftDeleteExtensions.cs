
namespace Eca.Commons.DomainLayer
{
    /// <summary>
    /// <para>
    /// Note: unfortunately these extension methods are useless because of a limitation of EntityFramework not
    /// being able to "see" the property exposed via an interface - EF only works against properties of a concrete class.
    /// </para>
    /// <para>
    /// The problem is componded by the fact that RIA services code generated class does not inherit from a
    /// the same base class as the server-side entity. As such you cannot make the generic type parameter to be
    /// of a concrete class, as this class is not the one inherited from in the client side.
    /// </para>
    /// </summary>
    public static class SoftDeleteExtensions
    {
/*
        #region Class Members

        public static IQueryable<T> Deleted<T>(this IQueryable<T> source) where T : ISoftDelete
        {
            return source.Where(x => x.Deleted);
        }


        public static IEnumerable<T> Deleted<T>(this IEnumerable<T> source) where T : ISoftDelete
        {
            return source.Safe().AsQueryable().Deleted();
        }


        /// <summary>
        /// Conditionally filter <paramref name="source"/> to include/exclude soft deleted items
        /// </summary>
        public static IQueryable<T> ExcludeSoftDeleted<T>(this IQueryable<T> source, bool excludeSoftDeleted)
            where T : ISoftDelete
        {
            return excludeSoftDeleted ? source.NotDeleted() : source;
        }


        /// <summary>
        /// Conditionally filter <paramref name="source"/> to include/exclude soft deleted items
        /// </summary>
        public static IEnumerable<T> ExcludeSoftDeleted<T>(this IEnumerable<T> source, bool excludeSoftDeleted)
            where T : ISoftDelete
        {
            return excludeSoftDeleted ? source.NotDeleted() : source.Safe();
        }


        public static IQueryable<T> NotDeleted<T>(this IQueryable<T> source) where T : ISoftDelete
        {
            return source.Where(x => !x.Deleted);
        }


        public static IEnumerable<T> NotDeleted<T>(this IEnumerable<T> source) where T : ISoftDelete
        {
            return source.Safe().AsQueryable().NotDeleted();
        }

        #endregion
*/
    }
}