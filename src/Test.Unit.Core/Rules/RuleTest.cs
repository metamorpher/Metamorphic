//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Actions;
using Metamorphic.Core.Sensors;
using Metamorphic.Core.Signals;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metamorphic.Core.Rules
{
    [TestFixture]
    public sealed class RuleTest
    {
        [Test]
        public void CreateWithNullActionId()
        {
            var sensorId = new SensorId("a");
            var parameters = new Dictionary<string, ActionParameterValue>();
            Assert.Throws<ArgumentNullException>(() => new Rule(sensorId, null, parameters));
        }

        [Test]
        public void CreateWithNullReferenceCollection()
        {
            var sensorId = new SensorId("a");
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
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a"),
                ["b"] = new ActionParameterValue("b"),
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
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", condition:o => false),
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
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a"),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            var signal = new Signal(
                new SensorId("c"),
                new Dictionary<string, object>
                {
                    ["a"] = 1
                });
            Assert.IsFalse(rule.ShouldProcess(signal));
        }

        [Test]
        public void ShouldProcess()
        {
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a"),
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
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a"),
                ["b"] = new ActionParameterValue("b"),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            Assert.IsFalse(rule.ShouldProcess(null));
        }

        [Test]
        public void ToJobWithDefaultValue()
        {
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");

            var parameterValue = 10;
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", parameterValue: parameterValue),
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
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a", condition: o => false),
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
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");
            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a"),
                ["b"] = new ActionParameterValue("b"),
            };
            var rule = new Rule(sensorId, actionId, parameters);

            Assert.Throws<InvalidSignalForRuleException>(() => rule.ToJob(null));
        }

        [Test]
        public void ToJobWithSignalValue()
        {
            var sensorId = new SensorId("a");
            var actionId = new ActionId("b");

            var parameters = new Dictionary<string, ActionParameterValue>
            {
                ["a"] = new ActionParameterValue("a"),
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
