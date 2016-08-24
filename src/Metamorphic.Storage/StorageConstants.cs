//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Storage
{
    /// <summary>
    /// Constants used in the storage application.
    /// </summary>
    internal static class StorageConstants
    {
        /// <summary>
        /// The name of the configuration section that describes the configuration of the application.
        /// </summary>
        public const string ConfigurationSectionApplicationSettings = "storage";

        /// <summary>
        /// The prefix used for each log message.
        /// </summary>
        public const string LogPrefix = "Metamorphic.Storage";

        /// <summary>
        /// The default value for the directory path that contains the rule files.
        /// </summary>
        public const string DefaultRuleDirectory = "rules";
    }
}
