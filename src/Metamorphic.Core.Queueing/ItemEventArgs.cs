//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores an item.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public sealed class ItemEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventArgs{T}"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ItemEventArgs(T item)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item
        {
            get;
        }
    }
}