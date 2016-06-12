//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
            {
                if (!configuration.HasValueFor(QueueingConfigurationKeys.RabbitMqUserName))
                {
                    throw new MissingConfigurationException(QueueingConfigurationKeys.RabbitMqUserName);
                }

                if (!configuration.HasValueFor(QueueingConfigurationKeys.RabbitMqUserPassword))
                {
                    throw new MissingConfigurationException(QueueingConfigurationKeys.RabbitMqUserPassword);
                }
            }

            var hosts = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMqHosts)
                ? configuration.Value<string[]>(QueueingConfigurationKeys.RabbitMqHosts)
                    .Select(
                        s => 
                        {
                            var uri = new Uri("http://" + s);
                            return new HostConfiguration
                            {
                                Host = uri.DnsSafeHost,
                                Port = (ushort)uri.Port
                            };
                        })
                : new[]
                    {
                        new HostConfiguration
                        {
                            Host = "localhost",
                            Port = DefaultPort
                        }
                    };

            return new ConnectionConfiguration
                {
                    VirtualHost = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMqVirtualHostName) 
                        ? configuration.Value<string>(QueueingConfigurationKeys.RabbitMqVirtualHostName) 
                        : DefaultVirtualHostName,
                    Port = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMqDefaultPort) 
                        ? configuration.Value<ushort>(QueueingConfigurationKeys.RabbitMqDefaultPort) 
                        : DefaultPort,

                    Hosts = hosts,

                    UserName = configuration.Value<string>(QueueingConfigurationKeys.RabbitMqUserName),
                    Password = configuration.Value<string>(QueueingConfigurationKeys.RabbitMqUserPassword),

                    RequestedHeartbeat = configuration.HasValueFor(QueueingConfigurationKeys.RabbitMqHeartbeatInSeconds) 
                        ? configuration.Value<ushort>(QueueingConfigurationKeys.RabbitMqHeartbeatInSeconds) 
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
            RabbitHutch.SetContainerFactory(
                () =>
                {
                    var localBuilder = new ContainerBuilder();
                    return new EasyNetQ.DI.AutofacAdapter(localBuilder);
                });

            builder.Register(
                    (c,p) => 
                    {
                        var configuration = c.Resolve<IConfiguration>();
                        return RabbitHutch.CreateBus(
                            RabbitMQConnectionFromConfiguration(configuration),
                            advancedBusEventHandlers: null,
                            registerServices: 
                                x => 
                                {
                                    // In a cluster 
                                    x.Register<IClusterHostSelectionStrategy<ConnectionFactoryInfo>, RandomClusterHostSelectionStrategy<ConnectionFactoryInfo>>();
                                });
                    })
                .As<IBus>()
                .SingleInstance();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterQueues(builder);
            RegisterRabbitMQ(builder);
        }
    }
}
