//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Metamorphic.Agent.Properties;

namespace Metamorphic.Agent
{
    /// <summary>
    /// An exception thrown when the loading of an action type fails.
    /// </summary>
    [Serializable]
    public sealed class UnableToLoadActionTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLoadActionTypeException"/> class.
        /// </summary>
        public UnableToLoadActionTypeException()
            : this(Resources.Exceptions_Messages_UnableToLoadActionType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLoadActionTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnableToLoadActionTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLoadActionTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnableToLoadActionTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLoadActionTypeException"/> class.
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
        private UnableToLoadActionTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
