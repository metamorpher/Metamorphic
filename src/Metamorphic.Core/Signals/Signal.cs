//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Metamorphic.Core.Properties;

namespace Metamorphic.Core.Signals
{
    /// <summary>
    /// Stores information about an event that has occurred.
    /// </summary>
    [Serializable]
    public sealed class Signal : ITranslateToDataObject<SignalData>
    {
        /// <summary>
        /// The collection that stores the parameters for the signal. Note that all parameter names are
        /// stored in lower case so as to provide case-insensitive comparisons between the signal and
        /// rule parameter names.
        /// </summary>
        private readonly IDictionary<string, object> m_Parameters
            = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Signal"/> class.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor that created the current signal.</param>
        /// <param name="parameters">The collection containing the parameters for the signal.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sensorId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        public Signal(SignalTypeId sensorId, IDictionary<string, object> parameters)
        {
            {
                Lokad.Enforce.Argument(() => sensorId);
                Lokad.Enforce.Argument(() => parameters);
            }

            Sensor = sensorId;
            foreach (var pair in parameters)
            {
                m_Parameters.Add(pair.Key.ToLower(), pair.Value);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the signal parameter set contains a parameter
        /// with the given name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>
        ///     <see langword="true" /> if the signal contains a parameter with the given name; otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return m_Parameters.ContainsKey(name.ToLower());
        }

        /// <summary>
        /// Returns the value of the parameter with the given name if it exists.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ParameterNotFoundException">
        ///     Thrown if <paramref name="name" /> does not exist.
        /// </exception>
        public object ParameterValue(string name)
        {
            if (!ContainsParameter(name))
            {
                throw new ParameterNotFoundException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_ParameterNotFound_WithName,
                        name));
            }

            return m_Parameters[name.ToLower()];
        }

        /// <summary>
        /// Returns an enumerator that can be used to enumerate over the 
        /// different parameters for the signal.
        /// </summary>
        /// <returns>The enumerator that can be used to enumerate over the parameters for the signal.</returns>
        public IEnumerable<string> Parameters()
        {
            return m_Parameters.Keys;
        }

        /// <summary>
        /// Gets the ID of the sensor from which the current signal originated.
        /// </summary>
        public SignalTypeId Sensor
        {
            get;
        }

        /// <summary>
        /// Creates a new data object with the data from the current object.
        /// </summary>
        /// <returns>A data object that represents the data on the current object.</returns>
        SignalData ITranslateToDataObject<SignalData>.ToDataObject()
        {
            return new SignalData
                {
                    SensorId = Sensor.ToString(),
                    Parameters = new Dictionary<string, object>(m_Parameters),
                };
        }
    }
}
