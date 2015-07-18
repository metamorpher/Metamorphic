//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using NUnit.Framework;

namespace Metamorphic.Server.Jobs
{
    [TestFixture]
    public sealed class JobQueueTest
    {
        [Test]
        public void DequeueWithEmptyQueue()
        {
            var queue = new JobQueue();

            Assert.IsTrue(queue.IsEmpty);
            Assert.IsNull(queue.Dequeue());
        }

        [Test]
        public void Enqueue()
        {
            var job1 = new Job(new ActionId("a"), new Dictionary<string, object>());
            var job2 = new Job(new ActionId("b"), new Dictionary<string, object>());
            var queue = new JobQueue();

            Assert.IsTrue(queue.IsEmpty);
            Assert.IsNull(queue.Dequeue());

            queue.Enqueue(job1);
            queue.Enqueue(job2);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreSame(job1, queue.Dequeue());
            Assert.AreSame(job2, queue.Dequeue());

            Assert.IsTrue(queue.IsEmpty);
        }

        [Test]
        public void EnqueueWithEmptyQueue()
        {
            var job = new Job(new ActionId("a"), new Dictionary<string, object>());
            var queue = new JobQueue();

            Assert.IsTrue(queue.IsEmpty);

            queue.Enqueue(job);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreSame(job, queue.Dequeue());

            Assert.IsTrue(queue.IsEmpty);
        }

        [Test]
        public void EnqueueWithNullObject()
        {
            var queue = new JobQueue();
            Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null));
        }
    }
}
