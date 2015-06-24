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

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines methods for transforming signals into work.
    /// </summary>
    public sealed class Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="signalId">The ID of the sensor from which the signals will originate.</param>
        /// <param name="parameterCriteria">
        ///     The criteria that should be applied to the individual parameters in order for the current rule to apply to a signal.
        /// </param>
        /// <param name="parameterReferences">The parameters that need to be provided for the action to be executable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterCriteria"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterReferences"/> is <see langword="null" />.
        /// </exception>
        public Rule(
            SensorId signalId,
            IDictionary<string, Predicate<object>> parameterCriteria,
            IDictionary<string, object> parameterReferences)
        {
            {
                Lokad.Enforce.Argument(() => signalId);
                Lokad.Enforce.Argument(() => parameterCriteria);
                Lokad.Enforce.Argument(() => parameterReferences);

                // Ensure that all parameters which reference a trigger parameter have a criteria?
            }

            Sensor = signalId;
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

            // Match the parameters to the criteria and the trigger parameters
            // Additional parameters are ok, missing parameters are not
            foobar();

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


            return new Job();
        }
    }
}
