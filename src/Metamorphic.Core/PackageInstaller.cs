//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Metamorphic.Core.Properties;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Core
{
    /// <summary>
    /// Provides methods for installing a NuGet package to a given location.
    /// </summary>
    internal sealed class PackageInstaller : IInstallPackages
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The collection of NuGet feeds that may be used to find a given package.
        /// </summary>
        private readonly IEnumerable<Uri> _feeds;

        /// <summary>
        /// The object that provides a virtualizing layer for the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The function that creates a <see cref="IPackageManager"/> for a given repository and an output directory.
        /// </summary>
        private readonly Func<IPackageRepository, string, IPackageManager> _managerFactory;

        /// <summary>
        /// The function that creates a <see cref="IPackageRepository"/> for a given feed.
        /// </summary>
        private readonly Func<string, IPackageRepository> _repositoryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageInstaller"/> class.
        /// </summary>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="repositoryFactory">The function that creates a <see cref="IPackageRepository"/> for a given feed.</param>
        /// <param name="managerFactory">The function that creates a <see cref="IPackageManager"/> for a given repository and an output directory.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repositoryFactory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="managerFactory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public PackageInstaller(
            IConfiguration configuration,
            Func<string, IPackageRepository> repositoryFactory,
            Func<IPackageRepository, string, IPackageManager> managerFactory,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentNullException("repositoryFactory");
            }

            if (managerFactory == null)
            {
                throw new ArgumentNullException("managerFactory");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            _diagnostics = diagnostics;
            _feeds = (configuration.HasValueFor(CoreConfigurationKeys.NugetFeeds)
                    ? configuration.Value<string[]>(CoreConfigurationKeys.NugetFeeds)
                    : new string[0])
                .Select(f => new Uri(f));
            _fileSystem = fileSystem;
            _managerFactory = managerFactory;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Installs a given version of a package and its dependencies.
        /// </summary>
        /// <param name="name">The ID of the package.</param>
        /// <param name="outputLocation">The full path of the directory where the packages should be installed.</param>
        /// <param name="postInstallAction">
        /// An action that is run after each package is installed. The input values are the <paramref name="outputLocation"/>,
        /// the path to the installed package and the package ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="outputLocation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="outputLocation"/> is an empty string.
        /// </exception>
        public void Install(
            PackageName name,
            string outputLocation,
            Action<string, string, PackageName> postInstallAction = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (outputLocation == null)
            {
                throw new ArgumentNullException("outputLocation");
            }

            if (string.IsNullOrWhiteSpace(outputLocation))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "outputLocation");
            }

            if (!_fileSystem.Directory.Exists(outputLocation))
            {
                _fileSystem.Directory.CreateDirectory(outputLocation);
            }

            var repositories = new Dictionary<Uri, IPackageRepository>();
            foreach (var feed in _feeds)
            {
                var repo = _repositoryFactory(feed.OriginalString);
                repositories.Add(feed, repo);
            }

            var packagesToInstall = new List<Tuple<IPackage, IPackageRepository>>();

            var packageQueue = new Queue<PackageName>();
            packageQueue.Enqueue(name);
            while (packageQueue.Count > 0)
            {
                var packageName = packageQueue.Dequeue();
                var map = repositories.Values.SelectMany(r => r.FindPackagesById(packageName.Id).Select(p => new Tuple<IPackage, IPackageRepository>(p, r)))
                    .Where(t => t.Item1.Version.Equals(packageName.Version))
                    .FirstOrDefault();
                if (map == null)
                {
                    continue;
                }

                packagesToInstall.Add(map);

                var set = map.Item1.DependencySets.FirstOrDefault();
                if (set != null)
                {
                    foreach (var dep in set.Dependencies)
                    {
                        var dependencyPair = repositories.Values.SelectMany(r => r.FindPackagesById(dep.Id).Select(p => new Tuple<IPackage, IPackageRepository>(p, r)))
                            .Where(p => dep.VersionSpec.Satisfies(p.Item1.Version))
                            .OrderBy(t => t.Item1.Version)
                            .FirstOrDefault();

                         packageQueue.Enqueue(new PackageName(dependencyPair.Item1.Id, dependencyPair.Item1.Version));
                    }
                }
            }

            foreach (var map in packagesToInstall)
            {
                var package = map.Item1;
                var repo = map.Item2;

                _diagnostics.Log(
                    LevelToLog.Debug,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_PackageInstaller_InstallingPackage_WithIdVersionAndRepository,
                        package.Id,
                        package.Version,
                        repo.Source));
                IPackageManager packageManager = _managerFactory(repo, outputLocation);
                packageManager.InstallPackage(package, true, true);

                postInstallAction?.Invoke(
                    outputLocation,
                    _fileSystem.Path.Combine(
                        outputLocation,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.{1}",
                            package.Id,
                            package.Version)),
                    new PackageName(package.Id, package.Version));
            }
        }
    }
}
