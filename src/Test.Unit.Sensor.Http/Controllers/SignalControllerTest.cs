//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Metamorphic.Core;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Metamorphic.Sensor.Http.Controllers;
using Moq;
using Newtonsoft.Json.Linq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Test.Unit.Sensor.Http.Controllers
{
    [TestFixture]
    public sealed class SignalControllerTest
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

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://localhost/api/signal"));
            controller.ControllerContext = new HttpControllerContext();
            controller.ControllerContext.Configuration = new HttpConfiguration();
            controller.ControllerContext.Request = request;
            controller.Request = request;

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
    ""Parameter_3"" : 10,
    ""Parameter_4"" : 2.34
}";

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://localhost/api/signal"));
            request.Content = new StringContent(jsonText);

            controller.ControllerContext = new HttpControllerContext();
            controller.ControllerContext.Configuration = new HttpConfiguration();
            controller.ControllerContext.Request = request;
            controller.Request = request;

            controller.Post();

            publisher.Verify(p => p.Publish(It.IsAny<Signal>()), Times.Once());

            var data = ((ITranslateToDataObject<SignalData>)capturedSignal).ToDataObject();
            Assert.AreEqual("SignalId", data.SensorId);
            Assert.AreEqual(4, data.Parameters.Count);
            Assert.AreEqual("Parameter_1_Value", data.Parameters["PARAMETER_1"]);
            Assert.AreEqual(true, data.Parameters["PARAMETER_2"]);
            Assert.AreEqual(10, data.Parameters["PARAMETER_3"]);
            Assert.AreEqual(2.34, data.Parameters["PARAMETER_4"]);
        }
    }
}
