//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Moq;
using Nuclei.Diagnostics;
using NuGet;
using NUnit.Framework;
using Test.SourceOnly;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Core
{
    [TestFixture]
    public sealed class PackageUtilitiesTest
    {
        [Test]
        public void CopyPackageFilesToSinglePathWithEmptyDestination()
        {
            Assert.Throws<ArgumentException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    "*.dll",
                    string.Empty,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithEmptyPath()
        {
            Assert.Throws<ArgumentException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    string.Empty,
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    "*.dll",
                    "b",
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithEmptySearchPattern()
        {
            Assert.Throws<ArgumentException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    string.Empty,
                    "b",
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithNullDestination()
        {
            Assert.Throws<ArgumentNullException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    "*.dll",
                    null,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    "*.dll",
                    "b",
                    null,
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    "*.dll",
                    "b",
                    new SystemDiagnostics((l, m) => { }, null),
                    null));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithNullId()
        {
            Assert.Throws<ArgumentNullException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    null,
                    "*.dll",
                    "b",
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithNullPath()
        {
            Assert.Throws<ArgumentNullException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    null,
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    "*.dll",
                    "b",
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithNullSearchPattern()
        {
            Assert.Throws<ArgumentNullException>(
                () => PackageUtilities.CopyPackageFilesToSinglePath(
                    "a",
                    new PackageName("a", new SemanticVersion("1.0.0")),
                    null,
                    "b",
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void CopyPackageFilesToSinglePathWithSinglePackage()
        {
            var knownFiles = new List<string>
            {
                "c:/a/b/c/d.dll",
                "c:/a/b/c/e.dll",

                "c:/a/b/c/f/g.dll",
                "c:/a/b/c/f/h.dll",

                "c:/a/b/c/i/j.dll",
                "c:/a/b/c/i/k.dll",
            };

            var copiedFiles = new Dictionary<string, string>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(knownFiles));
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(new Dictionary<string, string>(), copiedFiles));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            PackageUtilities.CopyPackageFilesToSinglePath(
                @"c:\a\b",
                new PackageName("a", new SemanticVersion("1.0.0")),
                "*.dll",
                @"d:\e",
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.AreEqual(knownFiles.Count, copiedFiles.Count);
            Assert.AreEqual(@"d:\e\d.dll", copiedFiles[knownFiles[0]]);
            Assert.AreEqual(@"d:\e\e.dll", copiedFiles[knownFiles[1]]);
            Assert.AreEqual(@"d:\e\g.dll", copiedFiles[knownFiles[2]]);
            Assert.AreEqual(@"d:\e\h.dll", copiedFiles[knownFiles[3]]);
            Assert.AreEqual(@"d:\e\j.dll", copiedFiles[knownFiles[4]]);
            Assert.AreEqual(@"d:\e\k.dll", copiedFiles[knownFiles[5]]);
        }
    }
}
