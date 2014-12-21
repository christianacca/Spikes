using NHibernate.Criterion;

namespace Eca.Commons.Data.NHibernate
{
    public static class DetatchedCriteriaExtensions
    {
        #region Class Members

        public static DetachedCriteria AddEq(this DetachedCriteria source, string propertyName, object value)
        {
            return source.Add(Expression.Eq(propertyName, value));
        }

        #endregion
    }
}