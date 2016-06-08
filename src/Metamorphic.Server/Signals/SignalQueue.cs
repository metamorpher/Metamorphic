//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server.Signals
{
    /// <summary>
    /// Provides a thread-safe queue of signals.
    /// </summary>
    internal sealed class SignalQueue : IQueueSignals
    {
        /// <summary>
        /// Stores the full path to the queued signals.
        /// </summary>
        private readonly ConcurrentQueue<Signal> m_Queue
            = new ConcurrentQueue<Signal>();

        /// <summary>
        /// Removes a signal from the queue for processing.
        /// </summary>
        /// <returns>The signal if it exists, otherwise null.</returns>
        public Signal Dequeue()
        {
            Signal signal;
            if (m_Queue.TryDequeue(out signal))
            {
                return signal;
            }

            return null;
        }

        /// <summary>
        /// Adds the given signal to the queue for processing.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="signal"/> is <see langword="null" />.
        /// </exception>
        public void Enqueue(Signal signal)
        {
            {
                Lokad.Enforce.Argument(() => signal);
            }

            m_Queue.Enqueue(signal);
            RaiseOnEnqueue();
        }

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return m_Queue.IsEmpty;
            }
        }

        /// <summary>
        /// An event raised when a new signal is enqueued.
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
