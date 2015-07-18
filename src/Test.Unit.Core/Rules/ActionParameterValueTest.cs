//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Metamorphic.Core.Signals;
using NUnit.Framework;

namespace Metamorphic.Core.Rules
{
    [TestFixture]
    public sealed class ActionParameterValueTest
    {
        [Test]
        public void CreateWithEmptyParameterReference()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterValue("a", string.Empty));
        }

        [Test]
        public void CreateWithNullParameterReference()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue("a", (string)null));
        }

        [Test]
        public void CreateWithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue("a", (object)null));
        }

        [Test]
        public void CreateWithParameterReferenceAndEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterValue(string.Empty, "a"));
        }

        [Test]
        public void CreateWithParameterReferenceAndNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue(null, "a"));
        }

        [Test]
        public void CreateWithValueAndEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterValue(string.Empty, 10));
        }

        [Test]
        public void CreateWithValueAndNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue(null, 10));
        }

        [Test]
        public void IsValidForNullSignal()
        {
            var parameterName = "a";
            var reference = new ActionParameterValue(parameterName, 10);
            Assert.IsFalse(reference.IsValidFor(null));
        }

        [Test]
        public void IsValidForSignalWithMatchingParameterValueWithCondition()
        {
            var parameterName = "a";
            var parameterValue = "10";
            Predicate<object> condition = o => o.Equals(parameterValue);
            var reference = new ActionParameterValue("b", parameterName, condition: condition);

            var signal = new Signal(
                new SignalTypeId("b"),
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
            var reference = new ActionParameterValue("b", parameterName);

            var signal = new Signal(
                new SignalTypeId("b"),
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
            var reference = new ActionParameterValue("b", parameterName, condition: condition);

            var signal = new Signal(
                new SignalTypeId("b"),
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
            var reference = new ActionParameterValue("b", parameterName);

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>());
            Assert.IsFalse(reference.IsValidFor(signal));
        }

        [Test]
        public void ValueForParameterWithDefaultValue()
        {
            var parameterName = "a";
            var parameterValue = 10;
            var reference = new ActionParameterValue(parameterName, parameterValue);

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "100" }
                });
            Assert.AreEqual(parameterValue, reference.ValueForParameter(signal));
        }

        [Test]
        public void ValueForParameterWithoutDefaultValue()
        {
            var parameterName = "a";
            var parameterValue = 10;
            var reference = new ActionParameterValue("b", parameterName);

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, parameterValue }
                });
            Assert.AreEqual(parameterValue, reference.ValueForParameter(signal));
        }
    }
}
