//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Sensors;
using Metamorphic.Core.Signals;
using Metamorphic.Core.Actions;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines methods for transforming signals into work.
    /// </summary>
    public sealed class Rule
    {
        /// <summary>
        /// The ID of the action that should be executed in response to signals that match the current rule.
        /// </summary>
        private readonly ActionId m_Action;

        /// <summary>
        /// The collection containing the required parameter references.
        /// </summary>
        private readonly Dictionary<string, ActionParameterValue> m_References;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="signalId">The ID of the sensor from which the signals will originate.</param>
        /// <param name="actionId">The ID of the action that should be executed in response to signals that match the current rule.</param>
        /// <param name="parameterReferences">The parameters that need to be provided for the action to be executable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterReferences"/> is <see langword="null" />.
        /// </exception>
        public Rule(
            SensorId signalId,
            ActionId actionId,
            IDictionary<string, ActionParameterValue> parameterReferences)
        {
            {
                Lokad.Enforce.Argument(() => signalId);
                Lokad.Enforce.Argument(() => actionId);
                Lokad.Enforce.Argument(() => parameterReferences);
            }

            Sensor = signalId;
            m_Action = actionId;
            m_References = new Dictionary<string, ActionParameterValue>(parameterReferences);
        }

        /// <summary>
        /// Returns a value indicating whether or not the current rule applies to the given signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ShouldProcess(Signal signal)
        {
            if (signal == null)
            {
                return false;
            }

            if (!signal.Sensor.Equals(Sensor))
            {
                return false;
            }

            foreach (var pair in m_References)
            {
                if (!pair.Value.IsValidFor(signal))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets or sets the signal identifier to which this rule applies.
        /// </summary>
        public SensorId Sensor
        {
            get;
        }

        /// <summary>
        /// Creates a new <see cref="Job"/> based on the current rule and the given signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The newly created job.</returns>
        public Job ToJob(Signal signal)
        {
            if (!ShouldProcess(signal))
            {
                throw new InvalidSignalForRuleException();
            }

            var parameters = new Dictionary<string, object>();
            foreach (var pair in m_References)
            {
                // If the given parameter reference points to a signal parameter
                // then the signal needs to have that parameter and it needs to
                // match the criteria for that parameter (if they exist)
                parameters.Add(pair.Key, pair.Value.ValueForParameter(signal));
            }

            return new Job(
                m_Action,
                parameters);
        }
    }
}
