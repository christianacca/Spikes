using System;

namespace Eca.Commons.Testing
{
    /// <summary>
    /// Convenient base class to implement the <see cref="ITestFixture"/>
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class EntityTestFixtureBase<T> : ITestFixture
    {
        #region Properties

        /// <summary>
        /// The compare used for the entity <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// A default comparer will be returned unless the subclass overrides this property
        /// to return a comparer configured specifically for the entity <typeparamref name="T"/>.
        /// <para>
        /// The default comparer will compare all properties, unless instructed otherwise by any 
        /// <see cref="EquivalenceComparer.GlobalFilter"/> that may be set.
        /// </para>
        /// </remarks>
        public EquivalenceComparer.Builder<T> Comparer
        {
            get { return EquivalenceComparer.For<T>(); }
        }

        #endregion


        #region ITestFixture Members

        /// <summary>
        /// Subclasses should override this method if there is any cleanup of 
        /// resources that the garbage collected will not do for (ie cleaning up files, or undoing changes in a database)
        /// </summary>
        public virtual void Cleanup() {}

        #endregion
    }
}