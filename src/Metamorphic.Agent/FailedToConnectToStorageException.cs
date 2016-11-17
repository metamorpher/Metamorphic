//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Metamorphic.Agent
{
    /// <summary>
    /// An exception thrown when the application cannot connect to the storage application.
    /// </summary>
    [Serializable]
    public sealed class FailedToConnectToStorageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToConnectToStorageException"/> class.
        /// </summary>
        public FailedToConnectToStorageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToConnectToStorageException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FailedToConnectToStorageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToConnectToStorageException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FailedToConnectToStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToConnectToStorageException"/> class.
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
        private FailedToConnectToStorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
