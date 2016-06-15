//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Metamorphic.Server.Actions;
using Metamorphic.Server.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Server.Jobs
{
    internal sealed class JobProcessor : IProcessJobs, IDisposable
    {
        /// <summary>
        /// The collection that contains all the actions for the application.
        /// </summary>
        private readonly IStoreActions m_ActionStorage;

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
        /// Initializes a new instance of the <see cref="JobProcessor"/> class.
        /// </summary>
        /// <param name="actions">The collection that stores all the actions known to the application.</param>
        /// <param name="jobQueue">The object that queues jobs that need to be processed.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="jobQueue"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public JobProcessor(
            IStoreActions actions,
            IQueueJobs jobQueue,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => actions);
                Lokad.Enforce.Argument(() => jobQueue);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_ActionStorage = actions;
            m_Diagnostics = diagnostics;
            m_JobQueue = jobQueue;
            m_JobQueue.OnEnqueue += HandleOnEnqueue;
        }

        private void CleanUpWorkerTask()
        {
            lock (m_Lock)
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    Resources.Log_Messages_JobProcessor_CleaningUpWorker);

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
                        Resources.Log_Messages_JobProcessor_NewItemInQueue_ProcessingNotStarted);

                    return;
                }

                if (m_Worker != null)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        Resources.Log_Messages_JobProcessor_NewItemInQueue_WorkerAlreadyExists);

                    return;
                }

                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    Resources.Log_Messages_JobProcessor_NewItemInQueue_StartingThread);

                m_CancellationSource = new CancellationTokenSource();
                m_Worker = Task.Factory.StartNew(
                    () => ProcessJobs(m_CancellationSource.Token),
                    m_CancellationSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        private void ProcessJobs(CancellationToken token)
        {
            try
            {
                m_Diagnostics.Log(LevelToLog.Trace, Resources.Log_Messages_JobProcessor_StartingJobProcessing);

                while (!token.IsCancellationRequested)
                {
                    if (m_JobQueue.IsEmpty)
                    {
                        m_Diagnostics.Log(LevelToLog.Trace, Resources.Log_Messages_JobProcessor_QueueEmpty);
                        break;
                    }

                    Job job = null;
                    if (!m_JobQueue.IsEmpty)
                    {
                        job = m_JobQueue.Dequeue();
                    }

                    if (job == null)
                    {
                        continue;
                    }

                    m_Diagnostics.Log(
                        LevelToLog.Info,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_JobProcessor_ProcessJob_WithId,
                            job.Action));

                    foreach (var parameter in job.ParameterNames())
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_JobProcessor_ProcessJob_ForParameters,
                                parameter,
                                job.ParameterValue(parameter)));
                    }

                    // Find the action
                    if (!m_ActionStorage.HasActionFor(job.Action))
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_JobProcessor_ProcessJob_ActionIdNotFound_WithId,
                                job.Action));
                        continue;
                    }

                    var action = m_ActionStorage.Action(job.Action);
                    try
                    {
                        var parameters = ToParameterData(job);
                        action.Invoke(parameters);
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_JobProcessor_ProcessJobFailed_WithId_WithException,
                                job.Action,
                                e));
                    }
                }
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_JobProcessor_ProcessJobsFailed_WithException,
                        e));
            }
            finally
            {
                CleanUpWorkerTask();
            }
        }

        /// <summary>
        /// Starts the job executing process.
        /// </summary>
        public void Start()
        {
            lock (m_Lock)
            {
                m_IsStarted = true;
            }
        }

        /// <summary>
        /// Stops the job executing process.
        /// </summary>
        /// <param name="clearCurrentQueue">
        /// Indicates if the elements currently in the queue need to be processed before stopping or not.
        /// </param>
        /// <returns>A task that completes when the processor has stopped.</returns>
        public Task Stop(bool clearCurrentQueue)
        {
            m_IsStarted = false;

            var result = Task.Factory.StartNew(
                () =>
                {
                    m_Diagnostics.Log(
                        LevelToLog.Info,
                        Resources.Log_Messages_JobProcessor_StoppingProcessing);

                    if (!clearCurrentQueue && !m_JobQueue.IsEmpty)
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

        private ActionParameterValueMap[] ToParameterData(Job job)
        {
            var namedParameters = new List<ActionParameterValueMap>();
            foreach(var name in job.ParameterNames())
            {
                var value = job.ParameterValue(name);

                namedParameters.Add(
                    new ActionParameterValueMap(
                        new ActionParameterDefinition(name),
                        value));
            }

            return namedParameters.ToArray();
        }
    }
}
