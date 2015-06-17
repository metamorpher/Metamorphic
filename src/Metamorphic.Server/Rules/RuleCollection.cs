//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metamorphic.Core.Rules;
using Nuclei.Diagnostics;

namespace Metamorphic.Server.Rules
{
    internal sealed class RuleCollection : IStoreRules, ILoadRules
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The object that watches the file system for newly added packages.
        /// </summary>
        private readonly FileSystemWatcher m_Watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCollection"/> class.
        /// </summary>
        /// <param name="packageQueue">The object that queues packages that need to be processed.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="diagnostics">The object providing the diagnostics methods for the application.</param>
        internal RuleCollection(
            IQueueSymbolPackages packageQueue,
            IConfiguration configuration,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => packageQueue);
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => diagnostics);

                Lokad.Enforce.With<ArgumentException>(
                    configuration.HasValueFor(CoreConfigurationKeys.s_UploadPath),
                    Resources.Exceptions_Messages_MissingConfigurationValue_WithKey,
                    CoreConfigurationKeys.s_UploadPath);
            }

            m_Queue = packageQueue;
            m_Diagnostics = diagnostics;

            var uploadPath = configuration.Value<string>(CoreConfigurationKeys.s_UploadPath);
            m_Watcher = new FileSystemWatcher
            {
                Path = uploadPath,
                Filter = "*.mmrule",
                IncludeSubdirectories = true,
                EnableRaisingEvents = false,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
            };

            m_Watcher.Created += HandleFileCreated;
        }

        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        public void Disable()
        {
            m_Watcher.EnableRaisingEvents = false;
            m_Diagnostics.Log(
                    LevelToLog.Info,
                    Resources.Log_Messages_FileWatcherBasedPackageUploader_PackageDiscovery_Disabled);
        }

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        public void Enable()
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                Resources.Log_Messages_FileWatcherBasedPackageUploader_PackageDiscovery_Enabled);

            EnqueueExistingFiles();
            m_Watcher.EnableRaisingEvents = true;
        }

        private void EnqueueExistingFiles()
        {
            foreach (var file in Directory.GetFiles(m_Watcher.Path, m_Watcher.Filter, SearchOption.AllDirectories))
            {
                ProcessRuleFile(file);
            }
        }

        private void HandleFileCreated(object sender, FileSystemEventArgs e)
        {
            if ((e.ChangeType == WatcherChangeTypes.Created) || (e.ChangeType == WatcherChangeTypes.Changed))
            {
                ProcessRuleFile(e.FullPath);
            }

            var renamedArgs = e as RenamedEventArgs;
            if ((e.ChangeType == WatcherChangeTypes.Renamed) && (renamedArgs != null))
            {
                RemoveRuleByPath(renamedArgs.OldFullPath);
                ProcessRuleFile(e.FullPath);
            }

            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                RemoveRuleByPath(e.FullPath);
            }
        }

        private void ProcessRuleFile(string filePath)
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_FileWatcherBasedPackageUploader_DiscoveredFile_WithFilePath,
                    file));
        }

        private void RemoveRuleByPath(string filePath)
        {
        }

        public IEnumerable<Rule> RulesForSignal(string signalType)
        {
            throw new NotImplementedException();
        }
    }
}
