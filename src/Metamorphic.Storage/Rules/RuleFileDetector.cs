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
using Metamorphic.Core.Rules;
using Metamorphic.Storage.Discovery;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Storage.Rules
{
    internal sealed class RuleFileDetector : IProcessFileChanges
    {
        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The collection containing the rules.
        /// </summary>
        private readonly IStoreRules _ruleCollection;

        /// <summary>
        /// The object that loads rule object from the rule file.
        /// </summary>
        private readonly ILoadRules _ruleLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleFileDetector"/> class.
        /// </summary>
        /// <param name="ruleCollection">The collection that contains all the rules.</param>
        /// <param name="ruleLoader">The object that loads <see cref="RuleDefinition"/> instances from a rule file.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleCollection"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleLoader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public RuleFileDetector(
            IStoreRules ruleCollection,
            ILoadRules ruleLoader,
            SystemDiagnostics diagnostics)
        {
            if (ruleCollection == null)
            {
                throw new ArgumentNullException("ruleCollection");
            }

            if (ruleLoader == null)
            {
                throw new ArgumentNullException("ruleLoader");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _ruleCollection = ruleCollection;
            _ruleLoader = ruleLoader;
        }

        /// <summary>
        /// Processes the added files.
        /// </summary>
        /// <param name="newFiles">The collection that contains the names of all the new files.</param>
        public void Added(IEnumerable<string> newFiles)
        {
            if (newFiles == null)
            {
                return;
            }

            if (!newFiles.Any())
            {
                return;
            }

            foreach (var file in newFiles)
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleFileDetector_FoundRuleFile_WithFilePath,
                        file));

                var rule = _ruleLoader.LoadFromFile(file);
                if (rule != null)
                {
                    _ruleCollection.Add(new RuleOrigin(new FileInfo(file)), rule);
                }
            }
        }

        /// <summary>
        /// Processes the removed files.
        /// </summary>
        /// <param name="removedFiles">The collection that contains the names of all the files that were removed.</param>
        public void Removed(IEnumerable<string> removedFiles)
        {
            if (removedFiles == null)
            {
                return;
            }

            if (!removedFiles.Any())
            {
                return;
            }

            foreach (var file in removedFiles)
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleFileDetector_RemovedRuleFile_WithFilePath,
                        file));

                _ruleCollection.Remove(new RuleOrigin(new FileInfo(file)));
            }
        }
    }
}
