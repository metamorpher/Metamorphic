//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Metamorphic.Core.Properties;
using Metamorphic.Core.Signals;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// An exception thrown when a <see cref="Signal"/> is applied to a <see cref="Rule"/> which does not fit
    /// the signal signature.
    /// </summary>
    [Serializable]
    public class InvalidSignalForRuleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSignalForRuleException"/> class.
        /// </summary>
        public InvalidSignalForRuleException()
            : this(Resources.Exceptions_Messages_InvalidSignalForRule)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSignalForRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidSignalForRuleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSignalForRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidSignalForRuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSignalForRuleException"/> class.
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
        private InvalidSignalForRuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
