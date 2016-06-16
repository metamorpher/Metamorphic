//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines the interface for objects that publish items.
    /// </summary>
    /// <typeparam name="T">The type of object that is published.</typeparam>
    public interface IPublishItems<T>
    {
        /// <summary>
        /// Publishes an item.
        /// </summary>
        /// <param name="item">The item that should be published.</param>
        void Publish(T item);
    }
}
