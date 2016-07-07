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
using Metamorphic.Actions.Powershell;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Moq;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Storage.Actions
{
    [TestFixture]
    public sealed class RemotePackageScannerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.RemotePackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullLogger()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RemotePackageScanner(
                    new Mock<IStoreActions>().Object,
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.RemotePackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullRepository()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RemotePackageScanner(
                    null,
                    new Mock<ILogMessagesFromRemoteAppDomains>().Object));
        }

        [Test]
        public void Scan()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var packageName = "a";
            var packageVersion = "1.0.0";

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var filesToScan = new string[]
                {
                    Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                };
            scanner.Scan(packageName, packageVersion, filesToScan);

            Assert.AreEqual(1, actions.Count);

            var definition = actions[0];
            Assert.AreEqual(new ActionId("powershell"), definition.Id);
            Assert.AreEqual(new PackageName(packageName, new SemanticVersion(packageVersion)), definition.Package);
            Assert.AreEqual(typeof(PowershellActions).AssemblyQualifiedName, definition.ActionType);
            Assert.AreEqual("InvokePowershell", definition.ActionMethod);

            Assert.AreEqual(2, definition.Parameters.Count);
            Assert.AreEqual("scriptFile", definition.Parameters.First().Name);
            Assert.AreEqual("arguments", definition.Parameters.Last().Name);
        }

        [Test]
        public void ScanWithEmptyFileCollection()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var filesToScan = new string[0];
            scanner.Scan("a", "1.0.0", filesToScan);

            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void ScanWithEmptyPackageName()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var filesToScan = new string[]
                {
                    Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                };
            scanner.Scan(string.Empty, "1.0.0", filesToScan);

            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void ScanWithEmptyPackageVersion()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var filesToScan = new string[]
                {
                    Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                };
            scanner.Scan("a", string.Empty, filesToScan);

            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void ScanWithNullFileCollection()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var packageName = "a";
            var packageVersion = "1.0.0";
            scanner.Scan(packageName, packageVersion, null);

            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void ScanWithNullPackageName()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var filesToScan = new string[]
                {
                    Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                };
            scanner.Scan(null, "1.0.0", filesToScan);

            Assert.AreEqual(0, actions.Count);
        }

        [Test]
        public void ScanWithNullPackageVersion()
        {
            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var logger = new Mock<ILogMessagesFromRemoteAppDomains>();

            var scanner = new RemotePackageScanner(storage.Object, logger.Object);

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var filesToScan = new string[]
                {
                    Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                };
            scanner.Scan("a", null, filesToScan);

            Assert.AreEqual(0, actions.Count);
        }
    }
}
