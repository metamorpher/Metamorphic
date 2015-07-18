//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Metamorphic.Core.Actions;
using Metamorphic.Server.Properties;

namespace Metamorphic.Server.Actions
{
    /// <summary>
    /// An exception thrown when the user tries to add an <see cref="ActionDefinition"/> to a collection
    /// that already has one with an identical <see cref="ActionId"/>.
    /// </summary>
    [Serializable]
    public sealed class DuplicateActionDefinitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateActionDefinitionException"/> class.
        /// </summary>
        public DuplicateActionDefinitionException()
            : this(Resources.Exceptions_Messages_DuplicateActionDefinition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateActionDefinitionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DuplicateActionDefinitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateActionDefinitionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateActionDefinitionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateActionDefinitionException"/> class.
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
        private DuplicateActionDefinitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}