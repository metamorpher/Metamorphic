//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metamorphic.Core.Sensors;
using Metamorphic.Core.Signals;
using NUnit.Framework;

namespace Metamorphic.Server.Signals
{
    [TestFixture]
    public sealed class SignalQueueTest
    {
        [Test]
        public void DequeueWithEmptyQueue()
        {
            var queue = new SignalQueue();

            Assert.IsTrue(queue.IsEmpty);
            Assert.IsNull(queue.Dequeue());
        }

        [Test]
        public void Enqueue()
        {
            var signal1 = new Signal(new SensorId("a"), new Dictionary<string, object>());
            var signal2 = new Signal(new SensorId("b"), new Dictionary<string, object>());
            var queue = new SignalQueue();

            Assert.IsTrue(queue.IsEmpty);
            Assert.IsNull(queue.Dequeue());

            queue.Enqueue(signal1);
            queue.Enqueue(signal2);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreSame(signal1, queue.Dequeue());
            Assert.AreSame(signal2, queue.Dequeue());

            Assert.IsTrue(queue.IsEmpty);
        }

        [Test]
        public void EnqueueWithEmptyQueue()
        {
            var signal = new Signal(new SensorId("a"), new Dictionary<string, object>());
            var queue = new SignalQueue();

            Assert.IsTrue(queue.IsEmpty);

            queue.Enqueue(signal);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreSame(signal, queue.Dequeue());

            Assert.IsTrue(queue.IsEmpty);
        }

        [Test]
        public void EnqueueWithNullObject()
        {
            var queue = new SignalQueue();
            Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null));
        }
    }
}
