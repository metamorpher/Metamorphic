//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
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
        private readonly IStoreActions _actionStorage;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The queue that contains the jobs that should be executed.
        /// </summary>
        private readonly IQueueJobs _jobQueue;

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The cancellation source that is used to cancel the worker task.
        /// </summary>
        private CancellationTokenSource _cancellationSource;

        /// <summary>
        /// A flag indicating if the processing of symbols has started or not.
        /// </summary>
        private bool _isStarted;

        /// <summary>
        /// The task that handles the actual symbol indexing process.
        /// </summary>
        private Task _worker;

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

            _actionStorage = actions;
            _diagnostics = diagnostics;
            _jobQueue = jobQueue;
            _jobQueue.OnEnqueue += HandleOnEnqueue;
        }

        private void CleanUpWorkerTask()
        {
            lock (_lock)
            {
                _diagnostics.Log(
                    LevelToLog.Trace,
                    Resources.Log_Messages_JobProcessor_CleaningUpWorker);

                _cancellationSource = null;
                _worker = null;
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
            lock (_lock)
            {
                if (!_isStarted)
                {
                    _diagnostics.Log(
                        LevelToLog.Trace,
                        Resources.Log_Messages_JobProcessor_NewItemInQueue_ProcessingNotStarted);

                    return;
                }

                if (_worker != null)
                {
                    _diagnostics.Log(
                        LevelToLog.Trace,
                        Resources.Log_Messages_JobProcessor_NewItemInQueue_WorkerAlreadyExists);

                    return;
                }

                _diagnostics.Log(
                    LevelToLog.Trace,
                    Resources.Log_Messages_JobProcessor_NewItemInQueue_StartingThread);

                _cancellationSource = new CancellationTokenSource();
                _worker = Task.Factory.StartNew(
                    () => ProcessJobs(_cancellationSource.Token),
                    _cancellationSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        private void ProcessJobs(CancellationToken token)
        {
            try
            {
                _diagnostics.Log(LevelToLog.Trace, Resources.Log_Messages_JobProcessor_StartingJobProcessing);

                while (!token.IsCancellationRequested)
                {
                    if (_jobQueue.IsEmpty)
                    {
                        _diagnostics.Log(LevelToLog.Trace, Resources.Log_Messages_JobProcessor_QueueEmpty);
                        break;
                    }

                    Job job = null;
                    if (!_jobQueue.IsEmpty)
                    {
                        job = _jobQueue.Dequeue();
                    }

                    if (job == null)
                    {
                        continue;
                    }

                    _diagnostics.Log(
                        LevelToLog.Info,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_JobProcessor_ProcessJob_WithId,
                            job.Action));

                    foreach (var parameter in job.ParameterNames())
                    {
                        _diagnostics.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_JobProcessor_ProcessJob_ForParameters,
                                parameter,
                                job.ParameterValue(parameter)));
                    }

                    // Find the action
                    if (!_actionStorage.HasActionFor(job.Action))
                    {
                        _diagnostics.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_JobProcessor_ProcessJob_ActionIdNotFound_WithId,
                                job.Action));
                        continue;
                    }

                    var action = _actionStorage.Action(job.Action);
                    try
                    {
                        var parameters = ToParameterData(job);
                        action.Invoke(parameters);
                    }
                    catch (Exception e)
                    {
                        _diagnostics.Log(
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
                _diagnostics.Log(
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
            lock (_lock)
            {
                _isStarted = true;
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
            _isStarted = false;

            var result = Task.Factory.StartNew(
                () =>
                {
                    _diagnostics.Log(
                        LevelToLog.Info,
                        Resources.Log_Messages_JobProcessor_StoppingProcessing);

                    if (!clearCurrentQueue && !_jobQueue.IsEmpty)
                    {
                        lock (_lock)
                        {
                            if (_cancellationSource != null)
                            {
                                _cancellationSource.Cancel();
                            }
                        }
                    }

                    Task worker;
                    lock (_lock)
                    {
                        worker = _worker;
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
            foreach (var name in job.ParameterNames())
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
