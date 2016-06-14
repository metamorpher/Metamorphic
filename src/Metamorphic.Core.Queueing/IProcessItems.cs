//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines the interface for objects that implement a persistent queue.
    /// </summary>
    /// <typeparam name="T">The type of object that is stored in the queue.</typeparam>
    public interface IProcessItems<T>
    {
        /// <summary>
        /// An event raised when a new item is available in the queue.
        /// </summary>
        event EventHandler<ItemEventArgs<T>> OnEnqueue;
    }
}
