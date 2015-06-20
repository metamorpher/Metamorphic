//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using Metamorphic.Core.Rules;
using Metamorphic.Server.Properties;
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
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The collectino containing the rules.
        /// </summary>
        private readonly IStoreRules m_RuleCollection;

        /// <summary>
        /// The object that loads rule object from the rule file.
        /// </summary>
        private readonly ILoadRules m_RuleLoader;

        /// <summary>
        /// The object that watches the file system for newly added packages.
        /// </summary>
        private readonly FileSystemWatcher m_Watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCollection"/> class.
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

                Lokad.Enforce.With<ArgumentException>(
                    configuration.HasValueFor(ServerConfigurationKeys.s_RulePath),
                    Resources.Exceptions_Messages_MissingConfigurationValue_WithKey,
                    ServerConfigurationKeys.s_RulePath);
            }

            m_Diagnostics = diagnostics;
            m_RuleCollection = ruleCollection;
            m_RuleLoader = ruleLoader;

            var uploadPath = configuration.Value<string>(ServerConfigurationKeys.s_RulePath);
            m_Watcher = new FileSystemWatcher
            {
                Path = uploadPath,
                Filter = "*.mmrule",
                IncludeSubdirectories = true,
                EnableRaisingEvents = false,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
            };

            m_Watcher.Created += HandleFileCreated;
            m_Watcher.Changed += HandleFileChanged;
            m_Watcher.Deleted += HandleFileDeleted;
        }

        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        public void Disable()
        {
            m_Watcher.EnableRaisingEvents = false;
            m_Diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_RuleWatcher_RuleDiscovery_Disabled);
        }

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        public void Enable()
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_RuleWatcher_RuleDiscovery_Enabled);

            EnqueueExistingFiles();
            m_Watcher.EnableRaisingEvents = true;
        }

        private void EnqueueExistingFiles()
        {
            foreach (var file in Directory.GetFiles(m_Watcher.Path, m_Watcher.Filter, SearchOption.AllDirectories))
            {
                m_Diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleWatcher_CreatedFile_WithFilePath,
                        file));

                var rule = m_RuleLoader.Load(file);
                m_RuleCollection.Add(file, rule);
            }
        }

        private void HandleFileChanged(object sender, FileSystemEventArgs e)
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_UpdatedFile_WithFilePath,
                    e.FullPath));

            m_RuleCollection.Remove(e.FullPath);

            var rule = m_RuleLoader.Load(e.FullPath);
            m_RuleCollection.Update(e.FullPath, rule);
        }

        private void HandleFileCreated(object sender, FileSystemEventArgs e)
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_CreatedFile_WithFilePath,
                    e.FullPath));

            var rule = m_RuleLoader.Load(e.FullPath);
            m_RuleCollection.Add(e.FullPath, rule);
        }

        private void HandleFileDeleted(object sender, FileSystemEventArgs e)
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_RuleWatcher_RemovedFile_WithFilePath,
                    e.FullPath));

            m_RuleCollection.Remove(e.FullPath);
        }
    }
}
