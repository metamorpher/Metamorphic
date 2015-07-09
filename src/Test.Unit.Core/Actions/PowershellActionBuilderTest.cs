//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
            var output = string.Empty;
            Action<LevelToLog, string> logger = (l, m) => 
                {
                    output = m;
                };
            var diagnostics = new SystemDiagnostics(logger, null);
            var builder = new PowershellActionBuilder(diagnostics);

            var definition = builder.ToDefinition();
            Assert.AreEqual(new ActionId("powershell"), definition.Id);
        }
    }
}
