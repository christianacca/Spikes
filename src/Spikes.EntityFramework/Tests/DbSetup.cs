using System;
using System.Linq;
using NUnit.Framework;

namespace Spikes.EntityFramework.Tests
{
    [SetUpFixture]
    public class DbSetup
    {
        [SetUp]
        public void Setup()
        {
            // trigger creation of db
            var db = new SpikesDbContext();
            bool ignored = db.Orders.Any();
            Console.Out.WriteLine(ignored);
            db.Dispose();
        }
    }
}