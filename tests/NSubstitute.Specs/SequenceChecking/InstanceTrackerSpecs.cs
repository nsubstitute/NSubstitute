using System;
using System.Collections.Generic;
using NSubstitute.Core.SequenceChecking;
using NUnit.Framework;

namespace NSubstitute.Specs.SequenceChecking
{
    public class InstanceTrackerSpecs
    {
        private InstanceTracker _instanceTracker;

        [SetUp]
        public void SetUp()
        {
            _instanceTracker = new InstanceTracker();
        }

        [Test]
        public void NumberInstancesInOrderSeen()
        {
            var first = "first";
            var second = new object();
            var third = new List<int>();

            Assert.That(_instanceTracker.InstanceNumber(first), Is.EqualTo(1));
            Assert.That(_instanceTracker.InstanceNumber(second), Is.EqualTo(2));
            Assert.That(_instanceTracker.InstanceNumber(third), Is.EqualTo(3));
            Assert.That(_instanceTracker.NumberOfInstances(), Is.EqualTo(3));
        }

        [Test]
        public void SameInstanceNumberReturnedForSameInstance()
        {
            var first = "first";
            var second = new object();
            var third = new List<int>();

            _instanceTracker.InstanceNumber(first);
            _instanceTracker.InstanceNumber(second);
            _instanceTracker.InstanceNumber(third);

            var numForExistingInstance = _instanceTracker.InstanceNumber(second);
            Assert.That(numForExistingInstance, Is.EqualTo(2));
            Assert.That(_instanceTracker.NumberOfInstances(), Is.EqualTo(3));
        }

        [Test]
        public void WorkForTypesThatMuckAroundWithEquality()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();

            Assert.That(_instanceTracker.InstanceNumber(foo1), Is.EqualTo(1));
            Assert.That(_instanceTracker.InstanceNumber(foo2), Is.EqualTo(2));
        }

        private class Foo
        {
            public override int GetHashCode() { return 0; }
            public override bool Equals(object obj) { return true; }
        }
    }
}