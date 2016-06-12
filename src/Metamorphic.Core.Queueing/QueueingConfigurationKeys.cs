//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Metamorphic.Core.Queueing
{
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
            = new ConfigurationKey("RabbitMQHosts", typeof(string[]));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the default port for the RabbitMQ instances.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey RabbitMqDefaultPort
            = new ConfigurationKey("RabbitMQDefaultPort", typeof(ushort));

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
            = new ConfigurationKey("RabbitMQVirtualHostName", typeof(string));
    }
}
