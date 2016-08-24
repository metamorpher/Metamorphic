//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines the configuration keys for the rule part of the application.
    /// </summary>
    public static class RuleConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the paths in which
        /// single rule files are placed.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey RuleLocations
            = new ConfigurationKey("RuleLocations", typeof(string[]));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    RuleLocations,
                };
        }
    }
}
