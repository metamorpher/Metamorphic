﻿//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metamorphic.Core;
using Metamorphic.Sensor.Http.Nuclei.ExceptionHandling;
using Nuclei.Configuration;
using Nuclei.Diagnostics.Logging;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace Metamorphic.Sensor.Http
{
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1400:AccessModifierMustBeDeclared",
        Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "storage.error.log";

        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionApplicationExitCode = 1;

        static int Main()
        {
            int functionReturnResult = -1;

            var processor = new LogBasedExceptionProcessor(
                LoggerBuilder.ForFile(
                    Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultErrorFileName),
                    new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now)));
            var result = TopLevelExceptionGuard.RunGuarded(
                () => functionReturnResult = RunApplication(),
                new ExceptionProcessor[]
                    {
                        processor.Process,
                    });

            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're catching the exception and then exiting the application.")]
        private static int RunApplication()
        {
            var host = HostFactory.New(
                c =>
                {
                    c.Service(
                        (ServiceConfigurator<ServiceEntryPoint> s) =>
                        {
                            s.ConstructUsing(() => new ServiceEntryPoint());
                            s.WhenStarted(m => m.OnStart());
                            s.WhenStopped(m => m.OnStop());
                        });
                    c.RunAsNetworkService();
                    c.StartAutomatically();

                    c.DependsOnEventLog();

                    c.EnableShutdown();

                    c.SetServiceName(Settings.Default.ServiceName);
                    c.SetDisplayName(Settings.Default.ServiceDisplayName);
                    c.SetDescription(Resources.Service_Description);
                });

            var exitCode = host.Run();
            return (exitCode == TopshelfExitCode.Ok)
                ? NormalApplicationExitCode
                : UnhandledExceptionApplicationExitCode;
        }
    }
}
