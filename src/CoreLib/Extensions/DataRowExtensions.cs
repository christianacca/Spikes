using System;
using System.Data;

namespace Eca.Commons.Extensions
{
    public static class DataRowExtensions
    {
        #region Class Members

        /// <summary>
        /// Convenient wrapper around <see cref="System.Data.DataRowExtensions.Field{T}(DataRow,DataColumn)"/> 
        /// to allow supplying an Enum to define the column index
        /// </summary>
        public static T Field<T>(this DataRow source, Enum columnIndex)
        {
            var column = (int) Convert.ChangeType(columnIndex, typeof (int));
            return source.Field<T>(column);
        }


        /// <summary>
        /// Convenient wrapper around <see cref="System.Data.DataRowExtensions.Field{T}(DataRow,DataColumn)"/> 
        /// to allow supplying an Enum to define the column index and a default value (<paramref name="valueIfNull"/>)
        /// for when the column value returns a <see cref="DBNull"/>
        /// </summary>
        /// <remarks>
        /// This method uses the <see cref="EnhancedConvertor"/> to provide better support for casting between
        /// the underlying field value and the desired type <typeparamref name="T"/>
        /// </remarks>
        public static T Field<T>(this DataRow source, Enum columnIndex, T valueIfNull)
        {
            var column = (int) Convert.ChangeType(columnIndex, typeof (int));
            return (T) (Convert.IsDBNull(source[column])
                            ? valueIfNull
                            : EnhancedConvertor.ChangeType(source[column], typeof (T)));
        }


        /// <summary>
        /// See <see cref="Field{T}(DataRow,Enum,T)"/>
        /// </summary>
        /// <returns>The underlying field value if not null, otherwise <c>default(T)</c></returns>
        public static T SafeField<T>(this DataRow source, Enum columnIndex)
        {
            return source.Field(columnIndex, default(T));
        }

        #endregion
    }
}