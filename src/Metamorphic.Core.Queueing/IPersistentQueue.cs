//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines the interface for objects that implement a persistent queue.
    /// </summary>
    /// <typeparam name="T">The type of object that is stored in the queue.</typeparam>
    public interface IPersistentQueue<T> where T : ISerializable
    {
        /// <summary>
        /// Removes an item from the queue for processing.
        /// </summary>
        /// <returns>The item.</returns>
        T Dequeue();

        /// <summary>
        /// Adds the given item to the queue for processing.
        /// </summary>
        /// <param name="signal">The item.</param>
        void Enqueue(T item);

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// An event raised when a new item is enqueued.
        /// </summary>
        event EventHandler OnEnqueue;
    }
}