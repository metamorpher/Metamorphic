//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Server
{
    /// <summary>
    /// Constants used in the server application.
    /// </summary>
    internal static class ServerConstants
    {
        /// <summary>
        /// The name of the configuration section that describes the configuration of the application.
        /// </summary>
        public const string ConfigurationSectionApplicationSettings = "server";

        /// <summary>
        /// The prefix used for each log message.
        /// </summary>
        public const string LogPrefix = "Metamorphic.Server";

        /// <summary>
        /// The default value for the directory path that contains the rule files.
        /// </summary>
        public const string DefaultRuleDirectory = "rules";
    }
}