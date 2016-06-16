//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for queueing of signals, jobs and actions.
    /// </summary>
    public static class QueueingConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve time between keep-alive heart beats in seconds
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqHeartbeatInSeconds
            = new ConfigurationKey("RabbitMqHeartbeatInSeconds", typeof(ushort));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the list of hosts for the RabbitMQ instances.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqHosts
            = new ConfigurationKey("RabbitMqHosts", typeof(string[]));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the default port for the RabbitMQ instances.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqDefaultPort
            = new ConfigurationKey("RabbitMqDefaultPort", typeof(ushort));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the user name used to connect to the RabbitMQ instances.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqUserName
            = new ConfigurationKey("RabbitMqUserName", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the password used to connect to the RabbitMQ instances.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqUserPassword
            = new ConfigurationKey("RabbitMqUserPassword", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the virtual host name for the RabbitMQ instances.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqVirtualHostName
            = new ConfigurationKey("RabbitMqVirtualHostName", typeof(string));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    RabbitMqHeartbeatInSeconds,
                    RabbitMqHosts,
                    RabbitMqDefaultPort,
                    RabbitMqUserName,
                    RabbitMqUserPassword,
                    RabbitMqVirtualHostName,
                };
        }
    }
}
