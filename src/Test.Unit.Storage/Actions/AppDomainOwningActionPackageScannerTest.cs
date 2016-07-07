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
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Metamorphic.Actions.Powershell;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Moq;
using Nuclei.Diagnostics;
using NuGet;
using NUnit.Framework;
using Test.SourceOnly;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Storage.Actions
{
    [TestFixture]
    public sealed class AppDomainOwningActionPackageScannerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.AppDomainOwningActionPackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullAppDomainBuilder()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AppDomainOwningActionPackageScanner(
                    new Mock<IInstallPackages>().Object,
                    null,
                    a => null,
                    new Mock<IStoreActions>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.AppDomainOwningActionPackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AppDomainOwningActionPackageScanner(
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    new Mock<IStoreActions>().Object,
                    null,
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.AppDomainOwningActionPackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AppDomainOwningActionPackageScanner(
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    new Mock<IStoreActions>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.AppDomainOwningActionPackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullInstaller()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AppDomainOwningActionPackageScanner(
                    null,
                    (name, paths) => null,
                    a => null,
                    new Mock<IStoreActions>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.AppDomainOwningActionPackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullRepository()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AppDomainOwningActionPackageScanner(
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    null,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.AppDomainOwningActionPackageScanner",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullScannerBuilder()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AppDomainOwningActionPackageScanner(
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    null,
                    new Mock<IStoreActions>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        public void Scan()
        {
            var packages = new[]
                {
                    new PackageName("a", new SemanticVersion("1.0.0"))
                };

            var installer = new Mock<IInstallPackages>();
            {
                installer.Setup(
                    i => i.Install(
                        It.IsAny<PackageName>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var files = new string[]
                {
                    Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                };
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(files));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var remoteScanner = new Mock<IScanActionPackageFiles>();
            {
                remoteScanner.Setup(e => e.Scan(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                    .Callback<PackageName, IEnumerable<string>>(
                        (p, f) =>
                        {
                            Assert.AreEqual(packages[0], p);
                            Assert.That(f, Is.EquivalentTo(files));
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadPackageScannersInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<IStoreActions>(), It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(remoteScanner.Object)
                    .Verifiable();
            }

            var scanner = new AppDomainOwningActionPackageScanner(
                installer.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                storage.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            scanner.Scan(packages);

            Assert.AreEqual(1, actions.Count);
            installer.Verify(
                    i => i.Install(
                        It.IsAny<PackageName>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<string, string, PackageName>>()),
                    Times.Once());
            storage.Verify(s => s.Add(It.IsAny<ActionDefinition>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<IStoreActions>(), It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            remoteScanner.Verify(e => e.Scan(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once());
        }

        [Test]
        public void ScanWithNullCollection()
        {
            var installer = new Mock<IInstallPackages>();
            {
                installer.Setup(
                    i => i.Install(
                        It.IsAny<PackageName>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var actions = new List<ActionDefinition>();
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => actions.Add(a));
            }

            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(
                        new string[]
                        {
                            Path.Combine(currentDirectory, "Metamorphic.Actions.Powershell.dll"),
                        }));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var scanner = new AppDomainOwningActionPackageScanner(
                installer.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => null,
                storage.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            scanner.Scan(null);

            Assert.AreEqual(0, actions.Count);
            installer.Verify(
                    i => i.Install(
                        It.IsAny<PackageName>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<string, string, PackageName>>()),
                    Times.Never());
            storage.Verify(s => s.Add(It.IsAny<ActionDefinition>()), Times.Never());
        }
    }
}
