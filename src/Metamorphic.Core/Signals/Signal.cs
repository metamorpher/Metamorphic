//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
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
    public sealed class Signal
    {
        /// <summary>
        /// The collection that stores the parameters for the signal.
        /// </summary>
        private readonly IDictionary<string, string> m_Parameters;

        /// <summary>
        /// The type of signal that has occurred.
        /// </summary>
        private readonly string m_SignalType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Signal"/> class.
        /// </summary>
        /// <param name="signalType">The type of signal.</param>
        /// <param name="parameters">The collection containing the parameters for the signal.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="signalType"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        public Signal(string signalType, IDictionary<string, string> parameters)
        {
            {
                Lokad.Enforce.Argument(() => signalType);
                Lokad.Enforce.Argument(() => signalType, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => parameters);
            }

            m_SignalType = signalType;
            m_Parameters = parameters;
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

            return m_Parameters.ContainsKey(name);
        }

        /// <summary>
        /// Returns the value of the parameter with the given name if it exists.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ParameterNotFoundException">
        ///     Thrown if <paramref name="name" /> does not exist.
        /// </exception>
        public string ParameterValue(string name)
        {
            if (!ContainsParameter(name))
            {
                throw new ParameterNotFoundException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_ParameterNotFound_WithName,
                        name));
            }

            return m_Parameters[name];
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
        /// Gets the type of signal that has occurred.
        /// </summary>
        public string SignalType
        {
            get
            {
                return m_SignalType;
            }
        }
    }
}
