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
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Metamorphic.Core.Rules;
using Metamorphic.Storage.Properties;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Storage.Discovery.FileSystem
{
    /// <summary>
    /// Handles the detection of new, updated and removed rule files stored in a directory.
    /// </summary>
    internal sealed class DirectoryFileListener : IWatchPackages
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that provides a virtualizing layer for the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that processes changes to files.
        /// </summary>
        private readonly List<IProcessFileChanges> _scanners
            = new List<IProcessFileChanges>();

        /// <summary>
        /// The collection of objects that watch the file system for newly added packages.
        /// </summary>
        private readonly IDictionary<string, FileSystemWatcher> _watchers
            = new Dictionary<string, FileSystemWatcher>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileListener"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="packageScanners">The collection of objects that scan NuGet packages for components.</param>
        /// <param name="diagnostics">The object providing the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageScanners"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public DirectoryFileListener(
            IConfiguration configuration,
            IEnumerable<IProcessFileChanges> packageScanners,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (packageScanners == null)
            {
                throw new ArgumentNullException("packageScanners");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _scanners.AddRange(packageScanners);

            var fileSearchPaths = configuration.HasValueFor(RuleConfigurationKeys.RuleLocations)
                ? configuration.Value<string[]>(RuleConfigurationKeys.RuleLocations)
                : new[] { RuleConstants.DefaultRuleLocation };
            foreach (var path in fileSearchPaths)
            {
                var uri = new Uri(path);
                if (uri.IsFile || uri.IsUnc)
                {
                    var localPath = uri.LocalPath;

                    if (!_fileSystem.Path.IsPathRooted(localPath))
                    {
                        var exeDirectoryPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();
                        localPath = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(exeDirectoryPath, localPath));
                    }

                    if (!_watchers.ContainsKey(localPath))
                    {
                        var watcher = new FileSystemWatcher
                        {
                            Path = localPath,
                            Filter = "*.mmrule",
                            IncludeSubdirectories = true,
                            EnableRaisingEvents = false,
                            NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                        };

                        watcher.Created += HandleFileCreated;
                        watcher.Changed += HandleFileChanged;
                        watcher.Deleted += HandleFileDeleted;

                        _watchers.Add(localPath, watcher);
                    }
                }
            }
        }

        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        public void Disable()
        {
            foreach (var pair in _watchers)
            {
                pair.Value.EnableRaisingEvents = false;
            }

            _diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_DirectoryFileListener_FileDiscovery_Disabled);
        }

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        public void Enable()
        {
            _diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_DirectoryFileListener_FileDiscovery_Enabled);

            EnqueueExistingFiles();
            foreach (var pair in _watchers)
            {
                pair.Value.EnableRaisingEvents = true;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void EnqueueExistingFiles()
        {
            var newFiles = new List<string>();
            foreach (var file in _watchers.Keys.SelectMany(path => _fileSystem.Directory.GetFiles(path, "*.mmrule", SearchOption.AllDirectories)))
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_DirectoryFileListener_LocatedFile_WithFilePath,
                        file));

                newFiles.Add(file);
            }

            foreach (var scanner in _scanners)
            {
                try
                {
                    scanner.Added(newFiles);
                }
                catch (Exception e)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_DirectoryFileListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            string.Join(";", newFiles),
                            e));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileChanged(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_DirectoryFileListener_UpdatedFile_WithFilePath,
                    e.FullPath));

            var updatedPackages = new[] { e.FullPath };
            foreach (var scanner in _scanners)
            {
                try
                {
                    scanner.Removed(updatedPackages);
                    scanner.Added(updatedPackages);
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_DirectoryFileListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            string.Join(";", updatedPackages),
                            exception));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileCreated(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_DirectoryFileListener_CreatedFile_WithFilePath,
                    e.FullPath));

            var newPackages = new[] { e.FullPath };
            foreach (var scanner in _scanners)
            {
                try
                {
                    scanner.Added(newPackages);
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_DirectoryFileListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            string.Join(";", newPackages),
                            exception));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileDeleted(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_DirectoryFileListener_RemovedFile_WithFilePath,
                    e.FullPath));

            var removedPackages = new[] { e.FullPath };
            foreach (var scanner in _scanners)
            {
                try
                {
                    scanner.Removed(removedPackages);
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_DirectoryFileListener_DeletedFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            string.Join(";", removedPackages),
                            exception));
                }
            }
        }
    }
}
