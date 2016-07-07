//-----------------------------------------------------------------------
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Metamorphic.Actions.Powershell;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Server
{
    [TestFixture]
    public sealed class RemoteActionExecutorTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.RemoteActionExecutor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullContainerBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => new RemoteActionExecutor(null, new Mock<ILogMessagesFromRemoteAppDomains>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.RemoteActionExecutor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullLogger()
        {
            Assert.Throws<ArgumentNullException>(() => new RemoteActionExecutor(new ContainerBuilder(), null));
        }

        [Test]
        public void Execute()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var output = new List<string>();
            Action<LevelToLog, string> logger = (l, m) =>
            {
                output.Add(m);
            };

            var builder = new ContainerBuilder();
            {
                builder.RegisterInstance(configuration.Object)
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new SystemDiagnostics(logger, null))
                    .As<SystemDiagnostics>()
                    .SingleInstance();
            }

            var executor = new RemoteActionExecutor(builder, new Mock<ILogMessagesFromRemoteAppDomains>().Object);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var powershellScriptPath = Path.Combine(currentDirectory, "hello.ps1");
            var powershellScriptContent = @"param( [string]$text ) Write-Output ('hello ' + $text)";
            File.WriteAllText(powershellScriptPath, powershellScriptContent);

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "scriptFile", powershellScriptPath },
                    { "arguments", "-text 'world'" },
                };
            var job = new Job(actionId, parameters);
            var actionDefinition = new ActionDefinition(
                actionId,
                "b",
                "1.0.0",
                typeof(PowershellActions).AssemblyQualifiedName,
                "InvokePowershell",
                new ActionParameterDefinition[]
                {
                    new ActionParameterDefinition("scriptFile"),
                    new ActionParameterDefinition("arguments"),
                });
            executor.Execute(job, actionDefinition);

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("Powershell script finished", output[1]);
        }

        [Test]
        public void ExecuteWithNullAction()
        {
            var executor = new RemoteActionExecutor(
                new ContainerBuilder(),
                new Mock<ILogMessagesFromRemoteAppDomains>().Object);

            Assert.DoesNotThrow(() => executor.Execute(
                new Job(new ActionId("a"), new Dictionary<string, object>()),
                null));
        }

        [Test]
        public void ExecuteWithNullJob()
        {
            var executor = new RemoteActionExecutor(
                new ContainerBuilder(),
                new Mock<ILogMessagesFromRemoteAppDomains>().Object);

            Assert.DoesNotThrow(() => executor.Execute(
                null,
                new ActionDefinition(
                    new ActionId("a"),
                    "b",
                    "1.0.0",
                    "c",
                    "d",
                    new ActionParameterDefinition[0])));
        }
    }
}
