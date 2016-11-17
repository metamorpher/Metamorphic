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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    public sealed class TfsPullRequestControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsPullRequestController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullPublisher()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new TfsPullRequestController(null, diagnostics));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsPullRequestController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            var publisher = new Mock<IPublishSignals>();
            Assert.Throws<ArgumentNullException>(() => new TfsPullRequestController(publisher.Object, null));
        }

        [Test]
        public void PostWithCreatedPullRequest()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsPullRequestController(publisher.Object, diagnostics);

            var jsonText = @"
{
  ""subscriptionId"": ""00000000-0000-0000-0000-000000000000"",
  ""notificationId"": 50,
  ""id"": ""2ab4e3d3-b7a6-425e-92b1-5a9982c1269e"",
  ""eventType"": ""git.pullrequest.created"",
  ""publisherId"": ""tfs"",
  ""scope"": ""all"",
  ""message"": {
    ""text"": ""Jamal Hartnett created a new pull request""
  },
  ""detailedMessage"": {
    ""text"": ""Jamal Hartnett created a new pull request\r\n\r\n- Merge status: Succeeded\r\n- Merge commit: eef717(https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/eef717f69257a6333f221566c1c987dc94cc0d72)\r\n""
  },
  ""resource"": {
    ""repository"": {
      ""id"": ""4bc14d40-c903-45e2-872e-0462c7748079"",
      ""name"": ""Fabrikam"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079"",
      ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam"",
        ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
      },
      ""defaultBranch"": ""refs/heads/master"",
      ""remoteUrl"": ""https://fabrikam.visualstudio.com/DefaultCollection/_git/Fabrikam""
    },
    ""pullRequestId"": 1,
    ""status"": ""active"",
    ""createdBy"": {
      ""id"": ""54d125f7-69f7-4191-904f-c5b96b6261c8"",
      ""displayName"": ""Jamal Hartnett"",
      ""uniqueName"": ""fabrikamfiber4@hotmail.com"",
      ""url"": ""https://fabrikam.vssps.visualstudio.com/_apis/Identities/54d125f7-69f7-4191-904f-c5b96b6261c8"",
      ""imageUrl"": ""https://fabrikam.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=54d125f7-69f7-4191-904f-c5b96b6261c8""
    },
    ""creationDate"": ""2014-06-17T16:55:46.589889Z"",
    ""title"": ""my first pull request"",
    ""description"": "" - test2\r\n"",
    ""sourceRefName"": ""refs/heads/mytopic"",
    ""targetRefName"": ""refs/heads/master"",
    ""mergeStatus"": ""succeeded"",
    ""mergeId"": ""a10bb228-6ba6-4362-abd7-49ea21333dbd"",
    ""lastMergeSourceCommit"": {
      ""commitId"": ""53d54ac915144006c2c9e90d2c7d3880920db49c"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/53d54ac915144006c2c9e90d2c7d3880920db49c""
    },
    ""lastMergeTargetCommit"": {
      ""commitId"": ""a511f535b1ea495ee0c903badb68fbc83772c882"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/a511f535b1ea495ee0c903badb68fbc83772c882""
    },
    ""lastMergeCommit"": {
      ""commitId"": ""eef717f69257a6333f221566c1c987dc94cc0d72"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/eef717f69257a6333f221566c1c987dc94cc0d72""
    },
    ""reviewers"": [
      {
        ""reviewerUrl"": null,
        ""vote"": 0,
        ""id"": ""2ea2d095-48f9-4cd6-9966-62f6f574096c"",
        ""displayName"": ""[Mobile]\\Mobile Team"",
        ""uniqueName"": ""vstfs:///Classification/TeamProject/f0811a3b-8c8a-4e43-a3bf-9a049b4835bd\\Mobile Team"",
        ""url"": ""https://fabrikam.vssps.visualstudio.com/_apis/Identities/2ea2d095-48f9-4cd6-9966-62f6f574096c"",
        ""imageUrl"": ""https://fabrikam.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=2ea2d095-48f9-4cd6-9966-62f6f574096c"",
        ""isContainer"": true
      }
    ],
    ""commits"": [
      {
        ""commitId"": ""53d54ac915144006c2c9e90d2c7d3880920db49c"",
        ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/53d54ac915144006c2c9e90d2c7d3880920db49c""
      }
    ],
    ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/pullRequests/1""
  },
  ""resourceVersion"": ""1.0-preview.1"",
  ""resourceContainers"": {
    ""collection"": {
      ""id"": ""c12d0eb8-e382-443b-9f9c-c52cba5014c2""
    },
    ""account"": {
      ""id"": ""f844ec47-a9db-4511-8281-8b63f4eaf94e""
    },
    ""project"": {
      ""id"": ""be9b3917-87e6-42a4-a549-2bc06a7a878f""
    }
  },
  ""createdDate"": ""2016-10-31T22:14:11.0194327Z""
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
            Assert.AreEqual("TfsPullRequestCreated", data.SensorId);
            Assert.AreEqual(9, data.Parameters.Count);
            Assert.AreEqual("1", data.Parameters["PULLREQUESTID"]);
            Assert.AreEqual("active", data.Parameters["STATUS"]);
            Assert.AreEqual("Jamal Hartnett", data.Parameters["USER"]);
            Assert.AreEqual("my first pull request", data.Parameters["TITLE"]);
            Assert.AreEqual(" - test2\r\n", data.Parameters["DESCRIPTION"]);
            Assert.AreEqual("refs/heads/mytopic", data.Parameters["SOURCEBRANCH"]);
            Assert.AreEqual("refs/heads/master", data.Parameters["TARGETBRANCH"]);
            Assert.AreEqual("Fabrikam", data.Parameters["TFSPROJECT"]);
            Assert.AreEqual("Fabrikam", data.Parameters["GITREPOSITORY"]);
        }

        [Test]
        public void PostWithUpdatedPullRequest()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsPullRequestController(publisher.Object, diagnostics);

            var jsonText = @"
{
  ""subscriptionId"": ""00000000-0000-0000-0000-000000000000"",
  ""notificationId"": 51,
  ""id"": ""af07be1b-f3ad-44c8-a7f1-c4835f2df06b"",
  ""eventType"": ""git.pullrequest.updated"",
  ""publisherId"": ""tfs"",
  ""scope"": ""all"",
  ""message"": {
    ""text"": ""Jamal Hartnett marked the pull request as completed""
  },
  ""detailedMessage"": {
    ""text"": ""Jamal Hartnett marked the pull request as completed\r\n\r\n- Merge status: Succeeded\r\n- Merge commit: eef717(https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/eef717f69257a6333f221566c1c987dc94cc0d72)\r\n""
  },
  ""resource"": {
    ""repository"": {
      ""id"": ""4bc14d40-c903-45e2-872e-0462c7748079"",
      ""name"": ""Fabrikam"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079"",
      ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam"",
        ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
      },
      ""defaultBranch"": ""refs/heads/master"",
      ""remoteUrl"": ""https://fabrikam.visualstudio.com/DefaultCollection/_git/Fabrikam""
    },
    ""pullRequestId"": 1,
    ""status"": ""completed"",
    ""createdBy"": {
      ""id"": ""54d125f7-69f7-4191-904f-c5b96b6261c8"",
      ""displayName"": ""Jamal Hartnett"",
      ""uniqueName"": ""fabrikamfiber4@hotmail.com"",
      ""url"": ""https://fabrikam.vssps.visualstudio.com/_apis/Identities/54d125f7-69f7-4191-904f-c5b96b6261c8"",
      ""imageUrl"": ""https://fabrikam.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=54d125f7-69f7-4191-904f-c5b96b6261c8""
    },
    ""creationDate"": ""2014-06-17T16:55:46.589889Z"",
    ""closedDate"": ""2014-06-30T18:59:12.3660573Z"",
    ""title"": ""my first pull request"",
    ""description"": "" - test2\r\n"",
    ""sourceRefName"": ""refs/heads/mytopic"",
    ""targetRefName"": ""refs/heads/master"",
    ""mergeStatus"": ""succeeded"",
    ""mergeId"": ""a10bb228-6ba6-4362-abd7-49ea21333dbd"",
    ""lastMergeSourceCommit"": {
      ""commitId"": ""53d54ac915144006c2c9e90d2c7d3880920db49c"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/53d54ac915144006c2c9e90d2c7d3880920db49c""
    },
    ""lastMergeTargetCommit"": {
      ""commitId"": ""a511f535b1ea495ee0c903badb68fbc83772c882"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/a511f535b1ea495ee0c903badb68fbc83772c882""
    },
    ""lastMergeCommit"": {
      ""commitId"": ""eef717f69257a6333f221566c1c987dc94cc0d72"",
      ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/eef717f69257a6333f221566c1c987dc94cc0d72""
    },
    ""reviewers"": [
      {
        ""reviewerUrl"": null,
        ""vote"": 0,
        ""id"": ""2ea2d095-48f9-4cd6-9966-62f6f574096c"",
        ""displayName"": ""[Mobile]\\Mobile Team"",
        ""uniqueName"": ""vstfs:///Classification/TeamProject/f0811a3b-8c8a-4e43-a3bf-9a049b4835bd\\Mobile Team"",
        ""url"": ""https://fabrikam.vssps.visualstudio.com/_apis/Identities/2ea2d095-48f9-4cd6-9966-62f6f574096c"",
        ""imageUrl"": ""https://fabrikam.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=2ea2d095-48f9-4cd6-9966-62f6f574096c"",
        ""isContainer"": true
      }
    ],
    ""commits"": [
      {
        ""commitId"": ""53d54ac915144006c2c9e90d2c7d3880920db49c"",
        ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/53d54ac915144006c2c9e90d2c7d3880920db49c""
      }
    ],
    ""url"": ""https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/pullRequests/1""
  },
  ""resourceVersion"": ""1.0-preview.1"",
  ""resourceContainers"": {
    ""collection"": {
      ""id"": ""c12d0eb8-e382-443b-9f9c-c52cba5014c2""
    },
    ""account"": {
      ""id"": ""f844ec47-a9db-4511-8281-8b63f4eaf94e""
    },
    ""project"": {
      ""id"": ""be9b3917-87e6-42a4-a549-2bc06a7a878f""
    }
  },
  ""createdDate"": ""2016-10-31T22:25:34.4617672Z""
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
            Assert.AreEqual("TfsPullRequestUpdated", data.SensorId);
            Assert.AreEqual(9, data.Parameters.Count);
            Assert.AreEqual("1", data.Parameters["PULLREQUESTID"]);
            Assert.AreEqual("completed", data.Parameters["STATUS"]);
            Assert.AreEqual("Jamal Hartnett", data.Parameters["USER"]);
            Assert.AreEqual("my first pull request", data.Parameters["TITLE"]);
            Assert.AreEqual(" - test2\r\n", data.Parameters["DESCRIPTION"]);
            Assert.AreEqual("refs/heads/mytopic", data.Parameters["SOURCEBRANCH"]);
            Assert.AreEqual("refs/heads/master", data.Parameters["TARGETBRANCH"]);
            Assert.AreEqual("Fabrikam", data.Parameters["TFSPROJECT"]);
            Assert.AreEqual("Fabrikam", data.Parameters["GITREPOSITORY"]);
        }
    }
}
