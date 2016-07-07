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
using System.Linq;
using System.Reflection;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics.Logging;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Performs assembly scanning in search for plugin information.
    /// </summary>
    internal sealed class RemotePackageScanner : MarshalByRefObject, IScanActionPackageFiles
    {
        private static IEnumerable<ActionDefinition> ExtractActions(string packageName, string packageVersion, Assembly assembly)
        {
            var result = new List<ActionDefinition>();
            foreach (var type in assembly.GetTypes())
            {
                var actionProviderAttribute = type.GetCustomAttribute(typeof(ActionProviderAttribute));
                if (actionProviderAttribute == null)
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    var attribute = (ActionAttribute)method.GetCustomAttribute(typeof(ActionAttribute));
                    if (attribute == null)
                    {
                        continue;
                    }

                    var parameters = method.GetParameters()
                        .OrderBy(p => p.Position)
                        .Select(p => new ActionParameterDefinition(p.Name))
                        .ToArray();

                    var id = attribute.Id;
                    var def = new ActionDefinition(
                        id,
                        packageName,
                        packageVersion,
                        type.AssemblyQualifiedName,
                        method.Name,
                        parameters);
                    result.Add(def);
                }
            }

            return result;
        }

        /// <summary>
        /// The object that will pass through the log messages.
        /// </summary>
        private readonly ILogMessagesFromRemoteAppDomains _logger;

        /// <summary>
        /// The object that stores all the information about the parts and the part groups.
        /// </summary>
        private readonly IStoreActions _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotePackageScanner"/> class.
        /// </summary>
        /// <param name="repository">The object that stores all the information about the parts and the part groups.</param>
        /// <param name="logger">The object that passes through the log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public RemotePackageScanner(IStoreActions repository, ILogMessagesFromRemoteAppDomains logger)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _repository = repository;
            _logger = logger;
        }

        private Assembly LoadAssembly(string file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.Length == 0)
            {
                return null;
            }

            // Check if the file exists.
            if (!File.Exists(file))
            {
                return null;
            }

            // Try to load the assembly. If we can't load the assembly
            // we log the exception / problem and return a null reference
            // for the assembly.
            string fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                // Only use the file name of the assembly
                return Assembly.Load(fileName);
            }
            catch (FileNotFoundException e)
            {
                // The file does not exist. Only possible if somebody removes the file
                // between the check and the loading.
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (FileLoadException e)
            {
                // Could not load the assembly.
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (BadImageFormatException e)
            {
                // incorrectly formatted assembly.
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }

            return null;
        }

        /// <summary>
        /// Scans the packages for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="packageName">The name of the NuGet package from which the files were retrieved.</param>
        /// <param name="packageVersion">The version of the NuGet package from which the files were retrieved.</param>
        /// <param name="filesToScan">
        /// The collection that contains the file paths to all the packages to be scanned.
        /// </param>
        public void Scan(string packageName, string packageVersion, IEnumerable<string> filesToScan)
        {
            if (string.IsNullOrWhiteSpace(packageName) || string.IsNullOrWhiteSpace(packageVersion))
            {
                _logger.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_AssemblyScanWithoutPackageName_WithFiles,
                        filesToScan.Join("; ")));

                return;
            }

            if ((filesToScan == null) || (!filesToScan.Any()))
            {
                _logger.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_AssemblyScanWithoutFiles_WithPackageInfo,
                        packageName,
                        packageVersion));

                return;
            }

            foreach (var assemblyFile in filesToScan)
            {
                var assembly = LoadAssembly(assemblyFile);
                ScanAssembly(packageName, packageVersion, assembly);
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Will catch an log here because we don't actually know what exceptions can happen due to the ExtractGroups() call.")]
        private void ScanAssembly(string packageName, string packageVersion, Assembly assembly)
        {
            try
            {
                _logger.Log(
                    LevelToLog.Trace,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_ScanningAssembly_WithName,
                        assembly.FullName));

                var actions = ExtractActions(packageName, packageVersion, assembly);
                foreach (var action in actions)
                {
                    try
                    {
                        _logger.Log(
                            LevelToLog.Trace,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_RemotePackageScanner_RegisteringAction_WithActionInformation,
                                action.Id,
                                action.Package.Id,
                                action.Package.Version));

                        _repository.Add(action);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LevelToLog.Warn, e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemotePackageScanner_TypeScanFailed_WithAssemblyAndException,
                        assembly.GetName().FullName,
                        e));
            }
        }
    }
}
