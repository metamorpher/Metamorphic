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
using System.Reflection;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Storage.Properties;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Storage.Actions
{
    internal sealed class DirectoryPackageListener : IWatchPackages
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
        /// The object that loads action objects from the NuGet packages.
        /// </summary>
        private readonly IDetectActionPackages _packageScanner;

        /// <summary>
        /// The collection of objects that watch the file system for newly added packages.
        /// </summary>
        private readonly IDictionary<string, FileSystemWatcher> _watchers
            = new Dictionary<string, FileSystemWatcher>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryPackageListener"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="actionLoader">The object that loads <see cref="ActionDefinition"/> instances from a NuGet package file.</param>
        /// <param name="diagnostics">The object providing the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionLoader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        internal DirectoryPackageListener(
            IConfiguration configuration,
            IDetectActionPackages actionLoader,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (actionLoader == null)
            {
                throw new ArgumentNullException("actionLoader");
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
            _packageScanner = actionLoader;

            var packagePaths = configuration.HasValueFor(CoreConfigurationKeys.NugetFeeds)
                ? configuration.Value<string[]>(CoreConfigurationKeys.NugetFeeds)
                : new[] { CoreConstants.DefaultFeedDirectory };
            foreach (var path in packagePaths)
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
                            Filter = "*.nupkg",
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
                Resources.Log_Messages_DirectoryPackageListener_PackageDiscovery_Disabled);
        }

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        public void Enable()
        {
            _diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_DirectoryPackageListener_PackageDiscovery_Enabled);

            EnqueueExistingFiles();
            foreach (var pair in _watchers)
            {
                pair.Value.EnableRaisingEvents = true;
            }
        }

        private void EnqueueExistingFiles()
        {
            var newPackages = new List<PackageName>();
            foreach (var file in _watchers.Keys.SelectMany(path => _fileSystem.Directory.GetFiles(path, "*.nupkg", SearchOption.AllDirectories)))
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleWatcher_CreatedFile_WithFilePath,
                        file));

                var zipPackage = new ZipPackage(file);
                newPackages.Add(new PackageName(zipPackage.Id, zipPackage.Version));
            }

            _packageScanner.Added(newPackages);
        }

        private void HandleFileChanged(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_UpdatedFile_WithFilePath,
                    e.FullPath));

            var zipPackage = new ZipPackage(e.FullPath);
            _packageScanner.Removed(new[] { new PackageName(zipPackage.Id, zipPackage.Version) });
            _packageScanner.Added(new[] { new PackageName(zipPackage.Id, zipPackage.Version) });
        }

        private void HandleFileCreated(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_CreatedFile_WithFilePath,
                    e.FullPath));

            var zipPackage = new ZipPackage(e.FullPath);
            _packageScanner.Added(new[] { new PackageName(zipPackage.Id, zipPackage.Version) });
        }

        private void HandleFileDeleted(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_RemovedFile_WithFilePath,
                    e.FullPath));

            var zipPackage = new ZipPackage(e.FullPath);
            _packageScanner.Removed(new[] { new PackageName(zipPackage.Id, zipPackage.Version) });
        }
    }
}
