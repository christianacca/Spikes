using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class LinkedListExamples
    {
        [Test]
        public void SimpleUsageOfLinkedList()
        {
            LinkedList<int> list = new LinkedList<int>();
            list.AddFirst(1);
            LinkedListNode<int> last = list.AddLast(3);
            list.AddBefore(last, 2);

            Assert.That(list.First.Value, Is.EqualTo(1));
            Assert.That(list.Last.Value, Is.EqualTo(3));
            Assert.That(list.Last.Previous.Value, Is.EqualTo(2));
        }


        [Test]
        public void AddFirstWillPushTheExistingNodesDown()
        {
            LinkedList<int> list = new LinkedList<int>();
            list.AddFirst(3);
            list.AddFirst(2);
            list.AddFirst(1);
            Assert.That(list.First.Value, Is.EqualTo(1));
            Assert.That(list.First.Next.Value, Is.EqualTo(2));
            Assert.That(list.Last.Value, Is.EqualTo(3));
        }


        [Test]
        public void ToInsertNodeNeedReferenceToLinkedList()
        {
            LinkedList<int> list = new LinkedList<int>();
            LinkedListNode<int> first = list.AddFirst(1);
            first.List.AddAfter(first, 2);
            Assert.That(first.Value, Is.EqualTo(1));
            Assert.That(first.Next.Value, Is.EqualTo(2));
        }
    }
}