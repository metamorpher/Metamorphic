//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server.Signals
{
    /// <summary>
    /// Defines the interface for classes that queue signals.
    /// </summary>
    internal interface IQueueSignals
    {
        /// <summary>
        /// Removes a signal from the queue for processing.
        /// </summary>
        /// <returns>The signal.</returns>
        Signal Dequeue();

        /// <summary>
        /// Adds the given signal to the queue for processing.
        /// </summary>
        /// <param name="signal">The signal.</param>
        void Enqueue(Signal signal);

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// An event raised when a new signal is enqueued.
        /// </summary>
        event EventHandler OnEnqueue;
    }
}