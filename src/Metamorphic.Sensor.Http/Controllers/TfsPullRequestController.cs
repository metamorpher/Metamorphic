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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Metamorphic.Sensor.Http.Properties;
using Newtonsoft.Json;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Sensor.Http.Controllers
{
    /// <summary>
    /// The controller that handles the generation of signals from TFS GIT POST requests.
    /// </summary>
    [VersionedApiRoute(template: "api/tfspullrequest", allowedVersion: 1)]
    public sealed class TfsPullRequestController : ApiController
    {
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Cannot be stuffed fixing that. Just wanting this to work. Will fix later when we split this out.")]
        private static Signal CreateSignalForPullRequest(string text)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(text);
            if (dynamicObject == null)
            {
                return null;
            }

            string signalType = "Unknown";
            var parameters = new Dictionary<string, object>();
            parameters.Add("tfsproject", dynamicObject.resource.repository.project.name.ToString());
            parameters.Add("gitrepository", dynamicObject.resource.repository.name.ToString());

            parameters.Add("sourcebranch", dynamicObject.resource.sourceRefName.ToString());
            parameters.Add("targetbranch", dynamicObject.resource.targetRefName.ToString());

            parameters.Add("pullrequestid", dynamicObject.resource.pullRequestId.ToString());

            parameters.Add("status", dynamicObject.resource.status.ToString());

            parameters.Add("user", dynamicObject.resource.createdBy.displayName.ToString());

            parameters.Add("title", dynamicObject.resource.title.ToString());
            parameters.Add("description", dynamicObject.resource.description.ToString());

            string eventType = dynamicObject.eventType;
            if (eventType.EndsWith("created", StringComparison.OrdinalIgnoreCase))
            {
                signalType = "TfsPullRequestCreated";
            }
            else
            {
                if (eventType.EndsWith("updated", StringComparison.OrdinalIgnoreCase))
                {
                    signalType = "TfsPullRequestUpdated";
                }
            }

                return new Signal(new SignalTypeId(signalType), parameters);
        }

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that is used to publish the generated signals.
        /// </summary>
        private readonly IPublishSignals _publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsPullRequestController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalPublisher"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TfsPullRequestController(IPublishSignals signalPublisher, SystemDiagnostics diagnostics)
        {
            if (signalPublisher == null)
            {
                throw new ArgumentNullException("signalPublisher");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _publisher = signalPublisher;
        }

        private string ClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Triggers a new signal.
        /// </summary>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Lot's of if statements to get out early. Should probably refactor but can't be stuffed for now.")]
        public HttpResponseMessage Post()
        {
            /*
                Expecting something like:

                {
                  "subscriptionId": "00000000-0000-0000-0000-000000000000",
                  "notificationId": 50,
                  "id": "2ab4e3d3-b7a6-425e-92b1-5a9982c1269e",
                  "eventType": "git.pullrequest.created",
                  "publisherId": "tfs",
                  "scope": "all",
                  "message": {
                    "text": "Jamal Hartnett created a new pull request"
                  },
                  "detailedMessage": {
                    "text": "Jamal Hartnett created a new pull request\r\n\r\n- Merge status: Succeeded\r\n- Merge commit: eef717(https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/eef717f69257a6333f221566c1c987dc94cc0d72)\r\n"
                  },
                  "resource": {
                    "repository": {
                      "id": "4bc14d40-c903-45e2-872e-0462c7748079",
                      "name": "Fabrikam",
                      "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079",
                      "project": {
                        "id": "6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c",
                        "name": "Fabrikam",
                        "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c",
                        "state": "wellFormed"
                      },
                      "defaultBranch": "refs/heads/master",
                      "remoteUrl": "https://fabrikam.visualstudio.com/DefaultCollection/_git/Fabrikam"
                    },
                    "pullRequestId": 1,
                    "status": "active",
                    "createdBy": {
                      "id": "54d125f7-69f7-4191-904f-c5b96b6261c8",
                      "displayName": "Jamal Hartnett",
                      "uniqueName": "fabrikamfiber4@hotmail.com",
                      "url": "https://fabrikam.vssps.visualstudio.com/_apis/Identities/54d125f7-69f7-4191-904f-c5b96b6261c8",
                      "imageUrl": "https://fabrikam.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=54d125f7-69f7-4191-904f-c5b96b6261c8"
                    },
                    "creationDate": "2014-06-17T16:55:46.589889Z",
                    "title": "my first pull request",
                    "description": " - test2\r\n",
                    "sourceRefName": "refs/heads/mytopic",
                    "targetRefName": "refs/heads/master",
                    "mergeStatus": "succeeded",
                    "mergeId": "a10bb228-6ba6-4362-abd7-49ea21333dbd",
                    "lastMergeSourceCommit": {
                      "commitId": "53d54ac915144006c2c9e90d2c7d3880920db49c",
                      "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/53d54ac915144006c2c9e90d2c7d3880920db49c"
                    },
                    "lastMergeTargetCommit": {
                      "commitId": "a511f535b1ea495ee0c903badb68fbc83772c882",
                      "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/a511f535b1ea495ee0c903badb68fbc83772c882"
                    },
                    "lastMergeCommit": {
                      "commitId": "eef717f69257a6333f221566c1c987dc94cc0d72",
                      "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/eef717f69257a6333f221566c1c987dc94cc0d72"
                    },
                    "reviewers": [
                      {
                        "reviewerUrl": null,
                        "vote": 0,
                        "id": "2ea2d095-48f9-4cd6-9966-62f6f574096c",
                        "displayName": "[Mobile]\\Mobile Team",
                        "uniqueName": "vstfs:///Classification/TeamProject/f0811a3b-8c8a-4e43-a3bf-9a049b4835bd\\Mobile Team",
                        "url": "https://fabrikam.vssps.visualstudio.com/_apis/Identities/2ea2d095-48f9-4cd6-9966-62f6f574096c",
                        "imageUrl": "https://fabrikam.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=2ea2d095-48f9-4cd6-9966-62f6f574096c",
                        "isContainer": true
                      }
                    ],
                    "commits": [
                      {
                        "commitId": "53d54ac915144006c2c9e90d2c7d3880920db49c",
                        "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/commits/53d54ac915144006c2c9e90d2c7d3880920db49c"
                      }
                    ],
                    "url": "https://fabrikam.visualstudio.com/DefaultCollection/_apis/git/repositories/4bc14d40-c903-45e2-872e-0462c7748079/pullRequests/1"
                  },
                  "resourceVersion": "1.0-preview.1",
                  "resourceContainers": {
                    "collection": {
                      "id": "c12d0eb8-e382-443b-9f9c-c52cba5014c2"
                    },
                    "account": {
                      "id": "f844ec47-a9db-4511-8281-8b63f4eaf94e"
                    },
                    "project": {
                      "id": "be9b3917-87e6-42a4-a549-2bc06a7a878f"
                    }
                  },
                  "createdDate": "2016-10-31T22:14:11.0194327Z"
                }


            */

            if ((ControllerContext == null) || (ControllerContext.Request == null) || (ControllerContext.Request.Content == null))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = Resources.HttpResponseMessage_NoInputStreamDefined,
                };
            }

            var clientIp = ClientIp(ControllerContext.Request);
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TfsPullRequestController_PostMethodInvoked_WithOrigin,
                    clientIp));

            var text = ControllerContext.Request.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrWhiteSpace(text))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = Resources.HttpResponseMessage_ExpectedJsonBody,
                };
            }

            var signal = (Signal)CreateSignalForPullRequest(text);
            if (signal == null)
            {
                if (HttpContext.Current != null)
                {
                    var body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_TfsPullRequestController_PostMethodInputDataInvalid_WithOriginAndBody,
                            HttpContext.Current.Request.UserHostAddress,
                            body));
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = Resources.HttpResponseMessage_ExpectedJsonBody,
                };
            }

            _diagnostics.Log(
                LevelToLog.Warn,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TfsGitController_PostMethodPublishingSignal_WithSignalInformation,
                    signal.Sensor.ToString(),
                    string.Join(",", signal.Parameters().Select(x => x + "=" + signal.ParameterValue(x).ToString()))));
            _publisher.Publish(signal);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
            };
        }
    }
}
