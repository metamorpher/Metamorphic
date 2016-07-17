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
using Metamorphic.Core;
using Metamorphic.Core.Rules;
using Metamorphic.Storage.Discovery;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Storage.Rules
{
    internal sealed class RulePackageDetector : IProcessPackageChanges
    {
        /// <summary>
        /// The file filter that can be used to find rule files.
        /// </summary>
        private const string RuleFileFilter = "*.mmrule";

        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
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
        /// The collection containing the rules.
        /// </summary>
        private readonly IStoreRules _ruleCollection;

        /// <summary>
        /// The object that loads rule object from the rule file.
        /// </summary>
        private readonly ILoadRules _ruleLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="RulePackageDetector"/> class.
        /// </summary>
        /// <param name="ruleCollection">The collection that contains all the rules.</param>
        /// <param name="ruleLoader">The object that loads <see cref="RuleDefinition"/> instances from a rule file.</param>
        /// <param name="packageInstaller"> The object that installs NuGet packages.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleCollection"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleLoader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageInstaller"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public RulePackageDetector(
            IStoreRules ruleCollection,
            ILoadRules ruleLoader,
            IInstallPackages packageInstaller,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (ruleCollection == null)
            {
                throw new ArgumentNullException("ruleCollection");
            }

            if (ruleLoader == null)
            {
                throw new ArgumentNullException("ruleLoader");
            }

            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
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
            _fileSystem = fileSystem;
            _packageInstaller = packageInstaller;
            _ruleCollection = ruleCollection;
            _ruleLoader = ruleLoader;
        }

        /// <summary>
        /// Processes the added packages.
        /// </summary>
        /// <param name="newPackages">The collection that contains the names of all the new packages.</param>
        public void Added(IEnumerable<PackageName> newPackages)
        {
            if (newPackages == null)
            {
                return;
            }

            if (!newPackages.Any())
            {
                return;
            }

            _diagnostics.Log(
                LevelToLog.Info,
                StorageConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RulePackageDetector_NewPackagesDetected_WithPackageList,
                    string.Join(
                        Environment.NewLine,
                        newPackages.Select(
                            p => string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} - {1}",
                                p.Id,
                                p.Version)))));

            foreach (var package in newPackages)
            {
                var tempDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());
                var rulePath = _fileSystem.Path.Combine(tempDirectory, "rules");
                if (!_fileSystem.Directory.Exists(rulePath))
                {
                    _diagnostics.Log(
                        LevelToLog.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_RulePackageDetector_CreatingRuleDirectory_WithPath,
                            rulePath));

                    _fileSystem.Directory.CreateDirectory(rulePath);
                }

                try
                {
                    _packageInstaller.Install(
                        package,
                        tempDirectory,
                        (outputLocation, path, id) => PackageUtilities.CopyPackageFilesToSinglePath(
                            path,
                            id,
                            RuleFileFilter,
                            rulePath,
                            _diagnostics,
                            _fileSystem));

                    foreach (var file in _fileSystem.Directory.GetFiles(rulePath, RuleFileFilter, SearchOption.AllDirectories))
                    {
                        _diagnostics.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_RulePackageDetector_FoundRuleFile_WithFilePath,
                                file));

                        var rule = _ruleLoader.LoadFromFile(file);
                        if (rule != null)
                        {
                            _ruleCollection.Add(new RuleOrigin(package), rule);
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

        /// <summary>
        /// Processes the removed packages.
        /// </summary>
        /// <param name="removedPackages">The collection that contains the names of all the packages that were removed.</param>
        public void Removed(IEnumerable<PackageName> removedPackages)
        {
            if (removedPackages == null)
            {
                return;
            }

            if (!removedPackages.Any())
            {
                return;
            }

            _diagnostics.Log(
                LevelToLog.Info,
                StorageConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RulePackageDetector_RemovedPackagesDetected_WithPackageList,
                    string.Join(
                        Environment.NewLine,
                        removedPackages.Select(
                            p => string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} - {1}",
                                p.Id,
                                p.Version)))));

            foreach (var package in removedPackages)
            {
                _ruleCollection.Remove(new RuleOrigin(package));
            }
        }
    }
}
