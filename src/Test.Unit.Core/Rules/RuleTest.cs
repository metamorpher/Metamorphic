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
using Metamorphic.Core.Signals;
using NUnit.Framework;

namespace Metamorphic.Core.Rules
{
    [TestFixture]
    public sealed class RuleTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Rules.Rule",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullActionId()
        {
            var sensorId = new SignalTypeId("a");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule("a", "b", sensorId, null, conditions, parameters));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Rules.Rule",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullConditionCollection()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(name, description, sensorId, actionId, null, parameters));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Rules.Rule",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDescription()
        {
            var name = "a";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(name, null, sensorId, actionId, conditions, parameters));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Rules.Rule",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullName()
        {
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(null, description, sensorId, actionId, conditions, parameters));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Rules.Rule",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullReferenceCollection()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            Assert.Throws<ArgumentNullException>(() => new Rule(name, description, sensorId, actionId, conditions, null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Rules.Rule",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullSensorId()
        {
            var name = "a";
            var description = "b";
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(name, description, null, actionId, conditions, parameters));
        }

        [Test]
        public void ShouldProcessWithBlockingCondition()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>
            {
                ["a"] = (object obj) => false,
            };
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(2),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcessWithMissingParameters()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("{{signal.a}}", new List<string> { "a" }),
                ["b"] = new ActionParameterValue("{{signal.b}}", new List<string> { "b" }),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcessWithNonmatchingParameters()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(
                    "{{signal.b}}",
                    new List<string> { "b" }),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcessWithNonmatchingSensorId()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(2),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                new SignalTypeId("e"),
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcess()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(2),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsTrue(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcessWithNullSignal()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(1),
                ["b"] = new ActionParameterValue(2),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            Assert.IsFalse(rule.ShouldProcess(null));
        }

        [Test]
        public void ToJobWithDefaultValue()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");

            var parameterValue = 10;
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(parameterValue),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            var job = rule.ToJob(signal);
            Assert.AreSame(actionId, job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("a"));
            Assert.AreEqual(parameterValue, job.ParameterValue("a"));
        }

        [Test]
        public void ToJobWithInvalidSignal()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(
                    "{{signal.b}}",
                    new List<string> { "b" }),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.Throws<InvalidSignalForRuleException>(() => rule.ToJob(signal));
        }

        [Test]
        public void ToJobWithNullSignal()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");
            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(1),
                ["b"] = new ActionParameterValue(2),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            Assert.Throws<InvalidSignalForRuleException>(() => rule.ToJob(null));
        }

        [Test]
        public void ToJobWithSignalValue()
        {
            var name = "a";
            var description = "b";
            var sensorId = new SignalTypeId("c");
            var actionId = new ActionId("d");

            var conditions = new Dictionary<string, Predicate<object>>();
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(1),
            };
            var rule = new Rule(name, description, sensorId, actionId, conditions, parameters);

            var parameterValue = 1;
            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreSame(actionId, job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("a"));
            Assert.AreEqual(parameterValue, job.ParameterValue("a"));
        }
    }
}
