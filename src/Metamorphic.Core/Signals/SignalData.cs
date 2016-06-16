//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Metamorphic.Core.Signals
{
    /// <summary>
    /// Stores the data required to create a new <see cref="Signal"/> instance.
    /// </summary>
    public sealed class SignalData : IEquatable<SignalData>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SignalData first, SignalData second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SignalData first, SignalData second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalData"/> class.
        /// </summary>
        public SignalData()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalData"/> class.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor that created the signal</param>
        public SignalData(string sensorId)
            : this(sensorId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalData"/> class.
        /// </summary>
        /// <param name="parameters">The parameters for the signal</param>
        public SignalData(IDictionary<string, object> parameters)
            : this(null, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalData"/> class.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor that created the signal</param>
        /// <param name="parameters">The parameters for the signal</param>
        public SignalData(string sensorId, IDictionary<string, object> parameters)
        {
            SensorId = sensorId;
            if (parameters != null)
            {
                Parameters = new Dictionary<string, object>(parameters);
            }
        }

        /// <summary>
        /// Gets or sets the ID of the sensor that created the signal.
        /// </summary>
        public string SensorId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameters for the signal.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SignalData"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SignalData"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SignalData"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(SignalData other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (!ReferenceEquals(other, null))
            {
                // Use the != operator here on purpose because it should be able to deal with either side
                // being null
                if (SensorId != other.SensorId)
                {
                    return false;
                }

                if ((Parameters == null) && (other.Parameters == null))
                {
                    return true;
                }

                if ((Parameters == null) || (other.Parameters == null))
                {
                    return false;
                }

                foreach (var map in Parameters)
                {
                    if ((!other.Parameters.ContainsKey(map.Key)) || (!map.Value.Equals(other.Parameters[map.Key])))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var id = obj as SignalData;
            return !ReferenceEquals(id, null) && Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public sealed override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                if (SensorId != null)
                {
                    hash = (hash * 23) ^ SensorId.GetHashCode();
                }

                if (Parameters != null)
                {
                    foreach (var map in Parameters)
                    {
                        hash = (hash * 23) ^ map.Value.GetHashCode();
                    }
                }

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string parameters = string.Empty;
            if (Parameters != null)
            {
                parameters = string.Join(",", Parameters.Select(kv => kv.Key.ToString() + "=" + kv.Value.ToString()).ToArray());
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}({1})",
                SensorId,
                parameters);
        }
    }
}
