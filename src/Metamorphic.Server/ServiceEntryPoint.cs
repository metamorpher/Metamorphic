//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;
using Metamorphic.Server.Jobs;
using Metamorphic.Server.Properties;
using Metamorphic.Server.Rules;
using Metamorphic.Server.Signals;
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
        /// The object that is used to process queued jobs.
        /// </summary>
        private IProcessJobs m_JobProcessor;

        /// <summary>
        /// The object that is used to keep track of the available rules.
        /// </summary>
        private IWatchRules m_RuleWatcher;

        /// <summary>
        /// The object that generates the signals.
        /// </summary>
        private IGenerateSignals m_SignalGenerator;

        /// <summary>
        /// The object that processes signals.
        /// </summary>
        private IProcessSignals m_SignalProcessor;

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
            m_JobProcessor = m_Container.Resolve<IProcessJobs>();
            m_RuleWatcher = m_Container.Resolve<IWatchRules>();
            m_SignalGenerator = m_Container.Resolve<IGenerateSignals>();
            m_SignalProcessor = m_Container.Resolve<IProcessSignals>();
            m_Diagnostics = m_Container.Resolve<SystemDiagnostics>();

            m_JobProcessor.Start();
            m_RuleWatcher.Enable();
            m_SignalProcessor.Start();
            m_SignalGenerator.Start();

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
            }

            if (hasBeenStopped)
            {
                return;
            }

            try
            {
                if (m_RuleWatcher != null)
                {
                    m_RuleWatcher.Disable();
                    m_RuleWatcher = null;
                }

                if (m_SignalGenerator != null)
                {
                    m_SignalGenerator.Stop();
                    m_SignalGenerator = null;
                }

                if (m_SignalProcessor != null)
                {
                    var clearingTask =  m_SignalProcessor.Stop(true);
                    clearingTask.Wait();

                    m_SignalProcessor = null;
                }

                if (m_JobProcessor != null)
                {
                    var clearingTask = m_JobProcessor.Stop(true);
                    clearingTask.Wait();

                    m_JobProcessor = null;
                }

                // Do what ever we need to do to stop the service here
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