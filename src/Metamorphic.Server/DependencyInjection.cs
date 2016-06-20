//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Autofac;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Commands;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Server.Nuclei.AppDomains;
using Nuclei;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Metamorphic.Server
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "server.info.log";

        private static AppDomainResolutionPaths AppDomainResolutionPathsFor(string[] additionalPaths)
        {
            var directoryPaths = new List<string>();
            directoryPaths.Add(Assembly.GetExecutingAssembly().LocalDirectoryPath());

            if (additionalPaths != null)
            {
                directoryPaths.AddRange(additionalPaths);
            }

            return AppDomainResolutionPaths.WithFilesAndDirectories(
                Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
                new string[0],
                directoryPaths);
        }

        /// <summary>
        /// Creates the dependency injection container for the application.
        /// </summary>
        /// <returns>The container.</returns>
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            {
                builder.Register(c => new XmlConfiguration(
                        ServerConfigurationKeys.ToCollection()
                            .Append(CoreConfigurationKeys.ToCollection())
                            .Append(QueueingConfigurationKeys.ToCollection())
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        ServerConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new ApplicationConstants())
                    .As<ApplicationConstants>();

                builder.Register(c => new FileConstants(c.Resolve<ApplicationConstants>()))
                    .As<FileConstants>();

                builder.RegisterModule(
                    new CommunicationModule(
                        new[]
                            {
                                // For now only allowing the different applications to be on the same machine
                                // because there is no way to do a limited discovery (i.e. only a fixed set of machines)
                                ChannelTemplate.NamedPipe
                            },
                        true));

                builder.RegisterModule(new CoreModule());
                builder.RegisterModule(new QueueingModule());

                RegisterAppDomainBuilder(builder);
                RegisterCommunication(builder);
                RegisterDiagnostics(builder);
                RegisterLoggers(builder);
                RegisterProcessor(builder);
                RegisterProxies(builder);

                builder.RegisterModule(new QueueingModule());

                builder.Register(c => new SignalProcessor(
                        c.Resolve<IQueueJobs>(),
                        c.Resolve<IDispenseSignals>(),
                        c.Resolve<SystemDiagnostics>()))
                    .SingleInstance();
            }

            return builder.Build();
        }

        private static void RegisterAppDomainBuilder(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    Func<string, string[], AppDomain> result =
                        (name, paths) => AppDomainBuilder.Assemble(
                            name,
                            AppDomainResolutionPathsFor(paths));

                    return result;
                })
                .As<Func<string, string[], AppDomain>>()
                .SingleInstance();
        }

        private static void RegisterCommunication(ContainerBuilder builder)
        {
            builder.Register(c => new CommunicationInitializer(
                    c.Resolve<IComponentContext>()))
                .As<IInitializeCommunicationInstances>()
                .SingleInstance();
        }

        private static void RegisterDiagnostics(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var loggers = c.Resolve<IEnumerable<ILogger>>();
                    Action<LevelToLog, string> action = (p, s) =>
                    {
                        var msg = new LogMessage(p, s);
                        foreach (var logger in loggers)
                        {
                            try
                            {
                                logger.Log(msg);
                            }
                            catch (NLog.NLogRuntimeException)
                            {
                                // Ignore it and move on to the next logger.
                            }
                        }
                    };

                    Profiler profiler = null;
                    if (c.IsRegistered<Profiler>())
                    {
                        profiler = c.Resolve<Profiler>();
                    }

                    return new SystemDiagnostics(action, profiler);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(c.Resolve<FileConstants>().LogPath(), DefaultInfoFileName),
                    new DebugLogTemplate(c.Resolve<IConfiguration>(), () => DateTimeOffset.Now),
                    assemblyInfo.Name,
                    assemblyInfo.Version))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterProcessor(ContainerBuilder builder)
        {
            Func<AppDomain, ILoadActionExecutorsInRemoteAppDomains> executorBuilder =
                a =>
                {
                    var loader = a.CreateInstanceAndUnwrap(
                        typeof(AppDomainActionClassLoader).Assembly.FullName,
                        typeof(AppDomainActionClassLoader).FullName) as AppDomainActionClassLoader;
                    return loader;
                };
            builder.Register(c => new SignalProcessor(
                    c.Resolve<IActionStorageProxy>(),
                    c.Resolve<IInstallPackages>(),
                    c.Resolve<Func<string, string[], AppDomain>>(),
                    executorBuilder,
                    c.Resolve<IStoreRules>(),
                    c.Resolve<IDispenseSignals>(),
                    c.Resolve<SystemDiagnostics>(),
                    c.Resolve<IFileSystem>()))
                .SingleInstance();
        }

        private static void RegisterProxies(ContainerBuilder builder)
        {
            builder.Register(c => new ActionStorageProxy(
                    c.Resolve<ISendCommandsToRemoteEndpoints>()))
                .As<IActionStorageProxy>()
                .SingleInstance();
        }
                            (string id) => ctx.Resolve<IStoreActions>().HasActionFor(new ActionId(id)),
    }
}
