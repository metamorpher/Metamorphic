using System;
using System.Runtime.Serialization;
using Metamorphic.Core.Properties;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// An exception thrown when the invocation of an action is not possible due to missing parameters.
    /// </summary>
    [Serializable]
    public sealed class MissingActionParameterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingActionParameterException"/> class.
        /// </summary>
        public MissingActionParameterException()
            : this(Resources.Exceptions_Messages_MissingActionParameter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingActionParameterException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingActionParameterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingActionParameterException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingActionParameterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingActionParameterException"/> class.
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
        private MissingActionParameterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}