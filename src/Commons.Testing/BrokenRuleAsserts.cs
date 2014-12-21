using System;
using System.Linq;
using System.Linq.Expressions;
using Eca.Commons.Reflection;
using Eca.Commons.Validation;
using NUnit.Framework;

namespace Eca.Commons.Testing
{
    public static class BrokenRuleAsserts
    {
        #region Class Members

        public static void DoesNotHaveBrokenProperties<T>(BrokenRules actualBrokenRules,
                                                          params Expression<Func<T, object>>[]
                                                              expectedBadProperties)
        {
            foreach (var expectedBadProperty in expectedBadProperties)
            {
                string propertyName = ReflectionUtil.GetPropertyNames(expectedBadProperty);
                bool found = actualBrokenRules.Any(failure => failure.PropertyName == propertyName);
                Assert.That(found,
                            Is.False,
                            String.Format("Expected not to find an error for property {0}", propertyName));
            }
        }


        public static void HasBrokenProperties<T>(BrokenRules actualBrokenRules,
                                                  params Expression<Func<T, object>>[] expectedBadProperties)
        {
            foreach (var expectedBadProperty in expectedBadProperties)
            {
                string propertyName = ReflectionUtil.GetPropertyNames(expectedBadProperty);
                bool found = actualBrokenRules.Any(failure => failure.PropertyName == propertyName);
                Assert.That(found, Is.True, String.Format("Expected to find an error for property {0}", propertyName));
            }
        }

        #endregion
    }
}