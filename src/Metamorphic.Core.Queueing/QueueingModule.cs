//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using EasyNetQ;
using Metamorphic.Core.Queueing.Signals;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Defines the dependency injection module for the Queueing area.
    /// </summary>
    public sealed class QueueingModule : Module
    {
        /// <summary>
        /// The default port on which RabbitMQ instances are expected to be found.
        /// </summary>
        private const ushort DefaultPort = 5672;

        /// <summary>
        /// The default heart beat time in seconds, used to keep the connection with the RabbitMQ instance open.
        /// </summary>
        private const ushort DefaultRequestedHeartbeatTimeInSeconds = 10;

        /// <summary>
        /// The default virtual host name for the RabbitMQ instances.
        /// </summary>
        private const string DefaultVirtualHostName = "/";

        private static ConnectionConfiguration RabbitMQConnectionFromConfiguration(IConfiguration configuration)
        {
            if (!configuration.HasValueFor(QueueingConfigurationKeys.RabbitMQUserName))
            {
                throw new MissingConfigurationException(QueueingConfigurationKeys.RabbitMQUserName);
            }

            if (!configuration.HasValueFor(QueueingConfigurationKeys.RabbitMQUserPassword))
            {
                throw new MissingConfigurationException(QueueingConfigurationKeys.RabbitMQUserPassword);
            }

            var hosts = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMQHosts)
                ? configuration.Value<string[]>(QueueingConfigurationKeys.RabbitMQHosts)
                    .Select(
                        s =>
                        {
                            var uri = new Uri(s);
                            return new HostConfiguration
                            {
                                Host = uri.DnsSafeHost,
                                Port = (ushort)uri.Port
                            };
                        })
                    .ToList()
                : new List<HostConfiguration>
                    {
                        new HostConfiguration
                        {
                            Host = "localhost",
                            Port = DefaultPort
                        }
                    };

            return new ConnectionConfiguration
            {
                VirtualHost = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMQVirtualHostName)
                        ? configuration.Value<string>(QueueingConfigurationKeys.RabbitMQVirtualHostName)
                        : DefaultVirtualHostName,
                Port = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMQDefaultPort)
                        ? configuration.Value<ushort>(QueueingConfigurationKeys.RabbitMQDefaultPort)
                        : DefaultPort,

                Hosts = hosts,

                UserName = configuration.Value<string>(QueueingConfigurationKeys.RabbitMQUserName),
                Password = configuration.Value<string>(QueueingConfigurationKeys.RabbitMQUserPassword),

                RequestedHeartbeat = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMQHeartbeatInSeconds)
                        ? configuration.Value<ushort>(QueueingConfigurationKeys.RabbitMQHeartbeatInSeconds)
                        : DefaultRequestedHeartbeatTimeInSeconds,

                // Persist the messages so that we have them even if the queue goes down
                PersistentMessages = true,

                // Confirm when a message has been published
                PublisherConfirms = true,

                // Get one message, process it and get the next one after we acknowledge. This
                // enables load balancing and also ensures we don't have to handle keeping the messages safe
                PrefetchCount = 1,
            };
        }

        private static void RegisterDispensers(ContainerBuilder builder)
        {
            builder.Register(c => new PersistentSignalDispenser(
                    c.Resolve<IBus>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IDispenseSignals>()
                .SingleInstance();
        }

        private static void RegisterQueues(ContainerBuilder builder)
        {
            builder.Register(c => new PersistentSignalPublisher(
                    c.Resolve<IBus>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IPublishSignals>()
                .SingleInstance();
        }

        private static void RegisterRabbitMQ(ContainerBuilder builder)
        {
            builder.Register(
                    (c, p) =>
                    {
                        var configuration = c.Resolve<IConfiguration>();
                        var ctx = c.Resolve<IComponentContext>();
                        return RabbitHutch.CreateBus(
                            RabbitMQConnectionFromConfiguration(configuration),
                            x =>
                            {
                                // logger
                                x.Register(serviceProvider => ctx.Resolve<IEasyNetQLogger>());

                                // In a cluster
                                x.Register<IClusterHostSelectionStrategy<ConnectionFactoryInfo>, RandomClusterHostSelectionStrategy<ConnectionFactoryInfo>>();
                            });
                    })
                .As<IBus>()
                .SingleInstance();
        }

        private static void RegisterRabbitMQOverrides(ContainerBuilder builder)
        {
            builder.Register(c => new EasyNetQLogger(c.Resolve<SystemDiagnostics>()))
                .As<IEasyNetQLogger>();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterDispensers(builder);
            RegisterQueues(builder);
            RegisterRabbitMQ(builder);
            RegisterRabbitMQOverrides(builder);
        }
    }
}
