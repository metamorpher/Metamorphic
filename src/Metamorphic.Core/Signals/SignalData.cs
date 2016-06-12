//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Metamorphic.Core.Signals
{
    /// <summary>
    /// Stores the data required to create a new <see cref="Signal"/> instance.
    /// </summary>
    public sealed class SignalData
    {
        /// <summary>
        /// Gets or sets the ID of the sensor that created the signal.
        /// </summary>
        public string SensorId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameters for the signal.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }
}