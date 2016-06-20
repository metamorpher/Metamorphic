//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;

namespace Metamorphic.Storage.Rules
{
    internal sealed class RuleCollection : IStoreRules
    {
        /// <summary>
        /// The collection that maps file paths to rules.
        /// </summary>
        private readonly Dictionary<string, Rule> _fileToRuleMap = new Dictionary<string, Rule>();

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The collection that maps signal types to rules.
        /// </summary>
        private readonly Dictionary<SignalTypeId, List<Rule>> _signalTypeToRuleMap = new Dictionary<SignalTypeId, List<Rule>>();

        /// <summary>
        /// Adds a new <see cref="Rule"/> that was created from the given file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="rule"/> is <see langword="null" />.
        /// </exception>
        public void Add(string filePath, Rule rule)
        {
            {
                Lokad.Enforce.Argument(() => filePath);
                Lokad.Enforce.Argument(() => filePath, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => rule);
            }

            lock (_lock)
            {
                if (_fileToRuleMap.ContainsKey(filePath))
                {
                    throw new RuleAlreadyExistsException();
                }

                _fileToRuleMap.Add(filePath, rule);

                if (!_signalTypeToRuleMap.ContainsKey(rule.Sensor))
                {
                    _signalTypeToRuleMap.Add(rule.Sensor, new List<Rule>());
                }

                List<Rule> collection = _signalTypeToRuleMap[rule.Sensor];
                if (!collection.Contains(rule))
                {
                    collection.Add(rule);
                }
            }
        }

        /// <summary>
        /// Removes the <see cref="Rule"/> that is associated with the given file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        public void Remove(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            lock (_lock)
            {
                Rule rule = null;
                if (_fileToRuleMap.ContainsKey(filePath))
                {
                    rule = _fileToRuleMap[filePath];
                    _fileToRuleMap.Remove(filePath);
                }

                if (rule != null)
                {
                    if (_signalTypeToRuleMap.ContainsKey(rule.Sensor))
                    {
                        var collection = _signalTypeToRuleMap[rule.Sensor];
                        collection.Remove(rule);

                        if (collection.Count == 0)
                        {
                            _signalTypeToRuleMap.Remove(rule.Sensor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a collection containing all rules that are applicable for the given signal type.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <returns>A collection that contains all the rules that apply to the given signal.</returns>
        public IEnumerable<Rule> RulesForSignal(SignalTypeId sensorId)
        {
            lock (_lock)
            {
                if (_signalTypeToRuleMap.ContainsKey(sensorId))
                {
                    return new List<Rule>(_signalTypeToRuleMap[sensorId]);
                }
                else
                {
                    return new List<Rule>();
                }
            }
        }

        /// <summary>
        /// Updates an existing <see cref="Rule"/>.
        /// </summary>
        /// <param name="filePath">The full path to the rule file that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="rule"/> is <see langword="null" />.
        /// </exception>
        public void Update(string filePath, Rule rule)
        {
            {
                Lokad.Enforce.Argument(() => filePath);
                Lokad.Enforce.Argument(() => filePath, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => rule);
            }

            lock (_lock)
            {
                Rule ruleToReplace = null;
                if (_fileToRuleMap.ContainsKey(filePath))
                {
                    ruleToReplace = _fileToRuleMap[filePath];
                    _fileToRuleMap[filePath] = rule;
                }

                if (ruleToReplace != null)
                {
                    List<Rule> oldCollection = null;
                    if (_signalTypeToRuleMap.ContainsKey(ruleToReplace.Sensor))
                    {
                        oldCollection = _signalTypeToRuleMap[ruleToReplace.Sensor];
                        oldCollection.Remove(ruleToReplace);
                    }

                    if (!_signalTypeToRuleMap.ContainsKey(rule.Sensor))
                    {
                        _signalTypeToRuleMap.Add(rule.Sensor, new List<Rule>());
                    }

                    var newCollection = _signalTypeToRuleMap[rule.Sensor];
                    newCollection.Add(rule);

                    if ((oldCollection != null) && (oldCollection.Count == 0))
                    {
                        _signalTypeToRuleMap.Remove(rule.Sensor);
                    }
                }
            }
        }
    }
}
