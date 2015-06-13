//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;
using Metamorphic.Server.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Server
{
    /// <summary>
    /// Defines the methods used by the service manager to control the service.
    /// </summary>
    internal sealed class ServiceEntryPoint : IDisposable
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The IOC container for the service.
        /// </summary>
        private IContainer m_Container;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// A flag that indicates that the application has been stopped.
        /// </summary>
        private volatile bool m_HasBeenStopped;

        /// <summary>
        /// A flag that indicates if the service has been disposed or not.
        /// </summary>
        private volatile bool m_IsDisposed;

        /// <summary>
        /// A flag that indicates that the current service is stopping the application.
        /// </summary>
        private bool m_IsStopping;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                OnStop();

                m_IsDisposed = true;
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
            m_Container = DependencyInjection.CreateContainer();
            m_Diagnostics = m_Container.Resolve<SystemDiagnostics>();

            m_Diagnostics.Log(
                LevelToLog.Info,
                ServerConstants.LogPrefix,
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
            lock (m_Lock)
            {
                hasBeenStopped = m_HasBeenStopped;
                m_IsStopping = true;
            }

            if (hasBeenStopped)
            {
                return;
            }

            try
            {
                // Do what ever we need to do to stop the service here
                foobar();

                m_Diagnostics.Log(
                    LevelToLog.Info,
                    ServerConstants.LogPrefix,
                    Resources.Log_Messages_ServiceStopped);

                if (m_Container != null)
                {
                    m_Container.Dispose();
                    m_Container = null;
                }
            }
            finally
            {
                lock (m_Lock)
                {
                    m_HasBeenStopped = true;
                }
            }
        }
    }
}