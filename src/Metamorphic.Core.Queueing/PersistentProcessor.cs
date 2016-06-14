//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using EasyNetQ;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines a processor that processes items from a persistent data store.
    /// </summary>
    /// <typeparam name="TItem">The type of item that should be processed.</typeparam>
    /// <typeparam name="TDataItem">The type of data object that stores all the data of the {TItem} instance.</typeparam>
    internal abstract class PersistentProcessor<TItem, TDataItem> : IProcessItems<TItem>, IDisposable
        where TItem : class
        where TDataItem : class
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Indicates if the current object has been disposed.
        /// </summary>
        private volatile bool m_IsDisposed;

        /// <summary>
        /// The object that references the subscription to the receive event.
        /// </summary>
        private IDisposable m_Subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentProcessor{TItem, TDataItem}"/> class.
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
        protected PersistentProcessor(IBus bus, string storeName, SystemDiagnostics diagnostics)
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
                throw new ArgumentException("The name should not be an empty string", "storeName");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            m_Subscription = bus.Receive(storeName, (Action<TDataItem>)ProcessDataItem);
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                // We've already disposed of everything. Job done.
                return;
            }

            m_IsDisposed = true;
            if (m_Subscription != null)
            {
                try
                {
                    m_Subscription.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Just ignore it. If it is disposed then we don't care anymore
                }

                m_Subscription = null;
            }
        }

        /// <summary>
        /// An event raised when a new item is available in the queue.
        /// </summary>
        public event EventHandler<ItemEventArgs<TItem>> OnEnqueue;

        private void ProcessDataItem(TDataItem dataItem)
        {
            if (dataItem == null)
            {
                return;
            }

            var itemIdentity = dataItem.ToString();
            try
            {
                var item = FromDataItem(dataItem);
                RaiseOnEnqueue(item);

                m_Diagnostics.Log(
                    LevelToLog.Debug,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Processed [{0}].",
                        itemIdentity));
            }
            catch (Exception)
            {
                m_Diagnostics.Log(
                    LevelToLog.Warn,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to process [{0}].",
                        itemIdentity));
                throw;
            }
        }

        private void RaiseOnEnqueue(TItem item)
        {
            var onEnqueue = OnEnqueue;
            if (onEnqueue != null)
            {
                onEnqueue(this, new ItemEventArgs<TItem>(item));
            }
        }

        /// <summary>
        /// Converts the data item into an item object.
        /// </summary>
        /// <param name="item">The data item.</param>
        /// <returns>An item object.</returns>
        internal abstract TItem FromDataItem(TDataItem item);
    }
}
