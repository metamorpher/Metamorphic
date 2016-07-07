//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using Metamorphic.Core.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core
{
    /// <summary>
    /// Defines utility methods for dealing with installed NuGet packages.
    /// </summary>
    public static class PackageUtilities
    {
        /// <summary>
        /// Copies a set of files from an installed NuGet package to a given destination directory.
        /// </summary>
        /// <param name="packageInstallPath">The path to the installed package.</param>
        /// <param name="id">The ID of the installed package.</param>
        /// <param name="fileSearchPattern">The search pattern that will be used to locate the desired files.</param>
        /// <param name="destinationPath">The full path to the directory to which the files should be copied.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageInstallPath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="packageInstallPath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSearchPattern"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="fileSearchPattern"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="destinationPath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="destinationPath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public static void CopyPackageFilesToSinglePath(
            string packageInstallPath,
            NuGet.PackageName id,
            string fileSearchPattern,
            string destinationPath,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (packageInstallPath == null)
            {
                throw new ArgumentNullException("packageInstallPath");
            }

            if (string.IsNullOrWhiteSpace(packageInstallPath))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "packageInstallPath");
            }

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (fileSearchPattern == null)
            {
                throw new ArgumentNullException("fileSearchPattern");
            }

            if (string.IsNullOrWhiteSpace(fileSearchPattern))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "fileSearchPattern");
            }

            if (destinationPath == null)
            {
                throw new ArgumentNullException("destinationPath");
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "destinationPath");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            foreach (var assemblyFile in fileSystem.Directory.GetFiles(packageInstallPath, fileSearchPattern, SearchOption.AllDirectories))
            {
                var destination = fileSystem.Path.Combine(destinationPath, fileSystem.Path.GetFileName(assemblyFile));
                if (!fileSystem.File.Exists(destination))
                {
                    diagnostics.Log(
                        LevelToLog.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_PackageUtilities_CopyingAssemblyFile_WithPackageIdAndVersionAndOriginAndDestination,
                            id.Name,
                            id.Version,
                            assemblyFile,
                            destination));

                    fileSystem.File.Copy(assemblyFile, destination);
                }
                else
                {
                    diagnostics.Log(
                        LevelToLog.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_PackageUtilities_AssemblyFileAlreadyExistsAtDestination_WithPackageIdAndVersionAndOriginAndDestination,
                            id.Name,
                            id.Version,
                            assemblyFile,
                            destination));
                }
            }
        }
    }
}
