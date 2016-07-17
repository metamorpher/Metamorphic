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
using Metamorphic.Core;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Provides an <see cref="IScanActionPackages"/> wrapper that loads the actual scanner into a <c>AppDomain</c>, provides the data
    /// to that scanner and then unloads the <c>AppDomain</c> when the scanning process is complete.
    /// </summary>
    internal sealed class AppDomainOwningActionPackageScanner : IScanActionPackages
    {
        /// <summary>
        /// The function that builds an <c>AppDomain</c> when requested.
        /// </summary>
        private readonly Func<string, string[], AppDomain> _appDomainBuilder;

        /// <summary>
        /// The object that provides the diagnostics for the system.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that provides a virtualizing layer for the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that installs NuGet packages.
        /// </summary>
        private readonly IInstallPackages _packageInstaller;

        /// <summary>
        /// The object that stores information about all the known packages.
        /// </summary>
        private readonly IStoreActions _repository;

        /// <summary>
        /// The function that is used to create a new <see cref="IScanActionPackageFiles"/> instance in a remote <see cref="AppDomain"/>.
        /// </summary>
        private readonly Func<AppDomain, ILoadPackageScannersInRemoteAppDomains> _scannerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainOwningActionPackageScanner"/> class.
        /// </summary>
        /// <param name="packageInstaller"> The object that installs NuGet packages.</param>
        /// <param name="appDomainBuilder">The function that is used to create a new <c>AppDomain</c> which will be used to scan action packages.</param>
        /// <param name="scannerBuilder">The function that is used to create a new <see cref="IScanActionPackageFiles"/> instance in a remote <see cref="AppDomain"/>.</param>
        /// <param name="repository">The object that contains the information about all the known packages.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageInstaller"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appDomainBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public AppDomainOwningActionPackageScanner(
            IInstallPackages packageInstaller,
            Func<string, string[], AppDomain> appDomainBuilder,
            Func<AppDomain, ILoadPackageScannersInRemoteAppDomains> scannerBuilder,
            IStoreActions repository,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
            }

            if (appDomainBuilder == null)
            {
                throw new ArgumentNullException("appDomainBuilder");
            }

            if (scannerBuilder == null)
            {
                throw new ArgumentNullException("scannerBuilder");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            _appDomainBuilder = appDomainBuilder;
            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _packageInstaller = packageInstaller;
            _repository = repository;
            _scannerBuilder = scannerBuilder;
        }

        /// <summary>
        /// Scans the packages for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="packagesToScan">
        /// The collection that contains the NuGet packages to be scanned.
        /// </param>
        public void Scan(IEnumerable<PackageName> packagesToScan)
        {
            if (packagesToScan == null)
            {
                return;
            }

            foreach (var package in packagesToScan)
            {
                var tempDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());
                var binPath = _fileSystem.Path.Combine(tempDirectory, "bin");
                if (!_fileSystem.Directory.Exists(binPath))
                {
                    _diagnostics.Log(
                        LevelToLog.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_PackageScanner_CreatingBinDirectory_WithPath,
                            binPath));

                    _fileSystem.Directory.CreateDirectory(binPath);
                }

                try
                {
                    _packageInstaller.Install(
                        package,
                        tempDirectory,
                        (outputLocation, path, id) => PackageUtilities.CopyPackageFilesToSinglePath(
                            path,
                            id,
                            "*.dll",
                            binPath,
                            _diagnostics,
                            _fileSystem));

                    var domain = _appDomainBuilder(Resources.ActionScanDomainName, new string[] { binPath });
                    try
                    {
                        // Inject the actual scanner
                        var loader = _scannerBuilder(domain);
                        var logger = new LogForwardingPipe(_diagnostics);
                        var repositoryProxy = new ActionStorageProxy(_repository);
                        var scannerProxy = loader.Load(repositoryProxy, logger);
                        scannerProxy.Scan(
                            package.Id,
                            package.Version.ToString(),
                            _fileSystem.Directory.GetFiles(binPath, "*.dll", SearchOption.TopDirectoryOnly));
                    }
                    finally
                    {
                        if ((domain != null) && (!AppDomain.CurrentDomain.Equals(domain)))
                        {
                            AppDomain.Unload(domain);
                        }
                    }
                }
                finally
                {
                    if (_fileSystem.Directory.Exists(tempDirectory))
                    {
                        _fileSystem.Directory.Delete(tempDirectory, true);
                    }
                }
            }
        }
    }
}
