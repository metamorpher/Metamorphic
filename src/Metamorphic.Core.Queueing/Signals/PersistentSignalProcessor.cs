﻿//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EasyNetQ;
using Metamorphic.Core.Signals;
using Nuclei.Diagnostics;

namespace Metamorphic.Core.Queueing.Signals
{
    /// <summary>
    /// Defines a processor that processes signals.
    /// </summary>
    internal sealed class PersistentSignalProcessor : PersistentProcessor<Signal, SignalData>, IProcessSignals
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentSignalProcessor"/> class.
        /// </summary>
        /// <param name="bus">The RabbitMQ bus that is used to send and get items from</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="bus"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PersistentSignalProcessor(IBus bus, SystemDiagnostics diagnostics) 
            : base(bus, StoreNames.Signal, diagnostics)
        {
        }

        /// <summary>
        /// Converts the data item into an item object.
        /// </summary>
        /// <param name="item">The data item.</param>
        /// <returns>An item object.</returns>
        internal override Signal FromDataItem(SignalData item)
        {
            return new Signal(new SignalTypeId(item.SensorId), new Dictionary<string, object>(item.Parameters));
        }
    }
}
