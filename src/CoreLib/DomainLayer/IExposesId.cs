using System;
using System.Collections.Generic;
using System.Linq;

namespace Eca.Commons.DomainLayer
{
    public interface IExposesId
    {
        Guid Id { get; }
    }



    public static class ExposesIdExtensions
    {
        #region Class Members

        public static IEnumerable<T> Ids<T>(this IEnumerable<IExposesId<T>> source)
        {
            return source.Select(x => x.Id);
        }


        public static bool IsNew(this IExposesId source)
        {
            return source.Id == Guid.Empty;
        }


        public static bool IsNew<T>(this IExposesId<T> source)
        {
            return IsNullOrEmptyId(source.Id);
        }


        /// <remarks>
        /// If <paramref name="id"/> was of type <see cref="string"/> and <paramref name="id"/> was a null,
        /// or an empty string then return true
        /// </remarks>
        public static bool IsNullOrEmptyId<TId>(TId id)
        {
            //one gotcha to be aware of - if id is of type int, then 0 would return true. 
            var isDefaultValue = EqualityComparer<TId>.Default.Equals(id, default(TId));
            if (isDefaultValue) return true;

            return String.IsNullOrEmpty(id.ToString());
        }


        public static T SafeGetId<T>(this IExposesId<T> source)
        {
            if (source == null) return default(T);
            return source.Id;
        }


        public static Guid SafeGetId(this IExposesId source)
        {
            if (source == null) return Guid.Empty;
            return source.Id;
        }

        #endregion
    }



    public interface IExposesId<TId>
    {
        TId Id { get; }
    }
}