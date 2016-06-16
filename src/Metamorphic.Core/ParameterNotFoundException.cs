//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Metamorphic.Core.Properties;

namespace Metamorphic.Core
{
    /// <summary>
    /// An exception thrown when the user tries to get a value for a parameter that does not exist.
    /// </summary>
    [Serializable]
    public class ParameterNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNotFoundException"/> class.
        /// </summary>
        public ParameterNotFoundException()
            : this(Resources.Exceptions_Messages_ParameterNotFound)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParameterNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ParameterNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNotFoundException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized
        ///     object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private ParameterNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
