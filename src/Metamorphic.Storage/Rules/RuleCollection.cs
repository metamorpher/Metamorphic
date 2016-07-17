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
        /// The collection that maps package names to rules.
        /// </summary>
        private readonly Dictionary<RuleOrigin, List<RuleDefinition>> _packageToRuleMap
            = new Dictionary<RuleOrigin, List<RuleDefinition>>();

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The collection that maps signal types to rules.
        /// </summary>
        private readonly Dictionary<SignalTypeId, List<RuleDefinition>> _signalTypeToRuleMap
            = new Dictionary<SignalTypeId, List<RuleDefinition>>();

        /// <summary>
        /// Adds a new <see cref="RuleDefinition"/> that was created from the given origin.
        /// </summary>
        /// <param name="origin">The origin of the data that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="rule"/> is <see langword="null" />.
        /// </exception>
        public void Add(RuleOrigin origin, RuleDefinition rule)
        {
            if (origin == null)
            {
                throw new ArgumentNullException("origin");
            }

            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }

            lock (_lock)
            {
                if (!_packageToRuleMap.ContainsKey(origin))
                {
                    _packageToRuleMap.Add(origin, new List<RuleDefinition>());
                }

                var rules = _packageToRuleMap[origin];
                rules.Add(rule);

                var signalId = new SignalTypeId(rule.Signal.Id);
                if (!_signalTypeToRuleMap.ContainsKey(signalId))
                {
                    _signalTypeToRuleMap.Add(signalId, new List<RuleDefinition>());
                }

                List<RuleDefinition> collection = _signalTypeToRuleMap[signalId];
                if (!collection.Contains(rule))
                {
                    collection.Add(rule);
                }
            }
        }

        /// <summary>
        /// Removes the <see cref="RuleDefinition"/> that is associated with the given origin.
        /// </summary>
        /// <param name="origin">The origin of the rule.</param>
        public void Remove(RuleOrigin origin)
        {
            if (origin == null)
            {
                return;
            }

            lock (_lock)
            {
                List<RuleDefinition> rules = null;
                if (_packageToRuleMap.ContainsKey(origin))
                {
                    rules = _packageToRuleMap[origin];
                    _packageToRuleMap.Remove(origin);
                }

                if (rules != null)
                {
                    foreach (var rule in rules)
                    {
                        var signalId = new SignalTypeId(rule.Signal.Id);
                        if (_signalTypeToRuleMap.ContainsKey(signalId))
                        {
                            var collection = _signalTypeToRuleMap[signalId];
                            collection.Remove(rule);

                            if (collection.Count == 0)
                            {
                                _signalTypeToRuleMap.Remove(signalId);
                            }
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
        public RuleDefinition[] RulesForSignal(SignalTypeId sensorId)
        {
            lock (_lock)
            {
                if (_signalTypeToRuleMap.ContainsKey(sensorId))
                {
                    return _signalTypeToRuleMap[sensorId].ToArray();
                }
                else
                {
                    return new RuleDefinition[0];
                }
            }
        }

        /// <summary>
        /// Updates an existing <see cref="RuleDefinition"/>.
        /// </summary>
        /// <param name="origin">The origin of the data that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="rule"/> is <see langword="null" />.
        /// </exception>
        public void Update(RuleOrigin origin, RuleDefinition rule)
        {
            if (origin == null)
            {
                throw new ArgumentNullException("origin");
            }

            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }

            lock (_lock)
            {
                RuleDefinition ruleToReplace = null;
                if (_packageToRuleMap.ContainsKey(origin))
                {
                    var rules = _packageToRuleMap[origin];
                    var index = rules.FindIndex(r => string.Equals(rule.Name, r.Name, StringComparison.OrdinalIgnoreCase));
                    if (index > -1)
                    {
                        ruleToReplace = rules[index];
                        rules[index] = rule;
                    }
                    else
                    {
                        rules.Add(rule);
                    }
                }

                if (ruleToReplace != null)
                {
                    var signalIdToReplace = new SignalTypeId(ruleToReplace.Signal.Id);
                    List<RuleDefinition> oldCollection = null;
                    if (_signalTypeToRuleMap.ContainsKey(signalIdToReplace))
                    {
                        oldCollection = _signalTypeToRuleMap[signalIdToReplace];
                        oldCollection.Remove(ruleToReplace);
                    }

                    var signalId = new SignalTypeId(rule.Signal.Id);
                    if (!_signalTypeToRuleMap.ContainsKey(signalId))
                    {
                        _signalTypeToRuleMap.Add(signalId, new List<RuleDefinition>());
                    }

                    var newCollection = _signalTypeToRuleMap[signalId];
                    newCollection.Add(rule);

                    if ((oldCollection != null) && (oldCollection.Count == 0))
                    {
                        _signalTypeToRuleMap.Remove(signalId);
                    }
                }
            }
        }
    }
}
