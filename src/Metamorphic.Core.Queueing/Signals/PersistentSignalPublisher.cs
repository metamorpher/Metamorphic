//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using EasyNetQ;
using Metamorphic.Core.Signals;
using Nuclei.Diagnostics;

namespace Metamorphic.Core.Queueing.Signals
{
    /// <summary>
    /// Defines a publisher that writes signals to a persistent data store for later processing.
    /// </summary>
    internal sealed class PersistentSignalPublisher : PersistentPublisher<Signal, SignalData>, IPublishSignals
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentSignalPublisher"/> class.
        /// </summary>
        /// <param name="bus">The RabbitMQ bus that is used to send and get items from</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="bus"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PersistentSignalPublisher(IBus bus, SystemDiagnostics diagnostics) 
            : base(bus, StoreNames.Signal, diagnostics)
        {
        }

        /// <summary>
        /// Converts the item into a simple data object that can be serialized.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A data object that can easily be serialized.</returns>
        internal override SignalData ToDataObject(Signal item)
        {
            return ((ITranslateToDataObject<SignalData>)item).ToDataObject();
        }
    }
}
