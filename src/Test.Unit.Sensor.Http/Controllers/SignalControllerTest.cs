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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Metamorphic.Core;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Metamorphic.Sensor.Http;
using Metamorphic.Sensor.Http.Controllers;
using Moq;
using Newtonsoft.Json.Linq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Test.Unit.Sensor.Http.Controllers
{
    [TestFixture]
    public class SignalControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.SignalController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullPublisher()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new SignalController(null, diagnostics));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.SignalController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            var publisher = new Mock<IPublishSignals>();
            Assert.Throws<ArgumentNullException>(() => new SignalController(publisher.Object, null));
        }

        [Test]
        public void Get()
        {
            var publisher = new Mock<IPublishSignals>();
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            SignalController controller = new SignalController(publisher.Object, diagnostics);

            var result = controller.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void Post()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            SignalController controller = new SignalController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""Type"" : ""SignalId"",
    ""Parameter_1"" : ""Parameter_1_Value"",
    ""Parameter_2"" : true,
    ""Parameter_3"" : 10
}";
            var token = JToken.Parse(jsonText);
            controller.Post(token);

            publisher.Verify(p => p.Publish(It.IsAny<Signal>()), Times.Once());

            var data = ((ITranslateToDataObject<SignalData>)capturedSignal).ToDataObject();
            Assert.AreEqual("SignalId", data.SensorId);
            Assert.AreEqual(3, data.Parameters.Count);
            Assert.AreEqual("Parameter_1_Value", data.Parameters["Parameter_1"]);
            Assert.AreEqual(true, data.Parameters["Parameter_2"]);
            Assert.AreEqual(10, data.Parameters["Parameter_3"]);
        }
    }
}
