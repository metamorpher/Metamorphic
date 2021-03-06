﻿//-----------------------------------------------------------------------
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
using Metamorphic.Core.Rules;
using Metamorphic.Storage.Actions;
using Metamorphic.Storage.Discovery;
using Metamorphic.Storage.Discovery.FileSystem;
using Metamorphic.Storage.Nuclei.AppDomains;
using Metamorphic.Storage.Rules;
using Nuclei;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Metamorphic.Storage
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "storage.info.log";

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
                        StorageConfigurationKeys.ToCollection()
                            .Append(CoreConfigurationKeys.ToCollection())
                            .Append(RuleConfigurationKeys.ToCollection())
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        StorageConstants.ConfigurationSectionApplicationSettings))
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

                RegisterActions(builder);
                RegisterAppDomainBuilder(builder);
                RegisterCommunication(builder);
                RegisterDiagnostics(builder);
                RegisterDiscovery(builder);
                RegisterLoggers(builder);
                RegisterRules(builder);
            }

            return builder.Build();
        }

        private static void RegisterActions(ContainerBuilder builder)
        {
            Func<AppDomain, ILoadPackageScannersInRemoteAppDomains> scannerBuilder =
                a =>
                {
                    var loader = a.CreateInstanceAndUnwrap(
                           typeof(AppDomainPackageClassLoader).Assembly.FullName,
                           typeof(AppDomainPackageClassLoader).FullName) as AppDomainPackageClassLoader;
                    return loader;
                };
            builder.Register((c, p) => new AppDomainOwningActionPackageScanner(
                    c.Resolve<IInstallPackages>(),
                    c.Resolve<Func<string, string[], AppDomain>>(),
                    scannerBuilder,
                    p.TypedAs<IStoreActions>(),
                    c.Resolve<SystemDiagnostics>(),
                    c.Resolve<IFileSystem>()))
                .As<IScanActionPackages>();

            builder.Register<Func<IStoreActions, IScanActionPackages>>(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return r => ctx.Resolve<IScanActionPackages>(new TypedParameter(typeof(IStoreActions), r));
                    });

            builder.Register(c => new ActionPackageDetector(
                    c.Resolve<IStoreActions>(),
                    c.Resolve<Func<IStoreActions, IScanActionPackages>>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IProcessPackageChanges>();

            builder.Register(c => new ActionStorage())
                .As<IStoreActions>()
                .SingleInstance();
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

        private static void RegisterDiscovery(ContainerBuilder builder)
        {
            builder.Register(c => new DirectoryPackageListener(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<IEnumerable<IProcessPackageChanges>>(),
                    c.Resolve<SystemDiagnostics>(),
                    c.Resolve<IFileSystem>()))
                .As<IWatchPackages>()
                .SingleInstance();

            builder.Register(c => new DirectoryFileListener(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<IEnumerable<IProcessFileChanges>>(),
                    c.Resolve<SystemDiagnostics>(),
                    c.Resolve<IFileSystem>()))
                .As<IWatchPackages>()
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

        private static void RegisterRules(ContainerBuilder builder)
        {
            builder.Register(c => new RuleCollection())
                .As<IStoreRules>()
                .SingleInstance();

            builder.Register(c => new RuleLoader(c.Resolve<SystemDiagnostics>()))
                .As<ILoadRules>()
                .SingleInstance();

            builder.Register(c => new RulePackageDetector(
                    c.Resolve<IStoreRules>(),
                    c.Resolve<ILoadRules>(),
                    c.Resolve<IInstallPackages>(),
                    c.Resolve<SystemDiagnostics>(),
                    c.Resolve<IFileSystem>()))
                .As<IProcessPackageChanges>()
                .SingleInstance();

            builder.Register(c => new RuleFileDetector(
                    c.Resolve<IStoreRules>(),
                    c.Resolve<ILoadRules>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IProcessFileChanges>()
                .SingleInstance();
        }
    }
}
