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
using Metamorphic.Core;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NuGet;
using NUnit.Framework;
using Test.SourceOnly;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Storage.Actions
{
    [TestFixture]
    public sealed class DirectoryPackageListenerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.DirectoryPackageListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryPackageListener(
                    null,
                    new Mock<IDetectActionPackages>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.DirectoryPackageListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryPackageListener(
                    new Mock<IConfiguration>().Object,
                    new Mock<IDetectActionPackages>().Object,
                    null,
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.DirectoryPackageListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryPackageListener(
                    new Mock<IConfiguration>().Object,
                    new Mock<IDetectActionPackages>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.DirectoryPackageListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullLoader()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryPackageListener(
                    new Mock<IConfiguration>().Object,
                    null,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CreateWithUriNugetFeeds()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => k.Equals(CoreConfigurationKeys.NugetFeeds))))
                    .Returns(new[] { @"http://nuget.org" });
            }

            var loader = new Mock<IDetectActionPackages>();
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var listener = new DirectoryPackageListener(
                configuration.Object,
                loader.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.DoesNotThrow(() => listener.Enable());
            Assert.DoesNotThrow(() => listener.Disable());
        }

        [Test]
        public void Enable()
        {
            var currentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => k.Equals(CoreConfigurationKeys.NugetFeeds))))
                    .Returns(new[] { currentDirectory });
            }

            IEnumerable<PackageName> packages = null;
            var loader = new Mock<IDetectActionPackages>();
            {
                loader.Setup(l => l.Added(It.IsAny<IEnumerable<PackageName>>()))
                    .Callback<IEnumerable<PackageName>>(c => packages = c);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(
                        new string[]
                        {
                            Path.Combine(currentDirectory, "Test.Unit.Storage.1.0.0.nupkg"),
                        }));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var listener = new DirectoryPackageListener(
                configuration.Object,
                loader.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            listener.Enable();

            Assert.AreEqual(1, packages.Count());

            var package = packages.First();
            Assert.AreEqual("Test.Unit.Storage", package.Id);
            Assert.AreEqual(new SemanticVersion("1.0.0"), package.Version);
        }
    }
}
