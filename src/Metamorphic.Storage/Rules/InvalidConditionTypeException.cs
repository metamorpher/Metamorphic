//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Metamorphic.Storage.Properties;

namespace Metamorphic.Storage.Rules
{
    /// <summary>
    /// An exception thrown when the a rule definition contains an invalid condition type.
    /// </summary>
    [Serializable]
    public sealed class InvalidConditionTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionTypeException"/> class.
        /// </summary>
        public InvalidConditionTypeException()
            : this(Resources.Exceptions_Messages_InvalidConditionType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidConditionTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidConditionTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConditionTypeException"/> class.
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
        private InvalidConditionTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
