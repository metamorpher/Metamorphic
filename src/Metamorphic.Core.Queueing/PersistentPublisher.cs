//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using EasyNetQ;
using Metamorphic.Core.Queueing.Properties;
using Nuclei;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines a publisher that writes items to a persistent data store for later processing.
    /// </summary>
    /// <typeparam name="TItem">The type of item that should be published.</typeparam>
    /// <typeparam name="TId">The type of the ID instance that is used to identity the current {TItem} instance.</typeparam>
    /// <typeparam name="TDataItem">The type of data object that stores all the data of the {TItem} instance.</typeparam>
    internal abstract class PersistentPublisher<TItem, TId, TDataItem> : IPublishItems<TItem>
        where TItem : class, IHaveIdentity<TId>
        where TId : IIsId<TId>
        where TDataItem : class
    {
        /// <summary>
        /// The bus that is used to send data to RabbitMQ.
        /// </summary>
        private readonly IBus _bus;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The name of the 'store' to which data is published.
        /// </summary>
        private readonly string _storeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentPublisher{TItem, TId, TDataItem}"/> class.
        /// </summary>
        /// <param name="bus">The RabbitMQ bus that is used to send and get items from</param>
        /// <param name="storeName">The name of the store to which data is published.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="bus"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storeName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="storeName"/> is a string that is either empty or only contains whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected PersistentPublisher(IBus bus, string storeName, SystemDiagnostics diagnostics)
        {
            if (bus == null)
            {
                throw new ArgumentNullException("bus");
            }

            if (storeName == null)
            {
                throw new ArgumentNullException("storeName");
            }

            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException(Resources.Exceptions_Messages_ArgumentShouldNotBeAnEmptyString, "storeName");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _bus = bus;
            _storeName = storeName;
            _diagnostics = diagnostics;
        }

        /// <summary>
        /// Publishes an item.
        /// </summary>
        /// <param name="item">The item that should be published.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="item"/> is <see langword="null" />.
        /// </exception>
        public void Publish(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var itemIdentity = item.IdAsText();
            var dataObject = ToDataObject(item);
            _bus.SendAsync(_storeName, dataObject)
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            _diagnostics.Log(
                                LevelToLog.Warn,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_PersistentPublisher_FailedToPublish_WithId,
                                    itemIdentity));
                        }
                        else
                        {
                            _diagnostics.Log(
                                LevelToLog.Debug,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_PersistentPublisher_Published_WithId,
                                    itemIdentity));
                        }
                    });
        }

        /// <summary>
        /// Converts the item into a simple data object that can be serialized.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A data object that can easily be serialized.</returns>
        internal abstract TDataItem ToDataObject(TItem item);
    }
}
