//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Configuration;

namespace Metamorphic.Server
{
    /// <summary>
    /// Defines all the configuration keys.
    /// </summary>
    internal static class ServerConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve path for the directory in 
        /// which the rule files will be placed.
        /// </summary>
        internal static readonly ConfigurationKey RuleDirectory
            = new ConfigurationKey("RulePath", typeof(string));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    RuleDirectory
                };
        }
    }
}
