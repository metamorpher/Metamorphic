//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
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
        /// The object that dispenses signals that need to be processed.
        /// </summary>
        private readonly IDispenseSignals m_SignalDispenser;

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

            m_Diagnostics = diagnostics;
            m_JobQueue = jobQueue;
            m_RuleCollection = ruleCollection;
            m_SignalDispenser = signalDispenser;
            m_SignalDispenser.OnItemAvailable += HandleOnEnqueue;
        }

        private void HandleOnEnqueue(object sender, ItemEventArgs<Signal> e)
        {
            Signal signal = e.Item;
            if (signal == null)
            {
                return;
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
}
