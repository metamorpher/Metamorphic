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
        public void CreateWithEmptyParameterFormat()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterValue("a", string.Empty, new List<string>()));
        }

        [Test]
        public void CreateWithNullParameterFormat()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue("a", (string)null, new List<string>()));
        }

        [Test]
        public void CreateWithNullParameterList()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue("a", "b", null));
        }

        [Test]
        public void CreateWithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue("a", null));
        }

        [Test]
        public void CreateWithParameterFormatAndEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterValue(string.Empty, "a", new List<string>()));
        }

        [Test]
        public void CreateWithParameterFormatAndNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue(null, "a", new List<string>()));
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
        public void IsValidForSignalWithMultipleMatchingParameterValueWithCondition()
        {
            var parameter1 = "a";
            var parameter2 = "b";
            var parameterValue1 = "10";
            Predicate<object> condition1 = o => o.Equals(parameterValue1);
            var reference = new ActionParameterValue(
                "c",
                "{{signal." + parameter1 + "}} {{signal." + parameter2 + "}}",
                new List<string>
                {
                    parameter1,
                    parameter2
                },
                new Dictionary<string, Predicate<object>>
                {
                    [parameter1] = condition1
                });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    [parameter1] = "10",
                    [parameter2] = "15"
                });
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithMultipleMatchingParameterValueWithoutCondition()
        {
            var parameter1 = "a";
            var parameter2 = "b";
            var reference = new ActionParameterValue(
                "c", 
                "{{signal." + parameter1 + "}} {{signal." + parameter2 + "}}", 
                new List<string>
                {
                    parameter1,
                    parameter2
                });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    [parameter1] = "10",
                    [parameter2] = "15"
                });
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithNonMatchingParameterValue()
        {
            var parameterName = "a";
            var parameterValue = "11";
            Predicate<object> condition = o => o.Equals(parameterValue);
            var reference = new ActionParameterValue(
                "b",
                "{{signal." + parameterName + "}}",
                new List<string>
                {
                    parameterName
                },
                new Dictionary<string, Predicate<object>>
                {
                    [parameterName] = condition
                });

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
            var reference = new ActionParameterValue("b", "{{signal." + parameterName + "}}", new List<string> { parameterName });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>());
            Assert.IsFalse(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithSingleMatchingParameterValueWithCondition()
        {
            var parameterName = "a";
            var parameterValue = "10";
            Predicate<object> condition = o => o.Equals(parameterValue);
            var reference = new ActionParameterValue(
                "b", 
                "{{signal." + parameterName + "}}", 
                new List<string>
                {
                    parameterName
                }, 
                new Dictionary<string, Predicate<object>>
                {
                    [parameterName] = condition
                });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "10" }
                });
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithSingleMatchingParameterValueWithoutCondition()
        {
            var parameterName = "a";
            var reference = new ActionParameterValue("b", "{{signal." + parameterName + "}}", new List<string> { parameterName });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    { parameterName, "10" }
                });
            Assert.IsTrue(reference.IsValidFor(signal));
        }

        [Test]
        public void ValueForMultipleParameterWithoutDefaultValue()
        {
            var parameter1 = "a";
            var parameterValue1 = "10";
            var parameter2 = "b";
            var parameterValue2 = "11";
            var reference = new ActionParameterValue(
                "c",
                "{{signal." + parameter1 + "}}-{{signal." + parameter2 + "}}",
                new List<string>
                {
                    parameter1,
                    parameter2
                });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    [parameter1] = parameterValue1,
                    [parameter2] = parameterValue2
                });
            Assert.AreEqual(string.Format("{0}-{1}", parameterValue1, parameterValue2), reference.ValueForParameter(signal));
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
        public void ValueForSingleParameterWithoutDefaultValue()
        {
            var parameterName = "a";
            var parameterValue = 10;
            var reference = new ActionParameterValue("b", "{{signal." + parameterName + "}}", new List<string> { parameterName });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>
                {
                    [parameterName] = parameterValue
                });
            Assert.AreEqual(parameterValue, reference.ValueForParameter(signal));
        }
    }
}
