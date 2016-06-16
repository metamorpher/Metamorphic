//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
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
