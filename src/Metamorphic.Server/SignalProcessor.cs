//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Jobs;
using Metamorphic.Server.Properties;
using Metamorphic.Server.Rules;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Server
{
    internal sealed class SignalProcessor
    {
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
        /// <param name="jobQueue">The object that queues jobs that need to be processed.</param>
        /// <param name="ruleCollection">The object that stores all the known rules for the application.</param>
        /// <param name="signalDispenser">The object that dispenses signals that need to be processed.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="jobQueue"/> is <see langword="null" />.
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
        public SignalProcessor(
            IQueueJobs jobQueue,
            IStoreRules ruleCollection,
            IDispenseSignals signalDispenser,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => jobQueue);
                Lokad.Enforce.Argument(() => ruleCollection);
                Lokad.Enforce.Argument(() => signalDispenser);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            _diagnostics = diagnostics;
            _jobQueue = jobQueue;
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
                    _jobQueue.Enqueue(job);
                }
            }
        }
    }
}
