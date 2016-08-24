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
    public sealed class TfsControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullPublisher()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new TfsController(null, diagnostics));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            var publisher = new Mock<IPublishSignals>();
            Assert.Throws<ArgumentNullException>(() => new TfsController(publisher.Object, null));
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
            var controller = new TfsController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""a24542e8 - 393a - 4acb - b9d2 - 9417d72dd639"",
    ""notificationId"": 1,
    ""id"": ""f045fd98-3c2f-4e27-98be-285da87c25b4"",
    ""eventType"": ""build.complete"",
    ""publisherId"": ""tfs"",
    ""message"": {
        ""text"": ""Build BuildName_1.2.3_20191225.7 has been canceled""
            },
    ""detailedMessage"": {
        ""text"": ""Build BuildName_1.2.3_20191225.7 has been canceled""
            },
    ""resource"": {
        ""uri"": ""vstfs:///Build/Build/393935"",
        ""id"": 393935,
        ""buildNumber"": ""BuildName_1.2.3_20191225.7"",
        ""url"": ""http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Builds/393935"",
        ""startTime"": ""2016-08-11T03:04:09.87Z"",
        ""finishTime"": ""2016-08-15T22:31:42.817Z"",
        ""reason"": ""manual"",
        ""status"": ""stopped"",
        ""dropLocation"": ""\\\\tfsbuilds\\TFSBuilds\\BuildName_1.2.3\\BuildName_1.2.3_20191225.7"",
        ""drop"": {
        ""location"": ""\\\\tfsbuilds\\TFSBuilds\\BuildName_1.2.3\\BuildName_1.2.3_20191225.7"",
            ""type"": ""localPath"",
            ""url"": ""file://///tfsbuilds/TFSBuilds/BuildName_1.2.3/BuildName_1.2.3_20191225.7"",
            ""downloadUrl"": ""file://///tfsbuilds/TFSBuilds/BuildName_1.2.3/BuildName_1.2.3_20191225.7""
        },
        ""log"": {
        ""type"": ""localPath"",
            ""url"": ""file://///tfsbuilds/TFSBuilds/BuildName_1.2.3/BuildName_1.2.3_20191225.7/logs"",
            ""downloadUrl"": ""file://///tfsbuilds/TFSBuilds/BuildName_1.2.3/BuildName_1.2.3_20191225.7/logs""
        },
        ""sourceGetVersion"": ""C185823"",
        ""lastChangedBy"": {
        ""id"": ""ecff947f-38e4-498d-8740-6bfbfc995f0d"",
            ""displayName"": ""John Doe"",
            ""uniqueName"": ""MYDOMAIN\\John.Doe"",
            ""url"": ""http://zzz:8080/tfs/MyProject/_apis/Identities/ecff947f-38e4-498d-8740-6bfbfc995f0d"",
            ""imageUrl"": ""http://zzz:8080/tfs/MyProject/_api/_common/identityImage?id=ecff947f-38e4-498d-8740-6bfbfc995f0d""
        },
        ""retainIndefinitely"": false,
        ""hasDiagnostics"": true,
        ""definition"": {
        ""definitionType"": ""xaml"",
            ""id"": 2060,
            ""name"": ""FranceCountryPack_4.5.8"",
            ""url"": ""http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Definitions/2060""
        },
        ""queue"": {
        ""queueType"": ""buildController"",
            ""id"": 12,
            ""name"": ""Default Controller - mymachine"",
            ""url"": ""http://zzz:8080/tfs/MyProject/_apis/build/Queues/12""
        },
        ""requests"": [
            {
                ""id"": 480801,
                ""url"": ""http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Requests/480801"",
                ""requestedFor"": {
                    ""id"": ""dd44aced-2435-4fa8-a9f5-c0ca9d812fe8"",
                    ""displayName"": ""TFS Build Service"",
                    ""uniqueName"": ""MYDOMAIN\\BUILDAGENT""
                }
            }
        ]
    },
    ""resourceVersion"": ""1.0"",
    ""createdDate"": ""2016-08-15T22:31:46.8089765Z""
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
            Assert.AreEqual("TfsJobComplete", data.SensorId);
            Assert.AreEqual(4, data.Parameters.Count);
            Assert.AreEqual("393935", data.Parameters["JOBID"]);
            Assert.AreEqual("BuildName_1.2.3", data.Parameters["JOBNAME"]);
            Assert.AreEqual("stopped", data.Parameters["JOBSTATUS"]);
            Assert.AreEqual("http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Builds/393935", data.Parameters["JOBURL"]);
        }
    }
}
