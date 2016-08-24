//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core;
using Nuclei.Configuration;

namespace Metamorphic.Actions.Powershell
{
    /// <summary>
    /// Defines the configuration keys for the powershell action plugin.
    /// </summary>
    public sealed class PowershellConfigurationKeys : IProvideConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve path for the directory in
        /// which the rule files will be placed.
        /// </summary>
        internal static readonly ConfigurationKey ScriptDirectory
            = new ConfigurationKey("ScriptPath", typeof(string));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    ScriptDirectory,
                };
        }
    }
}
