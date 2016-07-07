//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Properties;
using Metamorphic.Server.Rules;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Server
{
    internal sealed class SignalProcessor
    {
        /// <summary>
        /// The function that builds an <c>AppDomain</c> when requested.
        /// </summary>
        private readonly Func<string, string[], AppDomain> _appDomainBuilder;

        /// <summary>
        /// The object that forms a proxy for the remote action storage.
        /// </summary>
        private readonly IActionStorageProxy _actionStorage;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The function that is used to create a new <see cref="IExecuteActions"/> instance in a remote <see cref="AppDomain"/>.
        /// </summary>
        private readonly Func<AppDomain, ILoadActionExecutorsInRemoteAppDomains> _executorBuilder;

        /// <summary>
        /// The object that provides a virtualizing layer for the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that installs NuGet packages.
        /// </summary>
        private readonly IInstallPackages _packageInstaller;

        /// <summary>
        /// The collection containing all the rules.
        /// </summary>
        private readonly IStoreRules _ruleCollection;

        /// <summary>
        /// The object that dispenses signals that need to be processed.
        /// </summary>
        private readonly IDispenseSignals _signalDispenser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalProcessor"/> class.
        /// </summary>
        /// <param name="actionStorage">The object that forms a proxy for the remote action storage.</param>
        /// <param name="packageInstaller"> The object that installs NuGet packages.</param>
        /// <param name="appDomainBuilder">The function that is used to create a new <c>AppDomain</c> which will be used to scan action packages.</param>
        /// <param name="executorBuilder">The function that is used to create a new <see cref="IExecuteActions"/> instance in a remote <see cref="AppDomain"/>.</param>
        /// <param name="ruleCollection">The object that stores all the known rules for the application.</param>
        /// <param name="signalDispenser">The object that dispenses signals that need to be processed.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageInstaller"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appDomainBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executorBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleCollection"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalDispenser"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public SignalProcessor(
            IActionStorageProxy actionStorage,
            IInstallPackages packageInstaller,
            Func<string, string[], AppDomain> appDomainBuilder,
            Func<AppDomain, ILoadActionExecutorsInRemoteAppDomains> executorBuilder,
            IStoreRules ruleCollection,
            IDispenseSignals signalDispenser,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (actionStorage == null)
            {
                throw new ArgumentNullException("actionStorage");
            }

            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
            }

            if (appDomainBuilder == null)
            {
                throw new ArgumentNullException("appDomainBuilder");
            }

            if (executorBuilder == null)
            {
                throw new ArgumentNullException("executorBuilder");
            }

            if (ruleCollection == null)
            {
                throw new ArgumentNullException("ruleCollection");
            }

            if (signalDispenser == null)
            {
                throw new ArgumentNullException("signalDispenser");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _appDomainBuilder = appDomainBuilder;
            _actionStorage = actionStorage;
            _diagnostics = diagnostics;
            _executorBuilder = executorBuilder;
            _fileSystem = fileSystem;
            _packageInstaller = packageInstaller;
            _ruleCollection = ruleCollection;
            _signalDispenser = signalDispenser;
            _signalDispenser.OnItemAvailable += HandleOnEnqueue;
        }

        private void HandleOnEnqueue(object sender, ItemEventArgs<Signal> e)
        {
            Signal signal = e.Item;
            if (signal == null)
            {
                return;
            }

            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_SignalProcessor_ProcessSignal_WithType,
                    signal.Sensor));

            foreach (var parameter in signal.Parameters())
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_SignalProcessor_ProcessSignal_ForParameters,
                        parameter,
                        signal.ParameterValue(parameter)));
            }

            var rules = _ruleCollection.RulesForSignal(signal.Sensor);
            foreach (var rule in rules)
            {
                if (rule.ShouldProcess(signal))
                {
                    var job = rule.ToJob(signal);
                    ProcessJob(job);
                }
            }
        }

        private string InstallActionPackages(PackageName packageId, string tempDirectory)
        {
            var binPath = _fileSystem.Path.Combine(tempDirectory, "bin");
            if (!_fileSystem.Directory.Exists(binPath))
            {
                _diagnostics.Log(
                    LevelToLog.Debug,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_SignalProcessor_CreatingBinDirectory_WithPath,
                        binPath));

                _fileSystem.Directory.CreateDirectory(binPath);
            }

            _packageInstaller.Install(
                packageId,
                tempDirectory,
                (outputLocation, path, id) => PackageUtilities.CopyPackageFilesToSinglePath(
                    path,
                    id,
                    "*.dll",
                    binPath,
                    _diagnostics,
                    _fileSystem));

            return binPath;
        }

        private void InvokeActionInSeparateAppdomain(Job job, ActionDefinition action, string packageInstallPath)
        {
            var domain = _appDomainBuilder(Resources.ActionExecuteDomainName, new string[] { packageInstallPath });
            try
            {
                // Inject the actual scanner
                var loader = _executorBuilder(domain);

                var logger = new LogForwardingPipe(_diagnostics);
                var executorProxy = loader.Load(logger);
                executorProxy.Execute(job, action);
            }
            finally
            {
                if ((domain != null) && (!AppDomain.CurrentDomain.Equals(domain)))
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching to prevent a failure from killing the processor. Would catch more specific exceptions if we knew what they would be.")]
        private void ProcessJob(Job job)
        {
            if (job == null)
            {
                return;
            }

            try
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_SignalProcessor_ProcessJob_WithId,
                        job.Action));

                foreach (var parameter in job.ParameterNames())
                {
                    _diagnostics.Log(
                        LevelToLog.Info,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_SignalProcessor_ProcessJob_ForParameters,
                            parameter,
                            job.ParameterValue(parameter)));
                }

                ActionDefinition action = _actionStorage.Action(job.Action);
                if (action == null)
                {
                    _diagnostics.Log(
                        LevelToLog.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_SignalProcessor_ProcessJob_ActionIdNotFound_WithId,
                            job.Action));
                    return;
                }

                var tempDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());
                try
                {
                    var packageInstallPath = InstallActionPackages(action.Package, tempDirectory);
                    InvokeActionInSeparateAppdomain(job, action, packageInstallPath);
                }
                finally
                {
                    if (_fileSystem.Directory.Exists(tempDirectory))
                    {
                        _fileSystem.Directory.Delete(tempDirectory, true);
                    }
                }
            }
            catch (Exception e)
            {
                _diagnostics.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_SignalProcessor_ProcessJobFailed_WithException,
                        e));
            }
        }
    }
}
