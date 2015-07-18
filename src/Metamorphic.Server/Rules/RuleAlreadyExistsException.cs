//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Metamorphic.Server.Properties;

namespace Metamorphic.Server.Rules
{
    /// <summary>
    /// An exception thrown when the user tries to add a rule file to the <see cref="RuleCollection"/> more than once.
    /// </summary>
    [Serializable]
    public sealed class RuleAlreadyExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleAlreadyExistsException"/> class.
        /// </summary>
        public RuleAlreadyExistsException()
            : this(Resources.Exceptions_Messages_RuleAlreadyExists)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RuleAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RuleAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleAlreadyExistsException"/> class.
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
        private RuleAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}