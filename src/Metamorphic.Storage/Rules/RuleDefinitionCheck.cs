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
using System.Linq;
using System.Text.RegularExpressions;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Metamorphic.Storage.Properties;

namespace Metamorphic.Storage.Rules
{
    /// <summary>
    /// Defines methods and properties that indicate whether a definition is valid or not.
    /// </summary>
    internal sealed class RuleDefinitionCheck
    {
        private static readonly Regex TriggerParameterMatcher = new Regex(RuleConstants.TriggerParameterRegex, RegexOptions.IgnoreCase);

        private static bool IsValidConditionType(string conditionType)
        {
            switch (conditionType)
            {
                case "equals":
                case "notequals":
                case "lessthan":
                case "greaterthan":
                case "matchregex":
                case "notmatchregex":
                case "startswith":
                case "endswith": return true;
                default: return false;
            }
        }

        /// <summary>
        /// The collection of errors that were found in the given definition.
        /// </summary>
        private readonly List<string> _errors
            = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDefinitionCheck"/> class.
        /// </summary>
        /// <param name="definition">The definition that should be checked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="definition"/> is <see langword="null" />.
        /// </exception>
        public RuleDefinitionCheck(RuleDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            Verify(definition);
        }

        /// <summary>
        /// Returns the collection of errors that were found when the definition was checked.
        /// </summary>
        /// <returns>The collection of errors that were found when the definition was checked.</returns>
        public IReadOnlyCollection<string> Errors()
        {
            return _errors.AsReadOnly();
        }

        /// <summary>
        /// Gets a value indicating whether or not the definition was valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _errors.Count == 0;
            }
        }

        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "A lot of if statements used to check validity.")]
        private void Verify(RuleDefinition definition)
        {
            if (string.IsNullOrEmpty(definition.Name))
            {
                _errors.Add(Resources.Log_Messages_RuleDefinitionCheck_DefinitionNameMissing);
            }

            if (definition.Signal == null)
            {
                _errors.Add(Resources.Log_Messages_RuleDefinitionCheck_NoSignalDefined);
            }

            if (definition.Signal != null)
            {
                if (string.IsNullOrEmpty(definition.Signal.Id))
                {
                    _errors.Add(Resources.Log_Messages_RuleDefinitionCheck_SignalHasNoId);
                }

                if (definition.Signal.Parameters != null)
                {
                    foreach (var pair in definition.Signal.Parameters)
                    {
                        if (pair.Value == null)
                        {
                            _errors.Add(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_RuleDefinitionCheck_SignalParameterHasInvalidValue_WithParameterName,
                                    pair.Key));
                        }
                    }
                }
            }

            foreach (var condition in definition.Condition)
            {
                if (string.IsNullOrWhiteSpace(condition.Name))
                {
                    _errors.Add(Resources.Log_Messages_RuleDefinitionCheck_ConditionHasNoName);
                }

                if ((definition.Signal.Parameters == null) || (!definition.Signal.Parameters.ContainsKey(condition.Name)))
                {
                    _errors.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_RuleDefinitionCheck_ConditionHasNoMatchingSignalParameter_WithConditionName,
                            condition.Name));
                }

                if (!IsValidConditionType(condition.Type))
                {
                    _errors.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_RuleDefinitionCheck_ConditionTypeIsNotValid_WithNameAndType,
                            condition.Name,
                            condition.Type));
                }

                if (condition.Pattern == null)
                {
                    _errors.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_RuleDefinitionCheck_ConditionHasNoPattern_WithConditionName,
                            condition.Name));
                }
            }

            if (definition.Action == null)
            {
                _errors.Add(Resources.Log_Messages_RuleDefinitionCheck_NoActionDefined);
            }

            if (definition.Action != null)
            {
                if (string.IsNullOrEmpty(definition.Action.Id))
                {
                    _errors.Add(Resources.Log_Messages_RuleDefinitionCheck_ActionHasNoId);
                }

                if ((definition.Action.Parameters != null) && (definition.Signal != null))
                {
                    foreach (var pair in definition.Action.Parameters)
                    {
                        var parameterText = pair.Value as string;
                        if (parameterText != null)
                        {
                            var matches = TriggerParameterMatcher.Matches(parameterText);
                            if (matches.Count > 0)
                            {
                                foreach (Match match in matches)
                                {
                                    // The first item in the groups collection is the full string that matched
                                    // (i.e. 'some stuff {{signal.XXXXX}} and some more'), the next items are the match groups.
                                    // Given that we only expect one match group we'll just use the first item.
                                    var signalParameterName = match.Groups[1].Value;
                                    if ((definition.Signal.Parameters == null) || (!definition.Signal.Parameters.ContainsKey(signalParameterName)))
                                    {
                                        _errors.Add(
                                            string.Format(
                                                CultureInfo.InvariantCulture,
                                                Resources.Log_Messages_RuleDefinitionCheck_ComplexActionParameterHasParametersNotProvidedBySignal_WithActionIdAndValueAndSignalParameters,
                                                definition.Action.Id,
                                                parameterText,
                                                definition.Signal.Parameters.Keys.Join(", ")));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
