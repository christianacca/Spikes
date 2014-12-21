using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Eca.Commons.Data.NHibernate.ForTesting;
using Eca.Commons.DomainLayer;
using Eca.Commons.Reflection;
using NHibernate;
using NUnit.Framework;

namespace Eca.Commons.Testing.NHibernate
{
    public static class NhAssert
    {
        #region Member Variables

        public const string SqlLog = "NHibernate.SQL";

        #endregion


        #region Class Members

        public static PropertyNameFilter.FilterBuilder PropertiesToIgnoreForDbComparison
        {
            get
            {
                return PropertyNameFilter.ToExclude("ConcurrencyId")
                    .ToExclude<IEntityBase>(x => x.Id, x => x.IsNew)
                    .ToExclude<ICanStopValidating>(x => x.IsValidationEnabled)
                    .ToExclude(Auditable.AuditablePropertyNames);
            }
        }


        public static void AssertAreEquivalent<T>(T o1, T o2, EquivalenceComparer comparer) where T : class
        {
            if (o1 == null) Assert.That(o2, Is.Null, "o2 expected to be null");
            if (o2 == null) Assert.That(o1, Is.Null, "o1 expected to be null");
            Assert.That(o1, Is.Not.SameAs(o2), "only makes sense to compare two different object references");
            Assert.That(comparer.PropertiesNotEqual(o1, o2, typeof (T)), Is.Empty);
        }


        public static void IsLazyLoaded(object obj)
        {
            Assert.That(obj, Is.Not.Null, "cannot determine whether object is lazy loaded as it is null!");
            Assert.That(NHibernateUtil.IsInitialized(obj),
                        Is.False,
                        String.Format("Lazy load of {0} expected to be true", obj.GetType().Name));
        }


        public static void SelectsFromDatabaseDoNotExceed(int maximumSelectsAllowed, Action databaseOperation)
        {
            ICollection<string> sqlSelects = Logging.With.Log(SqlLog, databaseOperation);

            Assert.That(sqlSelects.Count,
                        Is.LessThanOrEqualTo(maximumSelectsAllowed),
                        "maximum number of selects exceeded");
        }


        /// <summary>
        /// Use this to verify that the mapping file has mapped an association as mandatory (ie not-null='true')
        /// </summary>
        /// <remarks>
        /// <example>
        /// This verifies that the association from claim to claimant is mandatory ie claim must have
        /// a reference to claimant before it can be persisted:
        /// <c>VerifyInsertThrowsBecausePropertyReferencesUnsavedInstance(claim, x => x.Claimant);</c>
        /// </example>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="PropertyT">the type of the property that is being tested</typeparam>
        /// <param name="objectUnderTest"></param>
        /// <param name="propertyReferencingUnsavedInstance">expression that references the property that is being tested as a mandatory association</param>
        public static void VerifyInsertThrowsBecausePropertyReferencesUnsavedInstance<T, PropertyT>
            (T objectUnderTest, Expression<Func<T, PropertyT>> propertyReferencingUnsavedInstance)
        {
            string expectedMsg = String.Format("not-null property references a null or transient value");
            var ex = Assert.Throws<PropertyValueException>(() => Nh.InsertIntoDb(objectUnderTest));
            Assert.That(ex.Message, Is.StringContaining(expectedMsg));

            string propertyName = ReflectionUtil.GetProperty(propertyReferencingUnsavedInstance).Name;
            Assert.That(ex.Message, Is.StringContaining(propertyName));
        }


        public static void VerifyIsEquivalentToObjectInDatabase<T>(T entity, EquivalenceComparer comparer)
            where T : class
        {
            using (var session = Nh.CreateSession())
            {
                var claimFreshFromDb = session.Get<T>(Nh.GetIdOf(entity));
                AssertAreEquivalent(entity, claimFreshFromDb, comparer);
            }
        }

        #endregion
    }
}