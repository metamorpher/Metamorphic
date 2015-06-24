using System;
using Nuclei;

namespace Metamorphic.Core.Sensors
{
    /// <summary>
    /// Defines an ID for a trigger.
    /// </summary>
    [Serializable]
    public sealed class SensorId : Id<SensorId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorId"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SensorId(string value) 
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
        protected override SensorId Clone(string value)
        {
            return new SensorId(value);
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
