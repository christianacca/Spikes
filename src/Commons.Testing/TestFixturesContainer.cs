using System;
using System.Collections.Generic;

namespace Eca.Commons.Testing
{
    /// <summary>
    /// Base class for a collection of test fixtures that should be disposed of together (for example in 
    /// the tear down method of a test case class). This class is responsible for maintaining a
    /// reference to each instance of <see cref="ITestFixture"/> created so that they can be easily disposed
    /// by calling <see cref="TestFixturesContainer.Dispose"/>
    /// </summary>
    public class TestFixturesContainer : IDisposable
    {
        #region Constructors

        public TestFixturesContainer()
        {
            AllFixtures = new List<ITestFixture>();
        }

        #endregion


        #region Properties

        public List<ITestFixture> AllFixtures { get; private set; }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            AllFixtures.ForEach(fixture => fixture.Cleanup());
        }

        #endregion


        protected T Create<T>() where T : ITestFixture, new()
        {
            var result = new T();
            AllFixtures.Add(result);
            return result;
        }
    }
}