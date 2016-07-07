//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;
using Metamorphic.Storage.Actions;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Storage
{
    /// <summary>
    /// Defines the methods used by the service manager to control the service.
    /// </summary>
    internal sealed class ServiceEntryPoint : IDisposable
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The IOC container for the service.
        /// </summary>
        private IContainer _container;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private SystemDiagnostics _diagnostics;

        /// <summary>
        /// A flag that indicates that the application has been stopped.
        /// </summary>
        private volatile bool _hasBeenStopped;

        /// <summary>
        /// A flag that indicates if the service has been disposed or not.
        /// </summary>
        private volatile bool _isDisposed;

        /// <summary>
        /// The object that that watches the NuGet feed directories for new packages.
        /// </summary>
        private IWatchPackages _packageWatcher;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                OnStop();

                _isDisposed = true;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent
        /// to the service by the Service Control Manager (SCM) or when the operating
        /// system starts (for a service that starts automatically). Specifies actions
        /// to take when the service starts.
        /// </summary>
        public void OnStart()
        {
            _container = DependencyInjection.CreateContainer();

            _packageWatcher = _container.Resolve<IWatchPackages>();
            _diagnostics = _container.Resolve<SystemDiagnostics>();

            _packageWatcher.Enable();

            _diagnostics.Log(
                LevelToLog.Info,
                StorageConstants.LogPrefix,
                Resources.Log_Messages_ServiceStarted);
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service stops running.
        /// </summary>
        public void OnStop()
        {
            bool hasBeenStopped;
            lock (_lock)
            {
                hasBeenStopped = _hasBeenStopped;
            }

            if (hasBeenStopped)
            {
                return;
            }

            try
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    StorageConstants.LogPrefix,
                    Resources.Log_Messages_ServiceStopped);

                if (_packageWatcher != null)
                {
                    _packageWatcher.Disable();
                    _packageWatcher = null;
                }

                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
            }
            finally
            {
                lock (_lock)
                {
                    _hasBeenStopped = true;
                }
            }
        }
    }
}
