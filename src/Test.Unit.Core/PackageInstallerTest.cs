//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NuGet;
using NUnit.Framework;
using Test.SourceOnly;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Core
{
    [TestFixture]
    public sealed class PackageInstallerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.PackageInstaller",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PackageInstaller(
                    null,
                    s => new Mock<IPackageRepository>().Object,
                    (r, o) => new Mock<IPackageManager>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.PackageInstaller",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PackageInstaller(
                    new Mock<IConfiguration>().Object,
                    s => new Mock<IPackageRepository>().Object,
                    (r, o) => new Mock<IPackageManager>().Object,
                    null,
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.PackageInstaller",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PackageInstaller(
                    new Mock<IConfiguration>().Object,
                    s => new Mock<IPackageRepository>().Object,
                    (r, o) => new Mock<IPackageManager>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.PackageInstaller",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullManagerFactory()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PackageInstaller(
                    new Mock<IConfiguration>().Object,
                    s => new Mock<IPackageRepository>().Object,
                    null,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.PackageInstaller",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullRepositoryFactory()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PackageInstaller(
                    new Mock<IConfiguration>().Object,
                    null,
                    (r, o) => new Mock<IPackageManager>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void Install()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var package = new MockPackage(packageName);
            var knownPackages = new[]
                {
                    package,
                };
            var repository = new Mock<IPackageRepository>();
            var packageLookup = repository.As<IPackageLookup>();
            {
                packageLookup.Setup(r => r.FindPackagesById(It.IsAny<string>()))
                    .Returns<string>(
                        n =>
                        {
                            return knownPackages.Where(p => p.Id.Equals(n, StringComparison.OrdinalIgnoreCase));
                        })
                    .Verifiable();
            }

            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            {
                packageManager.Setup(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .Verifiable();
            }

            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            var installDirectory = @"c:\packages";
            installer.Install(packageName, installDirectory);

            packageLookup.Verify(r => r.FindPackagesById(It.IsAny<string>()), Times.Once());
            packageManager.Verify(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Test]
        public void InstallWithEmptyOutputLocation()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var repository = new Mock<IPackageRepository>();
            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.Throws<ArgumentException>(() => installer.Install(packageName, string.Empty));
        }

        [Test]
        public void InstallWithNonexistentPackageName()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var repository = new Mock<IPackageRepository>();
            var packageLookup = repository.As<IPackageLookup>();
            {
                packageLookup.Setup(r => r.FindPackagesById(It.IsAny<string>()))
                    .Returns<string>(
                        n =>
                        {
                            return new IPackage[0];
                        })
                    .Verifiable();
            }

            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            {
                packageManager.Setup(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .Verifiable();
            }

            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            var installDirectory = @"c:\packages";
            installer.Install(packageName, installDirectory);

            packageLookup.Verify(r => r.FindPackagesById(It.IsAny<string>()), Times.Once());
            packageManager.Verify(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());
        }

        [Test]
        public void InstallWithNonexistentPackageVersion()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var package = new MockPackage(packageName);
            var knownPackages = new[]
                {
                    package,
                };
            var repository = new Mock<IPackageRepository>();
            var packageLookup = repository.As<IPackageLookup>();
            {
                packageLookup.Setup(r => r.FindPackagesById(It.IsAny<string>()))
                    .Returns<string>(
                        n =>
                        {
                            return knownPackages.Where(p => p.Id.Equals(n, StringComparison.OrdinalIgnoreCase));
                        })
                    .Verifiable();
            }

            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            {
                packageManager.Setup(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .Verifiable();
            }

            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            var installDirectory = @"c:\packages";
            installer.Install(new PackageName(packageName.Id, new SemanticVersion("2.0.0")), installDirectory);

            packageLookup.Verify(r => r.FindPackagesById(It.IsAny<string>()), Times.Once());
            packageManager.Verify(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());
        }

        [Test]
        public void InstallWithPackageWithDependencies()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var dependencyName = new PackageName("b", new SemanticVersion("2.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var dependencyPackage = new MockPackage(dependencyName);
            var package = new MockPackage(
                packageName,
                new PackageDependencySet[]
                {
                    new PackageDependencySet(
                        new FrameworkName(".NET Framework, Version=4.0"),
                        new PackageDependency[]
                        {
                            new PackageDependency(dependencyName.Id, new VersionSpec(dependencyName.Version))
                        })
                });
            var knownPackages = new[]
                {
                    package,
                    dependencyPackage
                };
            var repository = new Mock<IPackageRepository>();
            var packageLookup = repository.As<IPackageLookup>();
            {
                packageLookup.Setup(r => r.FindPackagesById(It.IsAny<string>()))
                    .Returns<string>(
                        n =>
                        {
                            return knownPackages.Where(p => p.Id.Equals(n, StringComparison.OrdinalIgnoreCase));
                        })
                    .Verifiable();
            }

            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            {
                packageManager.Setup(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .Verifiable();
            }

            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            var installDirectory = @"c:\packages";
            installer.Install(packageName, installDirectory);

            packageLookup.Verify(r => r.FindPackagesById(It.IsAny<string>()), Times.Exactly(3));
            packageManager.Verify(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Exactly(2));
        }

        [Test]
        public void InstallWithNullOutputLocation()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var repository = new Mock<IPackageRepository>();
            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.Throws<ArgumentNullException>(() => installer.Install(packageName, null));
        }

        [Test]
        public void InstallWithNullPackageName()
        {
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var repository = new Mock<IPackageRepository>();
            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.Throws<ArgumentNullException>(() => installer.Install(null, @"c:\temp"));
        }

        [Test]
        public void InstallWithPostInstallAction()
        {
            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var feeds = new[]
            {
                @"\\machine\directory"
            };
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.Is<ConfigurationKey>(k => CoreConfigurationKeys.NugetFeeds.Equals(k))))
                    .Returns(feeds);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var package = new MockPackage(packageName);
            var knownPackages = new[]
                {
                    package,
                };
            var repository = new Mock<IPackageRepository>();
            var packageLookup = repository.As<IPackageLookup>();
            {
                packageLookup.Setup(r => r.FindPackagesById(It.IsAny<string>()))
                    .Returns<string>(
                        n =>
                        {
                            return knownPackages.Where(p => p.Id.Equals(n, StringComparison.OrdinalIgnoreCase));
                        })
                    .Verifiable();
            }

            Func<string, IPackageRepository> factory = s => repository.Object;

            var packageManager = new Mock<IPackageManager>();
            {
                packageManager.Setup(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .Verifiable();
            }

            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, o) => packageManager.Object;

            var installer = new PackageInstaller(
                configuration.Object,
                factory,
                managerBuilder,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            var installDirectory = @"c:\packages";

            var outputLocation = string.Empty;
            var packageInstallLocation = string.Empty;
            PackageName packageId = null;
            Action<string, string, PackageName> postInstallAction =
                (o, i, id) =>
                {
                    outputLocation = o;
                    packageInstallLocation = i;
                    packageId = id;
                };
            installer.Install(packageName, installDirectory, postInstallAction);

            packageLookup.Verify(r => r.FindPackagesById(It.IsAny<string>()), Times.Once());
            packageManager.Verify(p => p.InstallPackage(It.IsAny<IPackage>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());

            Assert.AreEqual(installDirectory, outputLocation);
            Assert.AreEqual(
                Path.Combine(
                    installDirectory,
                    string.Format(CultureInfo.InvariantCulture, "{0}.{1}", packageName.Id, packageName.Version)),
                packageInstallLocation);
            Assert.AreEqual(packageName, packageId);
        }
    }
}
