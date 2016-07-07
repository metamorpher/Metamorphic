//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Diagnostics;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Storage.Actions
{
    [TestFixture]
    public sealed class ActionPackageDetectorTest
    {
        [Test]
        public void Added()
        {
            var repository = new Mock<IStoreActions>();

            IEnumerable<PackageName> packages = null;
            var scanner = new Mock<IScanActionPackages>();
            {
                scanner.Setup(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()))
                    .Callback<IEnumerable<PackageName>>(c => packages = c)
                    .Verifiable();
            }

            var detector = new ActionPackageDetector(
                repository.Object,
                s => scanner.Object,
                new SystemDiagnostics((l, m) => { }, null));

            var packagesToScan = new[] { new PackageName("a", new SemanticVersion("1.0.0")) };
            detector.Added(packagesToScan);

            Assert.AreSame(packagesToScan, packages);
            scanner.Verify(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()), Times.Once());
        }

        [Test]
        public void AddedWithNullPackageCollection()
        {
            var repository = new Mock<IStoreActions>();

            var scanner = new Mock<IScanActionPackages>();
            {
                scanner.Setup(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var detector = new ActionPackageDetector(
                repository.Object,
                s => scanner.Object,
                new SystemDiagnostics((l, m) => { }, null));

            detector.Added(null);

            scanner.Verify(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
        }

        [Test]
        public void AddedWithoutPackages()
        {
            var repository = new Mock<IStoreActions>();
            var scanner = new Mock<IScanActionPackages>();
            {
                scanner.Setup(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var detector = new ActionPackageDetector(
                repository.Object,
                s => scanner.Object,
                new SystemDiagnostics((l, m) => { }, null));

            var packagesToScan = new PackageName[0];
            detector.Added(packagesToScan);

            scanner.Verify(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.ActionPackageDetector",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ActionPackageDetector(
                    new Mock<IStoreActions>().Object,
                    s => null,
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.ActionPackageDetector",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullRepository()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ActionPackageDetector(
                    null,
                    s => null,
                    new SystemDiagnostics((l, m) => { }, null)));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Actions.ActionPackageDetector",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullScanner()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ActionPackageDetector(
                    new Mock<IStoreActions>().Object,
                    null,
                    new SystemDiagnostics((l, m) => { }, null)));
        }

        [Test]
        public void Removed()
        {
            IEnumerable<PackageName> packages = null;
            var repository = new Mock<IStoreActions>();
            {
                repository.Setup(r => r.RemovePackages(It.IsAny<IEnumerable<PackageName>>()))
                    .Callback<IEnumerable<PackageName>>(c => packages = c)
                    .Verifiable();
            }

            var scanner = new Mock<IScanActionPackages>();
            {
                scanner.Setup(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var detector = new ActionPackageDetector(
                repository.Object,
                s => scanner.Object,
                new SystemDiagnostics((l, m) => { }, null));

            var packagesToScan = new[] { new PackageName("a", new SemanticVersion("1.0.0")) };
            detector.Removed(packagesToScan);

            Assert.AreSame(packagesToScan, packages);
            repository.Verify(r => r.RemovePackages(It.IsAny<IEnumerable<PackageName>>()), Times.Once());
            scanner.Verify(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
        }

        [Test]
        public void RemovedWithNullPackageCollection()
        {
            var repository = new Mock<IStoreActions>();
            {
                repository.Setup(r => r.RemovePackages(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var scanner = new Mock<IScanActionPackages>();
            {
                scanner.Setup(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var detector = new ActionPackageDetector(
                repository.Object,
                s => scanner.Object,
                new SystemDiagnostics((l, m) => { }, null));

            detector.Removed(null);

            repository.Verify(r => r.RemovePackages(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
            scanner.Verify(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
        }

        [Test]
        public void RemovedWithoutPackages()
        {
            var repository = new Mock<IStoreActions>();
            {
                repository.Setup(r => r.RemovePackages(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var scanner = new Mock<IScanActionPackages>();
            {
                scanner.Setup(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()))
                    .Verifiable();
            }

            var detector = new ActionPackageDetector(
                repository.Object,
                s => scanner.Object,
                new SystemDiagnostics((l, m) => { }, null));

            var packagesToScan = new PackageName[0];
            detector.Removed(packagesToScan);

            repository.Verify(r => r.RemovePackages(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
            scanner.Verify(s => s.Scan(It.IsAny<IEnumerable<PackageName>>()), Times.Never());
        }
    }
}
