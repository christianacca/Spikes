using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NValidate.Framework;

namespace Eca.Commons.Reflection
{
    public static class PropertyInfoExtensions
    {
        #region Class Members

        public static IEnumerable<PropertyInfo> PropertiesOfType<T>(this IEnumerable<PropertyInfo> source)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => source).IsNotNull());
#endif

            return source.Where(p => p.PropertyType.IsTypeOf<T>());
        }

        #endregion
    }
}