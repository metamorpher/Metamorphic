//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei;
using NuGet;

namespace Metamorphic.Storage.Rules
{
    /// <summary>
    /// Defines the origin for a rule.
    /// </summary>
    public sealed class RuleOrigin : Id<RuleOrigin, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleOrigin"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Unfortunately we cannot validate this before using it because it's being passed to the base constructor.")]
        public RuleOrigin(PackageName value)
            : base(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}",
                    value.Id,
                    value.Version.ToString()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleOrigin"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private RuleOrigin(string value)
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
        protected override RuleOrigin Clone(string value)
        {
            return new RuleOrigin(value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}
