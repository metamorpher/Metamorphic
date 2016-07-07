//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Metamorphic.Core.Properties;
using Nuclei;

namespace Metamorphic.Core.Signals
{
    /// <summary>
    /// Stores information about an event that has occurred.
    /// </summary>
    [Serializable]
    public sealed class Signal : ITranslateToDataObject<SignalData>, IHaveIdentity<SignalTypeId>
    {
        /// <summary>
        /// The collection that stores the parameters for the signal. Note that all parameter names are
        /// stored in lower case so as to provide case-insensitive comparisons between the signal and
        /// rule parameter names.
        /// </summary>
        private readonly IDictionary<string, object> _parameters
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
            if (sensorId == null)
            {
                throw new ArgumentNullException("sensorId");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            Sensor = sensorId;
            foreach (var pair in parameters)
            {
                _parameters.Add(pair.Key.ToUpper(CultureInfo.InvariantCulture), pair.Value);
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

            return _parameters.ContainsKey(name.ToUpper(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the ID for the current instance.
        /// </summary>
        public IIsId<SignalTypeId> Id
        {
            get
            {
                return Sensor;
            }
        }

        /// <summary>
        /// Returns the ID of the current instance as a human readable string.
        /// </summary>
        /// <returns>The ID as a human readable string.</returns>
        public string IdAsText()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Signal[{0}]",
                Sensor);
        }

        /// <summary>
        /// Returns the value of the parameter with the given name if it exists.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ParameterNotFoundException">
        ///     Thrown if <paramref name="name" /> does not exist.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "The 'name' parameter is validated through the ContainsParameter method.")]
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

            return _parameters[name.ToUpper(CultureInfo.InvariantCulture)];
        }

        /// <summary>
        /// Returns an enumerator that can be used to enumerate over the
        /// different parameters for the signal.
        /// </summary>
        /// <returns>The enumerator that can be used to enumerate over the parameters for the signal.</returns>
        public IEnumerable<string> Parameters()
        {
            return _parameters.Keys;
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
                Parameters = new Dictionary<string, object>(_parameters),
            };
        }
    }
}
