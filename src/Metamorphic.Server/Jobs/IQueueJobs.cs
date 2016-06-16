//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core.Jobs;

namespace Metamorphic.Server.Jobs
{
    /// <summary>
    /// Defines the interface for classes that queue jobs.
    /// </summary>
    internal interface IQueueJobs
    {
        /// <summary>
        /// Removes a job from the queue for processing.
        /// </summary>
        /// <returns>The job.</returns>
        Job Dequeue();

        /// <summary>
        /// Adds the given job to the queue for processing.
        /// </summary>
        /// <param name="job">The job.</param>
        void Enqueue(Job job);

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// An event raised when a new job is enqueued.
        /// </summary>
        event EventHandler OnEnqueue;
    }
}
