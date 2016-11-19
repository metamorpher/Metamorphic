//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Tracing;
using Autofac;
using Metamorphic.Core;
using Metamorphic.Core.Queueing;
using Metamorphic.Sensor.Http.Models;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;

namespace Metamorphic.Sensor.Http
{
    internal static class DependencyInjection
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "web.api.info.log";

        /// <summary>
        /// Creates the DI container for the machine.
        /// </summary>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new QueueingModule());

                builder.Register(c => new XmlConfiguration(
                        SensorHttpConfigurationKeys.ToCollection()
                            .Append(CoreConfigurationKeys.ToCollection())
                            .Append(QueueingConfigurationKeys.ToCollection())
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        SensorHttpConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new NucleiBasedTraceWriter(
                        c.Resolve<SystemDiagnostics>()))
                    .As<ITraceWriter>()
                    .SingleInstance();

                RegisterLoggers(builder);
                RegisterDiagnostics(builder);
                RegisterControllers(builder);
                RegisterSiteInformation(builder);
            }

            return builder.Build();
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

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), foobar(), DefaultInfoFileName),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now),
                    assemblyInfo.Name,
                    assemblyInfo.Version))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterSiteInformation(ContainerBuilder builder)
        {
            builder.Register(c => new SiteInformationModel())
                .SingleInstance();
        }
    }
}
