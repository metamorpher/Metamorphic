//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Metamorphic.Core.Rules;
using Metamorphic.Server.Properties;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Server.Rules
{
    internal sealed class RuleWatcher : IWatchRules
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The collectino containing the rules.
        /// </summary>
        private readonly IStoreRules _ruleCollection;

        /// <summary>
        /// The object that loads rule object from the rule file.
        /// </summary>
        private readonly ILoadRules _ruleLoader;

        /// <summary>
        /// The object that watches the file system for newly added packages.
        /// </summary>
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleWatcher"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="ruleLoader">The object that loads <see cref="Rule"/> instances from a rule file.</param>
        /// <param name="ruleCollection">The collection that contains all the rules.</param>
        /// <param name="diagnostics">The object providing the diagnostics methods for the application.</param>
        internal RuleWatcher(
            IConfiguration configuration,
            ILoadRules ruleLoader,
            IStoreRules ruleCollection,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => ruleLoader);
                Lokad.Enforce.Argument(() => ruleCollection);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            _diagnostics = diagnostics;
            _ruleCollection = ruleCollection;
            _ruleLoader = ruleLoader;

            var uploadPath = string.Empty;
            uploadPath = configuration.HasValueFor(ServerConfigurationKeys.RuleDirectory)
                ? configuration.Value<string>(ServerConfigurationKeys.RuleDirectory)
                : ServerConstants.DefaultRuleDirectory;
            if (!Path.IsPathRooted(uploadPath))
            {
                var exeDirectoryPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();
                uploadPath = Path.GetFullPath(Path.Combine(exeDirectoryPath, uploadPath));
            }

            _watcher = new FileSystemWatcher
            {
                Path = uploadPath,
                Filter = "*.mmrule",
                IncludeSubdirectories = true,
                EnableRaisingEvents = false,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
            };

            _watcher.Created += HandleFileCreated;
            _watcher.Changed += HandleFileChanged;
            _watcher.Deleted += HandleFileDeleted;
        }

        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        public void Disable()
        {
            _watcher.EnableRaisingEvents = false;
            _diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_RuleWatcher_RuleDiscovery_Disabled);
        }

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        public void Enable()
        {
            _diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_RuleWatcher_RuleDiscovery_Enabled);

            EnqueueExistingFiles();
            _watcher.EnableRaisingEvents = true;
        }

        private void EnqueueExistingFiles()
        {
            foreach (var file in Directory.GetFiles(_watcher.Path, _watcher.Filter, SearchOption.AllDirectories))
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleWatcher_CreatedFile_WithFilePath,
                        file));

                var rule = _ruleLoader.Load(file);
                _ruleCollection.Add(file, rule);
            }
        }

        private void HandleFileChanged(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_UpdatedFile_WithFilePath,
                    e.FullPath));

            _ruleCollection.Remove(e.FullPath);

            var rule = _ruleLoader.Load(e.FullPath);
            if (rule != null)
            {
                _ruleCollection.Update(e.FullPath, rule);
            }
        }

        private void HandleFileCreated(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_CreatedFile_WithFilePath,
                    e.FullPath));

            var rule = _ruleLoader.Load(e.FullPath);
            if (rule != null)
            {
                _ruleCollection.Add(e.FullPath, rule);
            }
        }

        private void HandleFileDeleted(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_RemovedFile_WithFilePath,
                    e.FullPath));

            _ruleCollection.Remove(e.FullPath);
        }
    }
}
