//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public void CreateWithNullActionId()
        {
            var sensorId = new SignalTypeId("a");
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(sensorId, null, parameters));
        }

        [Test]
        public void CreateWithNullReferenceCollection()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            Assert.Throws<ArgumentNullException>(() => new Rule(sensorId, actionId, null));
        }

        [Test]
        public void CreateWithNullSensorId()
        {
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(null, actionId, parameters));
        }

        [Test]
        public void ShouldProcessWithMissingParameters()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", "{{signal.a}}", new List<string> { "a" }),
                ["b"] = new ActionParameterValue("b", "{{signal.b}}", new List<string> { "b" }),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcessWithNonMatchingParameters()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(
                    "a", 
                    "{{signal.b}}", 
                    new List<string> { "b" }, 
                    new Dictionary<string, Predicate<object>>
                    {
                        ["b"] = o => false
                    }),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            var signal = new Signal(
                sensorId,
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcessWithNonMatchingSensorId()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", 2),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            var signal = new Signal(
                new SignalTypeId("c"),
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcess()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", 2),
            };
            var rule = new Rule(sensorId, actionId, parameters);

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
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", 1),
                ["b"] = new ActionParameterValue("b", 2),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            Assert.IsFalse(rule.ShouldProcess(null));
        }

        [Test]
        public void ToJobWithDefaultValue()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");

            var parameterValue = 10;
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", parameterValue),
            };
            var rule = new Rule(sensorId, actionId, parameters);

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
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue(
                    "a",
                    "{{signal.b}}",
                    new List<string> { "b" },
                    new Dictionary<string, Predicate<object>>
                    {
                        ["b"] = o => false
                    }),
            };
            var rule = new Rule(sensorId, actionId, parameters);

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
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", 1),
                ["b"] = new ActionParameterValue("b", 2),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            Assert.Throws<InvalidSignalForRuleException>(() => rule.ToJob(null));
        }

        [Test]
        public void ToJobWithSignalValue()
        {
            var sensorId = new SignalTypeId("a");
            var actionId = new ActionId("b");

            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", 1),
            };
            var rule = new Rule(sensorId, actionId, parameters);

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
