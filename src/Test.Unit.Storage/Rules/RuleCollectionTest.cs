//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
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
            var collection = new RuleCollection();

            var path = "a";
            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());
        }

        [Test]
        public void AddWithEmptyFilePath()
        {
            var collection = new RuleCollection();

            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentException>(() => collection.Add(string.Empty, rule));
        }

        [Test]
        public void AddWithExistingFilePath()
        {
            var collection = new RuleCollection();

            var sensor = new SignalTypeId("b");
            var path = "a";
            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule);

            var matchingRules = collection.RulesForSignal(rule.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule, matchingRules.First());

            var path2 = "a";
            var rule2 = new Rule(
                "a",
                "b",
                sensor,
                new ActionId("e"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            Assert.Throws<RuleAlreadyExistsException>(() => collection.Add(path2, rule2));
        }

        [Test]
        public void AddWithExistingSensorId()
        {
            var collection = new RuleCollection();

            var sensor = new SignalTypeId("b");
            var path1 = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path1, rule1);

            var matchingRules = collection.RulesForSignal(sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var path2 = "d";
            var rule2 = new Rule(
                "a",
                "b",
                sensor,
                new ActionId("e"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path2, rule2);

            matchingRules = collection.RulesForSignal(sensor);
            Assert.That(matchingRules, Is.EquivalentTo(new[] { rule1, rule2 }));
        }

        [Test]
        public void AddWithNullFilePath()
        {
            var collection = new RuleCollection();

            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentNullException>(() => collection.Add(null, rule));
        }

        [Test]
        public void AddWithNullRule()
        {
            var collection = new RuleCollection();

            Assert.Throws<ArgumentNullException>(() => collection.Add("a", null));
        }

        [Test]
        public void Remove()
        {
            var collection = new RuleCollection();

            var path = "a";
            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
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
            var collection = new RuleCollection();

            var path = "a";
            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
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
            var collection = new RuleCollection();

            var sensor = new SignalTypeId("b");
            var path1 = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path1, rule1);

            var matchingRules = collection.RulesForSignal(sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var path2 = "d";
            var rule2 = new Rule(
                "a",
                "b",
                sensor,
                new ActionId("e"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
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
            var collection = new RuleCollection();

            var path = "a";
            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
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
            var collection = new RuleCollection();

            var path = "a";
            var rule = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
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
            var collection = new RuleCollection();

            var path = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Update(path, rule2);

            matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule2, matchingRules.First());
        }

        [Test]
        public void UpdateWithEmptyFilePath()
        {
            var collection = new RuleCollection();

            var path = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentException>(() => collection.Update(string.Empty, rule2));
        }

        [Test]
        public void UpdateWithNonExistingFilePath()
        {
            var collection = new RuleCollection();

            var path = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Update("d", rule2);

            matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());
        }

        [Test]
        public void UpdateWithNullFilePath()
        {
            var collection = new RuleCollection();

            var path = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            var rule2 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, rule2));
        }

        [Test]
        public void UpdateWithNullRule()
        {
            var collection = new RuleCollection();

            var path = "a";
            var rule1 = new Rule(
                "a",
                "b",
                new SignalTypeId("b"),
                new ActionId("c"),
                new Dictionary<string, Predicate<object>>(),
                new Dictionary<string, ActionParameterValue>());
            collection.Add(path, rule1);

            var matchingRules = collection.RulesForSignal(rule1.Sensor);
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(rule1, matchingRules.First());

            Assert.Throws<ArgumentNullException>(() => collection.Update(path, null));
        }
    }
}
