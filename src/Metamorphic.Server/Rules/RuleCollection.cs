//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Sensors;
using Nuclei.Diagnostics;

namespace Metamorphic.Server.Rules
{
    internal sealed class RuleCollection : IStoreRules
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The collection that maps file paths to rules.
        /// </summary>
        private readonly Dictionary<string, Rule> m_FileToRuleMap = new Dictionary<string, Rule>();

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that maps signal types to rules.
        /// </summary>
        private readonly Dictionary<SensorId, List<Rule>> m_SignalTypeToRuleMap = new Dictionary<SensorId, List<Rule>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCollection"/> class.
        /// </summary>
        /// <param name="diagnostics">The object providing the diagnostics methods for the application.</param>
        internal RuleCollection(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Adds a new <see cref="Rule"/> that was created from the given file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        public void Add(string filePath, Rule rule)
        {
            lock (m_Lock)
            {
                m_FileToRuleMap.Add(filePath, rule);

                if (!m_SignalTypeToRuleMap.ContainsKey(rule.Sensor))
                {
                    m_SignalTypeToRuleMap.Add(rule.Sensor, new List<Rule>());
                }
                List<Rule> collection = m_SignalTypeToRuleMap[rule.Sensor];
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
            lock(m_Lock)
            {
                Rule rule = null;
                if (m_FileToRuleMap.ContainsKey(filePath))
                {
                    rule = m_FileToRuleMap[filePath];
                    m_FileToRuleMap.Remove(filePath);
                }

                if (rule != null)
                {
                    if (m_SignalTypeToRuleMap.ContainsKey(rule.Sensor))
                    {
                        var collection = m_SignalTypeToRuleMap[rule.Sensor];
                        collection.Remove(rule);

                        if (collection.Count == 0)
                        {
                            m_SignalTypeToRuleMap.Remove(rule.Sensor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a collection containing all rules that are applicable for the given signal type.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <returns></returns>
        public IEnumerable<Rule> RulesForSignal(SensorId sensorId)
        {
            lock(m_Lock)
            {
                if (m_SignalTypeToRuleMap.ContainsKey(sensorId))
                {
                    return new List<Rule>(m_SignalTypeToRuleMap[sensorId]);
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
        public void Update(string filePath, Rule rule)
        {
            lock (m_Lock)
            {
                Rule ruleToReplace = null;
                if (m_FileToRuleMap.ContainsKey(filePath))
                {
                    ruleToReplace = m_FileToRuleMap[filePath];
                    m_FileToRuleMap[filePath] = rule;
                }

                if (ruleToReplace != null)
                {
                    List<Rule> oldCollection = null;
                    if (m_SignalTypeToRuleMap.ContainsKey(ruleToReplace.Sensor))
                    {
                        oldCollection = m_SignalTypeToRuleMap[ruleToReplace.Sensor];
                        oldCollection.Remove(ruleToReplace);
                    }

                    if (!m_SignalTypeToRuleMap.ContainsKey(rule.Sensor))
                    {
                        m_SignalTypeToRuleMap.Add(rule.Sensor, new List<Rule>());
                    }

                    var newCollection = m_SignalTypeToRuleMap[rule.Sensor];
                    newCollection.Add(rule);

                    if ((oldCollection != null) && (oldCollection.Count == 0))
                    {
                        m_SignalTypeToRuleMap.Remove(rule.Sensor);
                    }
                }
            }
        }
    }
}
