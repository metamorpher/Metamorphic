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
using Metamorphic.Core.Rules;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;
using Test.SourceOnly;

namespace Metamorphic.Storage.Discovery.FileSystem
{
    [TestFixture]
    public sealed class DirectoryFileListenerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Discovery.FileSystem.DirectoryFileListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryFileListener(
                    null,
                    new List<IProcessFileChanges> { new Mock<IProcessFileChanges>().Object },
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Discovery.FileSystem.DirectoryFileListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryFileListener(
                    new Mock<IConfiguration>().Object,
                    new List<IProcessFileChanges> { new Mock<IProcessFileChanges>().Object },
                    null,
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Discovery.FileSystem.DirectoryFileListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryFileListener(
                    new Mock<IConfiguration>().Object,
                    new List<IProcessFileChanges> { new Mock<IProcessFileChanges>().Object },
                    new SystemDiagnostics((l, m) => { }, null),
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Storage.Discovery.FileSystem.DirectoryFileListener",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullLoader()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DirectoryFileListener(
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
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => k.Equals(RuleConfigurationKeys.RuleLocations))))
                    .Returns(new[] { @"c:\temp" });
            }

            var loader = new Mock<IProcessFileChanges>();
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var listener = new DirectoryFileListener(
                configuration.Object,
                new List<IProcessFileChanges> { loader.Object },
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.DoesNotThrow(() => listener.Enable());
            Assert.DoesNotThrow(() => listener.Disable());
        }

        [Test]
        public void Enable()
        {
            var ruleFilesDirectory = Path.Combine(
                Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
                "TestFiles");

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => k.Equals(RuleConfigurationKeys.RuleLocations))))
                    .Returns(new[] { ruleFilesDirectory });
            }

            IEnumerable<string> files = null;
            var loader = new Mock<IProcessFileChanges>();
            {
                loader.Setup(l => l.Added(It.IsAny<IEnumerable<string>>()))
                    .Callback<IEnumerable<string>>(c => files = c);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(
                        new string[]
                        {
                            Path.Combine(ruleFilesDirectory, "EndsWithCondition.mmrule"),
                        }));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var listener = new DirectoryFileListener(
                configuration.Object,
                new List<IProcessFileChanges> { loader.Object },
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            listener.Enable();

            Assert.AreEqual(1, files.Count());

            var file = files.First();
            Assert.AreEqual(Path.Combine(ruleFilesDirectory, "EndsWithCondition.mmrule"), file);
        }
    }
}
