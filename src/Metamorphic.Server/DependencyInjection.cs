//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Actions;
using Metamorphic.Server.Jobs;
using Metamorphic.Server.Rules;
using Metamorphic.Server.Signals;
using Nuclei;
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
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        ServerConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new ApplicationConstants())
                    .As<ApplicationConstants>();

                builder.Register(c => new FileConstants(c.Resolve<ApplicationConstants>()))
                    .As<FileConstants>();

                RegisterActions(builder);
                RegisterControllers(builder);
                RegisterDiagnostics(builder);
                RegisterJobs(builder);
                RegisterLoggers(builder);
                RegisterRules(builder);
                RegisterSignals(builder);

                builder.RegisterModule(new QueueingModule());

                builder.Register(c => new SignalProcessor(
                        c.Resolve<IQueueJobs>(),
                        c.Resolve<IStoreRules>(),
                        c.Resolve<IDispenseSignals>(),
                        c.Resolve<SystemDiagnostics>()))
                    .SingleInstance();
            }

            return builder.Build();
        }

        private static void RegisterActions(ContainerBuilder builder)
        {
            builder.Register(c => new PowershellActionBuilder(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IActionBuilder>()
                .SingleInstance();

            builder.Register(c => new ActionStorage())
                .As<IStoreActions>()
                .OnActivated(
                    a =>
                    {
                        var collection = a.Instance;
                        var builders = a.Context.Resolve<IEnumerable<IActionBuilder>>();
                        foreach (var b in builders)
                        {
                            var definition = b.ToDefinition();
                            collection.Add(definition);
                        }
                    })
                .SingleInstance();
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            builder.Register(c => new SignalController(c.Resolve<IPublishSignals>()))
                .InstancePerRequest();
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

        private static void RegisterJobs(ContainerBuilder builder)
        {
            builder.Register(c => new JobProcessor(
                    c.Resolve<IStoreActions>(),
                    c.Resolve<IQueueJobs>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IProcessJobs>()
                .SingleInstance();

            builder.Register(c => new JobQueue())
                .As<IQueueJobs>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(c.Resolve<FileConstants>().LogPath(), DefaultInfoFileName),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now),
                    assemblyInfo.Name,
                    assemblyInfo.Version))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterRules(ContainerBuilder builder)
        {
            builder.Register(c => new RuleCollection(c.Resolve<SystemDiagnostics>()))
                .As<IStoreRules>()
                .SingleInstance();

            builder.Register(
                    c => 
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new RuleLoader(
                            (string id) => ctx.Resolve<IStoreActions>().HasActionFor(new ActionId(id)),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<ILoadRules>()
                .SingleInstance();

            builder.Register(c => new RuleWatcher(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<ILoadRules>(),
                    c.Resolve<IStoreRules>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IWatchRules>()
                .SingleInstance();
        }

        private static void RegisterSignals(ContainerBuilder builder)
        {
            builder.Register(c => new WebCallSignalGenerator())
                .As<IGenerateSignals>()
                .SingleInstance();
        }
    }
}