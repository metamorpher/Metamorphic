using System;
using Nuclei;

namespace Metamorphic.Core.Signals
{
    /// <summary>
    /// Defines an ID for a trigger.
    /// </summary>
    [Serializable]
    public sealed class SignalTypeId : Id<SignalTypeId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalTypeId"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SignalTypeId(string value) 
            : base(value)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override SignalTypeId Clone(string value)
        {
            return new SignalTypeId(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}
