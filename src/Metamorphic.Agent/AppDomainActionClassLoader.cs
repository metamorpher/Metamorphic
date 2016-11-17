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
using Metamorphic.Core;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Agent
{
    /// <summary>
    /// Defines methods to load an <see cref="IExecuteActions"/> object into a remote <c>AppDomain</c>.
    /// </summary>
    internal sealed class AppDomainActionClassLoader : MarshalByRefObject, ILoadActionExecutorsInRemoteAppDomains
    {
        private static ContainerBuilder CreateBuilder(ILogMessagesFromRemoteAppDomains logger)
        {
            var builder = new ContainerBuilder();
            {
                builder.Register(
                        c =>
                        {
                            var configs = c.Resolve<IEnumerable<IProvideConfigurationKeys>>();
                            var keys = configs.SelectMany(configCollection => configCollection.ToCollection())
                                .Append(AgentConfigurationKeys.ToCollection())
                                .Append(CoreConfigurationKeys.ToCollection())
                                .Append(DiagnosticsConfigurationKeys.ToCollection())
                                .ToList();
                            return new XmlConfiguration(keys, AgentConstants.ConfigurationSectionApplicationSettings);
                        })
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new ApplicationConstants())
                    .As<ApplicationConstants>();

                builder.Register(c => new FileConstants(c.Resolve<ApplicationConstants>()))
                    .As<FileConstants>();

                builder.RegisterModule(new CoreModule());

                RegisterDiagnostics(builder, logger);
            }

            return builder;
        }

        private static void RegisterDiagnostics(ContainerBuilder builder, ILogMessagesFromRemoteAppDomains logger)
        {
            builder.Register(
                c =>
                {
                    return new SystemDiagnostics(logger.Log, null);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        /// <summary>
        /// Loads the <see cref="IExecuteActions"/> object into the <c>AppDomain</c> in which the current
        /// object is currently loaded.
        /// </summary>
        /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
        /// <returns>The newly created <see cref="IExecuteActions"/> object.</returns>
        public IExecuteActions Load(ILogMessagesFromRemoteAppDomains logger)
        {
            try
            {
                var builder = CreateBuilder(logger);
                return new RemoteActionExecutor(builder, logger);
            }
            catch (Exception e)
            {
                logger.Log(LevelToLog.Error, e.ToString());
                throw;
            }
        }
    }
}
