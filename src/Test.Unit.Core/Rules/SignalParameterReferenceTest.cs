//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Sensors;
using Metamorphic.Core.Signals;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Metamorphic.Core.Rules
{
    [TestFixture]
    public sealed class SignalParameterReferenceTest
    {
        [Test]
        public void CreateWithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new SignalParameterReference(string.Empty));
        }

        [Test]
        public void CreateWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new SignalParameterReference(null));
        }

        [Test]
        public void IsValidForNullSignal()
        {
            var parameterName = "a";
            var reference = new SignalParameterReference(parameterName);
            Assert.IsFalse(reference.IsValidFor(null));
        }

        [Test]
        public void IsValidForSignalWithMatchingParameterValueWithCondition()
        {
            var parameterName = "a";
            var parameterValue = "10";
            Predicate<object> condition = o => o.Equals(parameterValue);
            var reference = new SignalParameterReference(parameterName, condition: condition);

            var signal = new Signal(
                new SensorId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "10" }
                });
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithMatchingParameterValueWithoutCondition()
        {
            var parameterName = "a";
            var reference = new SignalParameterReference(parameterName);

            var signal = new Signal(
                new SensorId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "10" }
                });
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithNonMatchingParameterValue()
        {
            var parameterName = "a";
            var parameterValue = "11";
            Predicate<object> condition = o => o.Equals(parameterValue);
            var reference = new SignalParameterReference(parameterName, condition: condition);

            var signal = new Signal(
                new SensorId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "10" }
                });
            Assert.IsFalse(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithMissingParameter()
        {
            var parameterName = "a";
            var reference = new SignalParameterReference(parameterName);

            var signal = new Signal(
                new SensorId("b"),
                new Dictionary<string, object>());
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void ValueForParameterWithDefaultValue()
        {
            var parameterName = "a";
            var parameterValue = 10;
            var reference = new SignalParameterReference(parameterName, parameterValue: parameterValue);

            var signal = new Signal(
                new SensorId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "100" }
                });
            Assert.AreSame(parameterValue, reference.ValueForParameter(signal));
        }

        [Test]
        public void ValueForParameterWithoutDefaultValue()
        {
            var parameterName = "a";
            var parameterValue = 10;
            var reference = new SignalParameterReference(parameterName);

            var signal = new Signal(
                new SensorId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, parameterValue }
                });
            Assert.AreSame(parameterValue, reference.ValueForParameter(signal));
        }
    }
}
