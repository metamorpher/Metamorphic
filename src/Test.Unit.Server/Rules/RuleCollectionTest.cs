//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Sensors;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Metamorphic.Server.Rules
{
    [TestFixture]
    public sealed class RuleCollectionTest
    {
        [Test]
        public void Add()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());
        }

        [Test]
        public void AddWithEmptyFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            Assert.Throws<ArgumentException>(() => collection.Add(string.Empty, new Rule(new SensorId("a"), new ActionId("b"), new Dictionary<string, ActionParameterValue>())));
        }

        [Test]
        public void AddWithExistingFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var sensor = new SensorId("b");
            var path = "a";
            var rule = new Rule(sensor, new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());

            var path2 = "d";
            var rule2 = new Rule(sensor, new ActionId("e"), new Dictionary<string, ActionParameterValue>());
            Assert.Throws<RuleAlreadyExistsException>(() => collection.Add(path2, rule2));
        }

        [Test]
        public void AddWithExistingSensorId()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var sensor = new SensorId("b");
            var path1 = "a";
            var rule1 = new Rule(sensor, new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path1, rule1);

            var matchingRules = collection.RulesForSignal(sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var path2 = "d";
            var rule2 = new Rule(sensor, new ActionId("e"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path2, rule2);

            matchingRules = collection.RulesForSignal(sensor);
            Assert.That(matchingRules, Is.EquivalentTo(new[] { rule1, rule2 }));
        }

        [Test]
        public void AddWithNullFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            Assert.Throws<ArgumentNullException>(() => collection.Add(null, new Rule(new SensorId("a"), new ActionId("b"), new Dictionary<string, ActionParameterValue>())));
        }

        [Test]
        public void AddWithNullRule()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            Assert.Throws<ArgumentNullException>(() => collection.Add("a", null));
        }

        [Test]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(() => new RuleCollection(null));
        }

        [Test]
        public void Remove()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());

            collection.Remove(path);
            matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(0, matchingRules.Count());
        }

        [Test]
        public void RemoveWithEmptyFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());

            collection.Remove(string.Empty);
            matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());
        }

        [Test]
        public void RemoveWithMultipleRulesPresent()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var sensor = new SensorId("b");
            var path1 = "a";
            var rule1 = new Rule(sensor, new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path1, rule1);

            var matchingRules = collection.RulesForSignal(sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var path2 = "d";
            var rule2 = new Rule(sensor, new ActionId("e"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path2, rule2);

            matchingRules = collection.RulesForSignal(sensor);
            Assert.That(matchingRules, Is.EquivalentTo(new[] { rule1, rule2 }));

            collection.Remove(path1);

            matchingRules = collection.RulesForSignal(sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule2, matchingRules.First());
        }

        [Test]
        public void RemoveWithNonExistingFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());

            collection.Remove("d");
            matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());
        }

        [Test]
        public void RemoveWithNullFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());

            collection.Remove(null);
            matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());
        }

        [Test]
        public void Update()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule1 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Update(path, rule2);

            matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule2, matchingRules.First());
        }

        [Test]
        public void UpdateWithEmptyFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule1 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentException>(() => collection.Update(string.Empty, rule2));
        }

        [Test]
        public void UpdateWithNonExistingFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule1 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Update("d", rule2);

            matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());
        }

        [Test]
        public void UpdateWithNullFilePath()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule1 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, rule2));
        }

        [Test]
        public void UpdateWithNullRule()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var collection = new RuleCollection(diagnostics);

            var path = "a";
            var rule1 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(new SensorId("b"), new ActionId("c"), new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentNullException>(() => collection.Update(path, null));
        }
    }
}
