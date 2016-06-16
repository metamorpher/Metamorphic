//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NUnit.Framework;

namespace Metamorphic.Core.Actions
{
    [TestFixture]
    public sealed class PowershellActionBuilderTest
    {
        [Test]
        public void ToDefinition()
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
            var diagnostics = new SystemDiagnostics(logger, null);
            var builder = new PowershellActionBuilder(configuration.Object, diagnostics);

            var definition = builder.ToDefinition();
            Assert.AreEqual(new ActionId("powershell"), definition.Id);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var powershellScriptPath = Path.Combine(currentDirectory, "hello.ps1");
            var powershellScriptContent = @"param( [string]$text ) Write-Output ('hello ' + $text)";
            File.WriteAllText(powershellScriptPath, powershellScriptContent);

            var parameters = new[]
                {
                    new ActionParameterValueMap(
                        new ActionParameterDefinition("scriptFile"),
                        powershellScriptPath),
                    new ActionParameterValueMap(
                        new ActionParameterDefinition("arguments"),
                        "-text 'world'"),
                };
            definition.Invoke(parameters);
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("Powershell script finished", output[1]);
        }
    }
}
