//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Metamorphic.Core.Properties;
using Nuclei.Configuration;

namespace Metamorphic.Core
{
    /// <summary>
    /// An exception thrown when the a configuration setting is missing.
    /// </summary>
    [Serializable]
    public sealed class MissingConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        public MissingConfigurationException()
            : this(Resources.Exceptions_Messages_MissingConfiguration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key for which the configuration could not be found.</param>
        public MissingConfigurationException(ConfigurationKey configurationKey)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_MissingConfiguration_WithKey, configurationKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConfigurationException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        ///     about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private MissingConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
