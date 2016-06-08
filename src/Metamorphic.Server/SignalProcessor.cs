//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Jobs;
using Metamorphic.Server.Properties;
using Metamorphic.Server.Rules;
using Metamorphic.Server.Signals;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Server
{
    internal sealed class SignalProcessor : IProcessSignals, IDisposable
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The queue that contains the jobs that should be executed.
        /// </summary>
        private readonly IQueueJobs m_JobQueue;

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection containing all the rules.
        /// </summary>
        private readonly IStoreRules m_RuleCollection;

        /// <summary>
        /// The queue that stores the location of the non-processed packages.
        /// </summary>
        private readonly IQueueSignals m_SignalQueue;

        /// <summary>
        /// The cancellation source that is used to cancel the worker task.
        /// </summary>
        private CancellationTokenSource m_CancellationSource;

        /// <summary>
        /// A flag indicating if the processing of symbols has started or not.
        /// </summary>
        private bool m_IsStarted;

        /// <summary>
        /// The task that handles the actual symbol indexing process.
        /// </summary>
        private Task m_Worker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalProcessor"/> class.
        /// </summary>
        /// <param name="jobQueue">The object that queues jobs that need to be processed.</param>
        /// <param name="ruleCollection">The object that stores all the known rules for the application.</param>
        /// <param name="signalQueue">The object that queues signals that need to be processed.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="jobQueue"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleCollection"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalQueue"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public SignalProcessor(
            IQueueJobs jobQueue,
            IStoreRules ruleCollection,
            IQueueSignals signalQueue,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => jobQueue);
                Lokad.Enforce.Argument(() => ruleCollection);
                Lokad.Enforce.Argument(() => signalQueue);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
            m_JobQueue = jobQueue;
            m_RuleCollection = ruleCollection;
            m_SignalQueue = signalQueue;
            m_SignalQueue.OnEnqueue += HandleOnEnqueue;
        }

        private void CleanUpWorkerTask()
        {
            lock (m_Lock)
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    Resources.Log_Messages_SignalProcessor_CleaningUpWorker);

                m_CancellationSource = null;
                m_Worker = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var task = Stop(false);
            task.Wait();
        }

        private void HandleOnEnqueue(object sender, EventArgs e)
        {
            lock (m_Lock)
            {
                if (!m_IsStarted)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        Resources.Log_Messages_SignalProcessor_NewItemInQueue_ProcessingNotStarted);

                    return;
                }

                if (m_Worker != null)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        Resources.Log_Messages_SignalProcessor_NewItemInQueue_WorkerAlreadyExists);

                    return;
                }

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    Resources.Log_Messages_SignalProcessor_NewItemInQueue_StartingThread);

                m_CancellationSource = new CancellationTokenSource();
                m_Worker = Task.Factory.StartNew(
                    () => ProcessSignals(m_CancellationSource.Token),
                    m_CancellationSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        private void ProcessSignals(CancellationToken token)
        {
            try
            {
                m_Diagnostics.Log(LevelToLog.Trace, Resources.Log_Messages_SignalProcessor_StartingSignalProcessing);

                while (!token.IsCancellationRequested)
                {
                    if (m_SignalQueue.IsEmpty)
                    {
                        m_Diagnostics.Log(LevelToLog.Trace, Resources.Log_Messages_SignalProcessor_QueueEmpty);
                        break;
                    }

                    Signal signal = null;
                    if (!m_SignalQueue.IsEmpty)
                    {
                        signal = m_SignalQueue.Dequeue();
                    }

                    if (signal == null)
                    {
                        continue;
                    }

                    m_Diagnostics.Log(
                        LevelToLog.Info,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_SignalProcessor_ProcessSignal_WithType,
                            signal.Sensor));

                    foreach (var parameter in signal.Parameters())
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_SignalProcessor_ProcessSignal_ForParameters,
                                parameter,
                                signal.ParameterValue(parameter)));
                    }

                    var rules = m_RuleCollection.RulesForSignal(signal.Sensor);
                    foreach (var rule in rules)
                    {
                        if (rule.ShouldProcess(signal))
                        {
                            var job = rule.ToJob(signal);
                            m_JobQueue.Enqueue(job);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_SignalProcessor_ProcessSignalsFailed_WithException,
                        e));
            }
            finally
            {
                CleanUpWorkerTask();
            }
        }

        /// <summary>
        /// Starts the signal processing.
        /// </summary>
        public void Start()
        {
            lock (m_Lock)
            {
                m_IsStarted = true;
            }
        }

        /// <summary>
        /// Stops the signal processing.
        /// </summary>
        /// <param name="clearCurrentQueue">
        /// Indicates if the elements currently in the queue need to be processed before stopping or not.
        /// </param>
        /// <returns>A task that completes when the indexer has stopped.</returns>
        public Task Stop(bool clearCurrentQueue)
        {
            m_IsStarted = false;

            var result = Task.Factory.StartNew(
                () =>
                {
                    m_Diagnostics.Log(
                        LevelToLog.Info,
                        Resources.Log_Messages_SignalProcessor_StoppingProcessing);

                    if (!clearCurrentQueue && !m_SignalQueue.IsEmpty)
                    {
                        lock (m_Lock)
                        {
                            if (m_CancellationSource != null)
                            {
                                m_CancellationSource.Cancel();
                            }
                        }
                    }

                    Task worker;
                    lock (m_Lock)
                    {
                        worker = m_Worker;
                    }

                    if (worker != null)
                    {
                        worker.Wait();
                    }

                    CleanUpWorkerTask();
                });

            return result;
        }
    }
}
