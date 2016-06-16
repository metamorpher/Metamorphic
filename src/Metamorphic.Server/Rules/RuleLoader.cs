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
using System.IO;
using System.Text.RegularExpressions;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Metamorphic.Server.Rules
{
    internal sealed class RuleLoader : ILoadRules
    {
        private static readonly Regex TriggerParameterMatcher = new Regex(@"(?:{{signal.)(.*?)(?:}})", RegexOptions.IgnoreCase);

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The predicate that is used to determine if a given <see cref="ActionId"/> exists.
        /// </summary>
        private readonly Predicate<string> _doesActionIdExist;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleLoader"/> class.
        /// </summary>
        /// <param name="doesActionIdExist">The predicate that is used to determine if a given <see cref="ActionId"/> exists.</param>
        /// <param name="diagnostics">The object that stores the diagnostics methods for the current application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="doesActionIdExist"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public RuleLoader(Predicate<string> doesActionIdExist, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => doesActionIdExist);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            _doesActionIdExist = doesActionIdExist;
            _diagnostics = diagnostics;
        }

        internal RuleDefinition CreateDefinitionFromFile(string filePath)
        {
            using (var input = new StreamReader(filePath))
            {
                var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                deserializer.TypeResolvers.Add(new ScalarYamlNodeTypeResolver());
                var definition = deserializer.Deserialize<RuleDefinition>(input);

                return definition;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the current rule definition is valid or not.
        /// </summary>
        /// <param name="definition">The definition of the rule.</param>
        /// <param name="doesActionIdExist">The predicate used to determine if a the action ID exists.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        internal bool IsValid(
            RuleDefinition definition,
            Predicate<string> doesActionIdExist)
        {
            if (definition == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.Name))
            {
                return false;
            }

            if (definition.Signal == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.Signal.Id))
            {
                return false;
            }

            if (definition.Signal.Parameters != null)
            {
                foreach (var pair in definition.Signal.Parameters)
                {
                    if (pair.Value == null)
                    {
                        return false;
                    }
                }
            }

            foreach (var condition in definition.Condition)
            {
                if ((definition.Signal.Parameters == null) || (!definition.Signal.Parameters.ContainsKey(condition.Name)))
                {
                    return false;
                }

                if (!IsValidConditionType(condition.Type))
                {
                    return false;
                }

                if (condition.Pattern == null)
                {
                    return false;
                }
            }

            if (definition.Action == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.Action.Id) || !doesActionIdExist(definition.Action.Id))
            {
                return false;
            }

            if (definition.Action.Parameters != null)
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
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool IsValidConditionType(string conditionType)
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
        /// Creates a new <see cref="Rule"/> object from the information in the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        /// <returns>A newly created rule instance.</returns>
        public Rule Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            if (!File.Exists(filePath))
            {
                return null;
            }

            var definition = CreateDefinitionFromFile(filePath);
            if (!IsValid(definition, _doesActionIdExist))
            {
                _diagnostics.Log(
                    LevelToLog.Warn,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleLoader_InvalidRuleDefinition_WithFilePath,
                        filePath));

                return null;
            }

            if (definition.Enabled)
            {
                var signalParameterConditions = new Dictionary<string, Predicate<object>>();
                foreach (var condition in definition.Condition)
                {
                    var pred = ToCondition(condition);
                    if (pred != null)
                    {
                        signalParameterConditions.Add(condition.Name, pred);
                    }
                }

                var parameters = new Dictionary<string, ActionParameterValue>();
                foreach (var pair in definition.Action.Parameters)
                {
                    ActionParameterValue reference = null;

                    var parameterText = pair.Value as string;
                    if (parameterText != null)
                    {
                        var matches = TriggerParameterMatcher.Matches(parameterText);
                        if (matches.Count > 0)
                        {
                            var signalParameters = new List<string>();

                            foreach (Match match in matches)
                            {
                                // The first item in the groups collection is the full string that matched
                                // (i.e. 'some stuff {{signal.XXXXX}} and some more'), the next items are the match groups.
                                // Given that we only expect one match group we'll just use the first item.
                                var signalParameterName = match.Groups[1].Value;
                                signalParameters.Add(signalParameterName);
                            }

                            reference = new ActionParameterValue(pair.Key, parameterText, signalParameters);
                        }
                    }

                    if (reference == null)
                    {
                        reference = new ActionParameterValue(pair.Key, pair.Value);
                    }

                    parameters.Add(pair.Key, reference);
                }

                return new Rule(
                    definition.Name,
                    definition.Description,
                    new SignalTypeId(definition.Signal.Id),
                    new ActionId(definition.Action.Id),
                    signalParameterConditions,
                    parameters);
            }

            return null;
        }

        private Predicate<object> ToCondition(ConditionRuleDefinition condition)
        {
            object comparisonValue = condition.Pattern;
            switch (condition.Type)
            {
                case "equals":
                    return o => o.Equals(comparisonValue);
                case "notequals":
                    return o => !o.Equals(comparisonValue);
                case "lessthan":
                    return o =>
                    {
                        var comparable = o as IComparable;
                        return comparable.CompareTo(comparisonValue) < 0;
                    };
                case "greaterthan":
                    return o =>
                    {
                        var comparable = o as IComparable;
                        return comparable.CompareTo(comparisonValue) > 0;
                    };
                case "matchregex":
                    return o =>
                    {
                        var text = o as string;
                        var pattern = comparisonValue as string;
                        return (text != null) && (pattern != null) && Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
                    };
                case "notmatchregex":
                    return o =>
                    {
                        var text = o as string;
                        var pattern = comparisonValue as string;
                        return (text != null) && (pattern != null) && !Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
                    };
                case "startswith":
                    return o =>
                    {
                        var text = o as string;
                        var pattern = comparisonValue as string;
                        return (text != null) && (pattern != null) && text.StartsWith(pattern);
                    };
                case "endswith":
                    return o =>
                    {
                        var text = o as string;
                        var pattern = comparisonValue as string;
                        return (text != null) && (pattern != null) && text.EndsWith(pattern);
                    };
                default:
                    throw new InvalidConditionTypeException();
            }
        }
    }
}
