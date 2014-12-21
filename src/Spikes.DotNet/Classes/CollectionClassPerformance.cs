using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class CollectionClassPerformance
    {
        [Test]
        public void AddingToGenericList()
        {
            int numberOfItems = 1000000;
            int millisecondsForGenericList = TimeToAddToGenericList(numberOfItems).Milliseconds;
            int millisecondsForArrayList = TimeToAddArrayList(numberOfItems).Milliseconds;
            Assert.That(millisecondsForArrayList * 5 > millisecondsForGenericList, Is.True);
        }


        private TimeSpan TimeToAddToGenericList(int numberOfItems) 
        {
            List<int> list = new List<int>(numberOfItems);
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            for (int i = 0; i < numberOfItems; i++)
                list.Add(i);
            watch.Stop();
            return watch.Elapsed;
        }


        private TimeSpan TimeToAddArrayList(int numberOfItems) 
        {
            ArrayList list = new ArrayList();
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            for (int i = 0; i < numberOfItems; i++)
                list.Add(i);
            watch.Stop();
            return watch.Elapsed;
        }
    }
}