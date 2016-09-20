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
    public sealed class TfsGitControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsGitController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullPublisher()
        {
            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new TfsGitController(null, diagnostics));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.TfsGitController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            var publisher = new Mock<IPublishSignals>();
            Assert.Throws<ArgumentNullException>(() => new TfsGitController(publisher.Object, null));
        }

        [Test]
        public void PostWithDeletedBranch()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsGitController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""d4e9c98f-5dd2-4b81-b494-b35dca5a4761"",
    ""notificationId"": 1,
    ""id"": ""03c164c2-8912-4d5e-8009-3707d5f83734"",
    ""eventType"": ""git.push"",
    ""publisherId"": ""tfs"",
    ""message"": {
    ""text"": ""Jamal Hartnett pushed updates to branch master of repository Fabrikam-Fiber-Git.""
    },
    ""detailedMessage"": {
    ""text"": ""Jamal Hartnett pushed 1 commit to branch master of repository Fabrikam-Fiber-Git.\n - Fixed bug in web.config file 33b55f7c""
    },
    ""resource"": {
    ""commits"": [
        {
        ""commitId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""author"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""committer"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""comment"": ""Fixed bug in web.config file"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git/commit/33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""refUpdates"": [
        {
        ""name"": ""refs/heads/feature/mybranch_with_almost_spaces"",
        ""oldObjectId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""newObjectId"": ""0000000000000000000000000000000000000000""
        }
    ],
    ""repository"": {
        ""id"": ""278d5cd2-584d-4b63-824a-2ba458937249"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249"",
        ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
        },
        ""defaultBranch"": ""refs/heads/master"",
        ""remoteUrl"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git""
    },
    ""pushedBy"": {
        ""id"": ""00067FFED5C7AF52@Live.com"",
        ""displayName"": ""Jamal Hartnett"",
        ""uniqueName"": ""Windows Live ID\\fabrikamfiber4@hotmail.com""
    },
    ""pushId"": 14,
    ""date"": ""2014-05-02T19:17:13.3309587Z"",
    ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249/pushes/14""
    },
    ""createdDate"": ""2016-09-20T02:28:06.9969612Z""
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
            Assert.AreEqual("GitBranchDelete", data.SensorId);
            Assert.AreEqual(2, data.Parameters.Count);
            Assert.AreEqual("feature/mybranch_with_almost_spaces", data.Parameters["NAME"]);
            Assert.AreEqual("33b55f7cb7e7e245323987634f960cf4a6e6bc74", data.Parameters["REVISION"]);
        }

        [Test]
        public void PostWithDeletedTag()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsGitController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""d4e9c98f-5dd2-4b81-b494-b35dca5a4761"",
    ""notificationId"": 1,
    ""id"": ""03c164c2-8912-4d5e-8009-3707d5f83734"",
    ""eventType"": ""git.push"",
    ""publisherId"": ""tfs"",
    ""message"": {
    ""text"": ""Jamal Hartnett pushed updates to branch master of repository Fabrikam-Fiber-Git.""
    },
    ""detailedMessage"": {
    ""text"": ""Jamal Hartnett pushed 1 commit to branch master of repository Fabrikam-Fiber-Git.\n - Fixed bug in web.config file 33b55f7c""
    },
    ""resource"": {
    ""commits"": [
        {
        ""commitId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""author"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""committer"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""comment"": ""Fixed bug in web.config file"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git/commit/33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""refUpdates"": [
        {
        ""name"": ""refs/tags/1.2.3"",
        ""oldObjectId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""newObjectId"": ""0000000000000000000000000000000000000000""
        }
    ],
    ""repository"": {
        ""id"": ""278d5cd2-584d-4b63-824a-2ba458937249"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249"",
        ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
        },
        ""defaultBranch"": ""refs/heads/master"",
        ""remoteUrl"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git""
    },
    ""pushedBy"": {
        ""id"": ""00067FFED5C7AF52@Live.com"",
        ""displayName"": ""Jamal Hartnett"",
        ""uniqueName"": ""Windows Live ID\\fabrikamfiber4@hotmail.com""
    },
    ""pushId"": 14,
    ""date"": ""2014-05-02T19:17:13.3309587Z"",
    ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249/pushes/14""
    },
    ""createdDate"": ""2016-09-20T02:28:06.9969612Z""
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
            Assert.AreEqual("GitTagDelete", data.SensorId);
            Assert.AreEqual(2, data.Parameters.Count);
            Assert.AreEqual("1.2.3", data.Parameters["NAME"]);
            Assert.AreEqual("33b55f7cb7e7e245323987634f960cf4a6e6bc74", data.Parameters["REVISION"]);
        }

        [Test]
        public void PostWithNewBranch()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsGitController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""d4e9c98f-5dd2-4b81-b494-b35dca5a4761"",
    ""notificationId"": 1,
    ""id"": ""03c164c2-8912-4d5e-8009-3707d5f83734"",
    ""eventType"": ""git.push"",
    ""publisherId"": ""tfs"",
    ""message"": {
    ""text"": ""Jamal Hartnett pushed updates to branch master of repository Fabrikam-Fiber-Git.""
    },
    ""detailedMessage"": {
    ""text"": ""Jamal Hartnett pushed 1 commit to branch master of repository Fabrikam-Fiber-Git.\n - Fixed bug in web.config file 33b55f7c""
    },
    ""resource"": {
    ""commits"": [
        {
        ""commitId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""author"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""committer"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""comment"": ""Fixed bug in web.config file"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git/commit/33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""refUpdates"": [
        {
        ""name"": ""refs/heads/feature/mybranch_with_almost_spaces"",
        ""oldObjectId"": ""0000000000000000000000000000000000000000"",
        ""newObjectId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""repository"": {
        ""id"": ""278d5cd2-584d-4b63-824a-2ba458937249"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249"",
        ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
        },
        ""defaultBranch"": ""refs/heads/master"",
        ""remoteUrl"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git""
    },
    ""pushedBy"": {
        ""id"": ""00067FFED5C7AF52@Live.com"",
        ""displayName"": ""Jamal Hartnett"",
        ""uniqueName"": ""Windows Live ID\\fabrikamfiber4@hotmail.com""
    },
    ""pushId"": 14,
    ""date"": ""2014-05-02T19:17:13.3309587Z"",
    ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249/pushes/14""
    },
    ""createdDate"": ""2016-09-20T02:28:06.9969612Z""
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
            Assert.AreEqual("GitBranchCreate", data.SensorId);
            Assert.AreEqual(2, data.Parameters.Count);
            Assert.AreEqual("feature/mybranch_with_almost_spaces", data.Parameters["NAME"]);
            Assert.AreEqual("33b55f7cb7e7e245323987634f960cf4a6e6bc74", data.Parameters["REVISION"]);
        }

        [Test]
        public void PostWithNewCommit()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsGitController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""d4e9c98f-5dd2-4b81-b494-b35dca5a4761"",
    ""notificationId"": 1,
    ""id"": ""03c164c2-8912-4d5e-8009-3707d5f83734"",
    ""eventType"": ""git.push"",
    ""publisherId"": ""tfs"",
    ""message"": {
    ""text"": ""Jamal Hartnett pushed updates to branch master of repository Fabrikam-Fiber-Git.""
    },
    ""detailedMessage"": {
    ""text"": ""Jamal Hartnett pushed 1 commit to branch master of repository Fabrikam-Fiber-Git.\n - Fixed bug in web.config file 33b55f7c""
    },
    ""resource"": {
    ""commits"": [
        {
        ""commitId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""author"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""committer"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""comment"": ""Fixed bug in web.config file"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git/commit/33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""refUpdates"": [
        {
        ""name"": ""refs/heads/feature/mybranch_with_almost_spaces"",
        ""oldObjectId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""newObjectId"": ""aad331d8d3b131fa9ae03cf5e53965b51942618a""
        }
    ],
    ""repository"": {
        ""id"": ""278d5cd2-584d-4b63-824a-2ba458937249"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249"",
        ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
        },
        ""defaultBranch"": ""refs/heads/master"",
        ""remoteUrl"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git""
    },
    ""pushedBy"": {
        ""id"": ""00067FFED5C7AF52@Live.com"",
        ""displayName"": ""Jamal Hartnett"",
        ""uniqueName"": ""Windows Live ID\\fabrikamfiber4@hotmail.com""
    },
    ""pushId"": 14,
    ""date"": ""2014-05-02T19:17:13.3309587Z"",
    ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249/pushes/14""
    },
    ""createdDate"": ""2016-09-20T02:28:06.9969612Z""
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
            Assert.AreEqual("GitCommit", data.SensorId);
            Assert.AreEqual(3, data.Parameters.Count);
            Assert.AreEqual("feature/mybranch_with_almost_spaces", data.Parameters["NAME"]);
            Assert.AreEqual("33b55f7cb7e7e245323987634f960cf4a6e6bc74", data.Parameters["PREVIOUSREVISION"]);
            Assert.AreEqual("aad331d8d3b131fa9ae03cf5e53965b51942618a", data.Parameters["CURRENTREVISION"]);
        }

        [Test]
        public void PostWithNewTag()
        {
            Signal capturedSignal = null;
            var publisher = new Mock<IPublishSignals>();
            {
                publisher.Setup(p => p.Publish(It.IsAny<Signal>()))
                    .Callback<Signal>(s => capturedSignal = s)
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((l, m) => { }, null);
            var controller = new TfsGitController(publisher.Object, diagnostics);

            var jsonText = @"
{
    ""subscriptionId"": ""d4e9c98f-5dd2-4b81-b494-b35dca5a4761"",
    ""notificationId"": 1,
    ""id"": ""03c164c2-8912-4d5e-8009-3707d5f83734"",
    ""eventType"": ""git.push"",
    ""publisherId"": ""tfs"",
    ""message"": {
    ""text"": ""Jamal Hartnett pushed updates to branch master of repository Fabrikam-Fiber-Git.""
    },
    ""detailedMessage"": {
    ""text"": ""Jamal Hartnett pushed 1 commit to branch master of repository Fabrikam-Fiber-Git.\n - Fixed bug in web.config file 33b55f7c""
    },
    ""resource"": {
    ""commits"": [
        {
        ""commitId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74"",
        ""author"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""committer"": {
            ""name"": ""Jamal Hartnett"",
            ""email"": ""fabrikamfiber4@hotmail.com"",
            ""date"": ""2015-02-25T19:01:00Z""
        },
        ""comment"": ""Fixed bug in web.config file"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git/commit/33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""refUpdates"": [
        {
        ""name"": ""refs/tags/1.2.3"",
        ""oldObjectId"": ""0000000000000000000000000000000000000000"",
        ""newObjectId"": ""33b55f7cb7e7e245323987634f960cf4a6e6bc74""
        }
    ],
    ""repository"": {
        ""id"": ""278d5cd2-584d-4b63-824a-2ba458937249"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249"",
        ""project"": {
        ""id"": ""6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""name"": ""Fabrikam-Fiber-Git"",
        ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c"",
        ""state"": ""wellFormed""
        },
        ""defaultBranch"": ""refs/heads/master"",
        ""remoteUrl"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git""
    },
    ""pushedBy"": {
        ""id"": ""00067FFED5C7AF52@Live.com"",
        ""displayName"": ""Jamal Hartnett"",
        ""uniqueName"": ""Windows Live ID\\fabrikamfiber4@hotmail.com""
    },
    ""pushId"": 14,
    ""date"": ""2014-05-02T19:17:13.3309587Z"",
    ""url"": ""https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249/pushes/14""
    },
    ""createdDate"": ""2016-09-20T02:28:06.9969612Z""
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
            Assert.AreEqual("GitTagCreate", data.SensorId);
            Assert.AreEqual(2, data.Parameters.Count);
            Assert.AreEqual("1.2.3", data.Parameters["NAME"]);
            Assert.AreEqual("33b55f7cb7e7e245323987634f960cf4a6e6bc74", data.Parameters["REVISION"]);
        }
    }
}
