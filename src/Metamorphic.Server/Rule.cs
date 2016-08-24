//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server
{
    /// <summary>
    /// Defines methods for transforming signals into work.
    /// </summary>
    internal sealed class Rule
    {
        /// <summary>
        /// The ID of the action that should be executed in response to signals that match the current rule.
        /// </summary>
        private readonly ActionId _action;

        /// <summary>
        /// The collection of conditions that the signal parameters have to match in order for the signal to match
        /// the current reference. Note that all parameter names are
        /// stored in lower case so as to provide case-insensitive comparisons between the signal and
        /// rule parameter names.
        /// </summary>
        private readonly IDictionary<string, Predicate<object>> _conditions
            = new Dictionary<string, Predicate<object>>();

        /// <summary>
        /// The collection containing the required parameter references.
        /// </summary>
        private readonly Dictionary<string, ActionParameterValue> _references;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="name">The name of the rule.</param>
        /// <param name="description">The description of the rule. May be an empty string.</param>
        /// <param name="signalId">The ID of the sensor from which the signals will originate.</param>
        /// <param name="actionId">The ID of the action that should be executed in response to signals that match the current rule.</param>
        /// <param name="signalParameterConditions">The collection containing the conditions for the signal parameters.</param>
        /// <param name="parameterReferences">The parameters that need to be provided for the action to be executable.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="description"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalParameterConditions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterReferences"/> is <see langword="null" />.
        /// </exception>
        public Rule(
            string name,
            string description,
            SignalTypeId signalId,
            ActionId actionId,
            IDictionary<string, Predicate<object>> signalParameterConditions,
            IDictionary<string, ActionParameterValue> parameterReferences)
        {
            {
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => description);

                Lokad.Enforce.Argument(() => signalId);
                Lokad.Enforce.Argument(() => actionId);
                Lokad.Enforce.Argument(() => signalParameterConditions);
                Lokad.Enforce.Argument(() => parameterReferences);
            }

            Name = name;
            Description = description;

            Sensor = signalId;
            _action = actionId;

            if (signalParameterConditions != null)
            {
                foreach (var pair in signalParameterConditions)
                {
                    _conditions.Add(pair.Key.ToUpper(CultureInfo.InvariantCulture), pair.Value);
                }
            }

            _references = new Dictionary<string, ActionParameterValue>(parameterReferences);
        }

        /// <summary>
        /// Gets the description of the rule.
        /// </summary>
        public string Description
        {
            get;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Returns a value indicating whether or not the current rule applies to the given signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
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

            foreach (var parameterName in signal.Parameters())
            {
                if (_conditions.ContainsKey(parameterName))
                {
                    var condition = _conditions[parameterName];
                    if (!condition(signal.ParameterValue(parameterName)))
                    {
                        return false;
                    }
                }
            }

            foreach (var pair in _references)
            {
                if (!pair.Value.IsValidFor(signal))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the signal identifier to which this rule applies.
        /// </summary>
        public SignalTypeId Sensor
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
            foreach (var pair in _references)
            {
                // If the given parameter reference points to a signal parameter
                // then the signal needs to have that parameter and it needs to
                // match the criteria for that parameter (if they exist)
                parameters.Add(pair.Key, pair.Value.ValueForParameter(signal));
            }

            return new Job(
                _action,
                parameters);
        }
    }
}
