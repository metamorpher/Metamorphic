//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using Metamorphic.Core.Jobs;

namespace Metamorphic.Server.Jobs
{
    /// <summary>
    /// Provides a thread-safe queue of jobs.
    /// </summary>
    internal sealed class JobQueue : IQueueJobs
    {
        /// <summary>
        /// Stores the full path to the queued jobs.
        /// </summary>
        private readonly ConcurrentQueue<Job> _queue
            = new ConcurrentQueue<Job>();

        /// <summary>
        /// Removes a job from the queue for processing.
        /// </summary>
        /// <returns>The job if it exists, otherwise null.</returns>
        public Job Dequeue()
        {
            Job signal;
            if (_queue.TryDequeue(out signal))
            {
                return signal;
            }

            return null;
        }

        /// <summary>
        /// Adds the given job to the queue for processing.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="job"/> is <see langword="null" />.
        /// </exception>
        public void Enqueue(Job job)
        {
            {
                Lokad.Enforce.Argument(() => job);
            }

            _queue.Enqueue(job);
            RaiseOnEnqueue();
        }

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _queue.IsEmpty;
            }
        }

        /// <summary>
        /// An event raised when a new job is enqueued.
        /// </summary>
        public event EventHandler OnEnqueue;

        private void RaiseOnEnqueue()
        {
            var local = OnEnqueue;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
