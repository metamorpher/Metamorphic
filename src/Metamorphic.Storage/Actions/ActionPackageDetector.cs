//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    internal sealed class ActionPackageDetector : IDetectActionPackages
    {
        /// <summary>
        /// The object that stores information about all the parts and the part groups.
        /// </summary>
        private readonly IStoreActions _repository;

        /// <summary>
        /// The function that returns a reference to an assembly scanner which has been
        /// created in the given AppDomain.
        /// </summary>
        private readonly Func<IStoreActions, IScanActionPackages> _scannerBuilder;

        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPackageDetector"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the parts and the part groups.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public ActionPackageDetector(
            IStoreActions repository,
            Func<IStoreActions, IScanActionPackages> scannerBuilder,
            SystemDiagnostics systemDiagnostics)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (scannerBuilder == null)
            {
                throw new ArgumentNullException("scannerBuilder");
            }

            if (systemDiagnostics == null)
            {
                throw new ArgumentNullException("systemDiagnostics");
            }

            _repository = repository;
            _scannerBuilder = scannerBuilder;
            _diagnostics = systemDiagnostics;
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
                    Resources.Log_Messages_ActionPackageDetector_NewPackagesDetected_WithPackageList,
                    string.Join(
                        Environment.NewLine,
                        newPackages.Select(
                            p => string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} - {1}",
                                p.Id,
                                p.Version)))));

            var scanner = _scannerBuilder(_repository);
            scanner.Scan(newPackages);
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
                    Resources.Log_Messages_ActionPackageDetector_RemovedPackagesDetected_WithPackageList,
                    string.Join(
                        Environment.NewLine,
                        removedPackages.Select(
                            p => string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} - {1}",
                                p.Id,
                                p.Version)))));

            _repository.RemovePackages(removedPackages);
        }
    }
}
