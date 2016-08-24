//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
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
    public sealed class JenkinsControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.JenkinsController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullPublisher()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new JenkinsController(null, diagnostics));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.JenkinsController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            var publisher = new Mock<IPublishSignals>();
            Assert.Throws<ArgumentNullException>(() => new JenkinsController(publisher.Object, null));
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
            var controller = new JenkinsController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""name"": ""BUILDNAME"",
    ""url"": ""job/Release/job/BUILD_NAME/"",
    ""build"": {
        ""full_url"": ""http://myserver/job/Release/job/BUILD_NAME/BUILD_NUMBER/"",
        ""number"": 10,
        ""queue_id"": 1693,
        ""phase"": ""FINALIZED"",
        ""status"": ""SUCCESS"",
        ""url"": ""job/Release/job/BUILD_NAME/BUILD_NUMBER/"",
        ""scm"": { },
        ""parameters"": {
            ""PARAMETER_1"": ""PARAMETER_1_VALUE"",
            ""PARAMETER_2"": true,
            ""PARAMETER_3"": 10
        },
        ""log"": """",
        ""artifacts"": { }
    }
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
            Assert.AreEqual("JenkinsJobComplete", data.SensorId);
            Assert.AreEqual(6, data.Parameters.Count);
            Assert.AreEqual("PARAMETER_1_VALUE", data.Parameters["PARAMETER_1"]);
            Assert.AreEqual(true, data.Parameters["PARAMETER_2"]);
            Assert.AreEqual(10, data.Parameters["PARAMETER_3"]);
            Assert.AreEqual("BUILDNAME", data.Parameters["JOBNAME"]);
            Assert.AreEqual("SUCCESS", data.Parameters["JOBSTATUS"]);
            Assert.AreEqual("http://myserver/job/Release/job/BUILD_NAME/BUILD_NUMBER/", data.Parameters["JOBURL"]);
        }
    }
}
