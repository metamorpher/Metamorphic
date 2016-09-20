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
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Test.Unit.Sensor.Http.Controllers
{
    [TestFixture]
    public sealed class TfsWorkItemControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsWorkItemController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullPublisher()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new TfsWorkItemController(null, diagnostics));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsWorkItemController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            var publisher = new Mock<IPublishSignals>();
            Assert.Throws<ArgumentNullException>(() => new TfsWorkItemController(publisher.Object, null));
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
            var controller = new TfsWorkItemController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""61fa58ab - dced - 4b53 - a9dd - a987a3517216"",
    ""notificationId"": 8266,
    ""id"": ""27646e0e-b520-4d2b-9411-bba7524947cd"",
    ""eventType"": ""workitem.updated"",
    ""publisherId"": ""tfs"",
    ""message"": {
      ""text"": ""Bug #5 (Some great new idea!) updated by Jamal Hartnett.\r\n(http://fabrikam-fiber-inc.visualstudio.com/web/wi.aspx?pcguid=74e918bf-3376-436d-bd20-8e8c1287f465&id=5)""
    },
    ""detailedMessage"": {
      ""text"": ""Bug #5 (Some great new idea!) updated by Jamal Hartnett.\r\n(http://fabrikam-fiber-inc.visualstudio.com/web/wi.aspx?pcguid=74e918bf-3376-436d-bd20-8e8c1287f465&id=5)\r\n\r\n- New State: Approved\r\n""
    },
    ""resource"": {
      ""id"": 2,
      ""workItemId"": 0,
      ""rev"": 2,
      ""revisedBy"": null,
      ""revisedDate"": ""0001-01-01T00:00:00"",
      ""fields"": {
        ""System.Rev"": {
          ""oldValue"": ""1"",
          ""newValue"": ""2""
        },
        ""System.AuthorizedDate"": {
          ""oldValue"": ""2014-07-15T16:48:44.663Z"",
          ""newValue"": ""2014-07-15T17:42:44.663Z""
        },
        ""System.RevisedDate"": {
          ""oldValue"": ""2014-07-15T17:42:44.663Z"",
          ""newValue"": ""9999-01-01T00:00:00Z""
        },
        ""System.State"": {
          ""oldValue"": ""New"",
          ""newValue"": ""Approved""
        },
        ""System.Reason"": {
          ""oldValue"": ""New defect reported"",
          ""newValue"": ""Approved by the Product Owner""
        },
        ""System.AssignedTo"": {
          ""newValue"": ""Jamal Hartnet""
        },
        ""System.ChangedDate"": {
          ""oldValue"": ""2014-07-15T16:48:44.663Z"",
          ""newValue"": ""2014-07-15T17:42:44.663Z""
        },
        ""System.Watermark"": {
          ""oldValue"": ""2"",
          ""newValue"": ""5""
        },
        ""Microsoft.VSTS.Common.Severity"": {
          ""oldValue"": ""3 - Medium"",
          ""newValue"": ""2 - High""
        }
    },
    ""_links"": {
    ""self"": {
          ""href"": ""http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/updates/2""
        },
        ""parent"": {
          ""href"": ""http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5""
        },
        ""workItemUpdates"": {
          ""href"": ""http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/updates""
        }
    },
    ""url"": ""http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/updates/2"",
    ""revision"": {
      ""id"": 5,
      ""rev"": 2,
      ""fields"": {
        ""System.AreaPath"": ""FabrikamCloud"",
        ""System.TeamProject"": ""FabrikamCloud"",
        ""System.IterationPath"": ""FabrikamCloud\\Release 1\\Sprint 1"",
        ""System.WorkItemType"": ""Bug"",
        ""System.State"": ""New"",
        ""System.Reason"": ""New defect reported"",
        ""System.CreatedDate"": ""2014-07-15T16:48:44.663Z"",
        ""System.CreatedBy"": ""Jamal Hartnett"",
        ""System.ChangedDate"": ""2014-07-15T16:48:44.663Z"",
        ""System.ChangedBy"": ""Jamal Hartnett"",
        ""System.Title"": ""Some great new idea!"",
        ""Microsoft.VSTS.Common.Severity"": ""3 - Medium"",
        ""WEF_EB329F44FE5F4A94ACB1DA153FDF38BA_Kanban.Column"": ""New""
      },
      ""url"": ""http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/revisions/2""
    }
  },
  ""createdDate"": ""2016-09-20T04:50:18.6739333Z""
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
            Assert.AreEqual("TfsWorkItemChange", data.SensorId);
            Assert.AreEqual(7, data.Parameters.Count);
            Assert.AreEqual("5", data.Parameters["ID"]);
            Assert.AreEqual("New", data.Parameters["STATE"]);
            Assert.AreEqual(string.Empty, data.Parameters["ASSIGNEDTO"]);
            Assert.AreEqual("Some great new idea!", data.Parameters["TITLE"]);
            Assert.AreEqual("Bug", data.Parameters["TYPE"]);
            Assert.AreEqual("FabrikamCloud", data.Parameters["AREAPATH"]);
            Assert.AreEqual("FabrikamCloud", data.Parameters["TEAMPROJECT"]);
        }
    }
}
