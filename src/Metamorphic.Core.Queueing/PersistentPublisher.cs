//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using EasyNetQ;
using Metamorphic.Core.Queueing.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines a publisher that writes items to a persistent data store for later processing.
    /// </summary>
    /// <typeparam name="TItem">The type of item that should be published.</typeparam>
    /// <typeparam name="TDataItem">The type of data object that stores all the data of the {TItem} instance.</typeparam>
    internal abstract class PersistentPublisher<TItem, TDataItem> : IPublishItems<TItem> 
        where TItem : class
        where TDataItem : class
    {
        /// <summary>
        /// The bus that is used to send data to RabbitMQ.
        /// </summary>
        private readonly IBus m_Bus;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The name of the 'store' to which data is published.
        /// </summary>
        private readonly string m_StoreName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentPublisher{TItem, TDataItem}"/> class.
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

            m_Bus = bus;
            m_StoreName = storeName;
            m_Diagnostics = diagnostics;
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

            var itemIdentity = item.ToString();
            var dataObject = ToDataObject(item);
            m_Bus.SendAsync(m_StoreName, dataObject)
                .ContinueWith(
                    t => 
                    {
                        if (t.IsFaulted)
                        {
                            m_Diagnostics.Log(
                                LevelToLog.Warn,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_PersistentPublisher_FailedToPublish_WithId,
                                    itemIdentity));
                        }
                        else
                        {
                            m_Diagnostics.Log(
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
