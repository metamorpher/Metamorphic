//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Metamorphic.Core.Signals;
using NUnit.Framework;

namespace Metamorphic.Server
{
    [TestFixture]
    public sealed class ActionParameterValueTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.ActionParameterValue",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyParameterFormat()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterValue(string.Empty, new List<string>()));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.ActionParameterValue",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullParameterFormat()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue(null, new List<string>()));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.ActionParameterValue",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullParameterList()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue("b", null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.ActionParameterValue",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValue(null));
        }

        [Test]
        public void IsValidForNullSignal()
        {
            var reference = new ActionParameterValue(10);
            Assert.IsFalse(reference.IsValidFor(null));
        }

        [Test]
        public void IsValidForSignalWithMultipleMatchingParameterValue()
        {
            var parameter1 = "a";
            var parameter2 = "b";
            var reference = new ActionParameterValue(
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
        public void IsValidForSignalWithMissingParameter()
        {
            var parameterName = "a";
            var reference = new ActionParameterValue("{{signal." + parameterName + "}}", new List<string> { parameterName });

            var signal = new Signal(
                new SignalTypeId("b"),
                new Dictionary<string, object>());
            Assert.IsFalse(reference.IsValidFor(signal));
        }

        [Test]
        public void IsValidForSignalWithSingleMatchingParameterValueWithoutCondition()
        {
            var parameterName = "a";
            var reference = new ActionParameterValue("{{signal." + parameterName + "}}", new List<string> { parameterName });

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
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", parameterValue1, parameterValue2), reference.ValueForParameter(signal));
        }

        [Test]
        public void ValueForParameterWithDefaultValue()
        {
            var parameterName = "a";
            var parameterValue = 10;
            var reference = new ActionParameterValue(parameterValue);

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
            var reference = new ActionParameterValue("{{signal." + parameterName + "}}", new List<string> { parameterName });

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
