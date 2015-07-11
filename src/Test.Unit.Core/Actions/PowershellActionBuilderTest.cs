//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            var output = new List<string>();
            Action<LevelToLog, string> logger = (l, m) => 
                {
                    output.Add(m);
                };
            var diagnostics = new SystemDiagnostics(logger, null);
            var builder = new PowershellActionBuilder(diagnostics);

            var definition = builder.ToDefinition();
            Assert.AreEqual(new ActionId("powershell"), definition.Id);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var powershellScriptPath = Path.Combine(currentDirectory, "hello.ps1");
            var powershellScriptContent = @"Write-Output 'hello'";
            File.WriteAllText(powershellScriptPath, powershellScriptContent);

            var parameters = new[]
                {
                    new ActionParameterValueMap(
                        new ActionParameterDefinition("scriptFile"),
                        powershellScriptPath),
                };
            definition.Invoke(parameters);
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("Powershell script finished", output[1]);
        }
    }
}
