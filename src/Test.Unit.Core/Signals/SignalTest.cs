//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Metamorphic.Core.Signals
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public class SignalTest
    {
        [Test]
        public void Create()
        {
            var type = new SignalTypeId("a");
            var signal = new Signal(type, new Dictionary<string, object>());

            Assert.AreSame(type, signal.Sensor);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Signals.Signal",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullParameters()
        {
            Assert.Throws<ArgumentNullException>(() => new Signal(new SignalTypeId("a"), null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Signals.Signal",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullSensorId()
        {
            Assert.Throws<ArgumentNullException>(() => new Signal(null, new Dictionary<string, object>()));
        }

        [Test]
        public void ContainsParameterWithNullString()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.IsFalse(signal.ContainsParameter(null));
        }

        [Test]
        public void ContainsParameterWithEmptyString()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.IsFalse(signal.ContainsParameter(string.Empty));
        }

        [Test]
        public void ContainsParameterWithNonExistingParameter()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.IsFalse(signal.ContainsParameter("c"));
        }

        [Test]
        public void ContainsParameterWithExistingParameter()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.IsTrue(signal.ContainsParameter("a"));
        }

        [Test]
        public void ParameterValueWithNullString()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => signal.ParameterValue(null));
        }

        [Test]
        public void ParameterValueWithEmptyString()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => signal.ParameterValue(string.Empty));
        }

        [Test]
        public void ParameterValueWithNonExistingParameter()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => signal.ParameterValue("c"));
        }

        [Test]
        public void ParameterValueWithExistingParameter()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            Assert.AreEqual("b", signal.ParameterValue("a"));
        }

        [Test]
        public void ToDataObject()
        {
            var typeId = "a";
            var type = new SignalTypeId(typeId);
            var parameters = new Dictionary<string, object>
                {
                    { "A", "b" }
                };
            var signal = new Signal(type, parameters);

            var obj = ((ITranslateToDataObject<SignalData>)signal).ToDataObject();
            Assert.AreEqual(typeId, obj.SensorId);
            Assert.That(obj.Parameters, Is.EquivalentTo(parameters));
        }
    }
}
