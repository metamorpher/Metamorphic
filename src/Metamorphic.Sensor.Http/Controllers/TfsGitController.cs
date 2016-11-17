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
using System.Text.RegularExpressions;
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
    [VersionedApiRoute(template: "api/tfsgit", allowedVersion: 1)]
    public sealed class TfsGitController : ApiController
    {
        /// <summary>
        /// The regex that is used to extract the branch name from a git tag branch.
        /// </summary>
        private static readonly Regex GitBranchMatcher = new Regex(
            @"(?:refs)\/(?:heads)\/(.*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The regex that is used to extract the tag name from a git tag ref.
        /// </summary>
        private static readonly Regex GitTagMatcher = new Regex(
            @"(?:refs)\/(?:tags)\/(.*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Cannot be stuffed fixing that. Just wanting this to work. Will fix later when we split this out.")]
        private static Signal CreateSignalForCommit(string text)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(text);
            if (dynamicObject == null)
            {
                return null;
            }

            string refName = dynamicObject.resource.refUpdates[0].name;
            string previousCommit = dynamicObject.resource.refUpdates[0].oldObjectId;
            string currentCommit = dynamicObject.resource.refUpdates[0].newObjectId;

            string signalType = "Unknown";
            var parameters = new Dictionary<string, object>();
            parameters.Add("tfsproject", dynamicObject.resource.repository.project.name.ToString());
            parameters.Add("gitrepository", dynamicObject.resource.repository.name.ToString());
            if (refName.StartsWith("refs/heads", StringComparison.OrdinalIgnoreCase))
            {
                var branchName = ParseBranchNameFromRef(refName);
                if (string.IsNullOrEmpty(branchName))
                {
                    return null;
                }

                parameters.Add("gitbranch", branchName);
                if (previousCommit.Equals("0000000000000000000000000000000000000000"))
                {
                    parameters.Add("gitrevision", currentCommit);
                    signalType = "GitBranchCreate";
                }
                else
                {
                    if (currentCommit.Equals("0000000000000000000000000000000000000000"))
                    {
                        parameters.Add("gitrevision", previousCommit);
                        signalType = "GitBranchDelete";
                    }
                    else
                    {
                        parameters.Add("gitpreviousrevision", previousCommit);
                        parameters.Add("gitcurrentrevision", currentCommit);
                        signalType = "GitCommit";
                    }
                }
            }
            else
            {
                if (refName.StartsWith("refs/tags", StringComparison.OrdinalIgnoreCase))
                {
                    var tagName = ParseTagNameFromRef(refName);
                    if (string.IsNullOrEmpty(tagName))
                    {
                        return null;
                    }

                    parameters.Add("gittag", tagName);
                    if (previousCommit.Equals("0000000000000000000000000000000000000000"))
                    {
                        parameters.Add("gitrevision", currentCommit);
                        signalType = "GitTagCreate";
                    }
                    else
                    {
                        parameters.Add("gitrevision", previousCommit);
                        signalType = "GitTagDelete";
                    }
                }
            }

            return new Signal(new SignalTypeId(signalType), parameters);
        }

        private static string ParseBranchNameFromRef(string refName)
        {
            var match = GitBranchMatcher.Match(refName);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        private static string ParseTagNameFromRef(string refName)
        {
            var match = GitTagMatcher.Match(refName);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
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
        /// Initializes a new instance of the <see cref="TfsGitController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalPublisher"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TfsGitController(IPublishSignals signalPublisher, SystemDiagnostics diagnostics)
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
                  "subscriptionId": "d4e9c98f-5dd2-4b81-b494-b35dca5a4761",
                  "notificationId": 1,
                  "id": "03c164c2-8912-4d5e-8009-3707d5f83734",
                  "eventType": "git.push",
                  "publisherId": "tfs",
                  "message": {
                    "text": "Jamal Hartnett pushed updates to branch master of repository Fabrikam-Fiber-Git."
                  },
                  "detailedMessage": {
                    "text": "Jamal Hartnett pushed 1 commit to branch master of repository Fabrikam-Fiber-Git.\n - Fixed bug in web.config file 33b55f7c"
                  },
                  "resource": {
                    "commits": [
                      {
                        "commitId": "33b55f7cb7e7e245323987634f960cf4a6e6bc74",
                        "author": {
                          "name": "Jamal Hartnett",
                          "email": "fabrikamfiber4@hotmail.com",
                          "date": "2015-02-25T19:01:00Z"
                        },
                        "committer": {
                          "name": "Jamal Hartnett",
                          "email": "fabrikamfiber4@hotmail.com",
                          "date": "2015-02-25T19:01:00Z"
                        },
                        "comment": "Fixed bug in web.config file",
                        "url": "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git/commit/33b55f7cb7e7e245323987634f960cf4a6e6bc74"
                      }
                    ],
                    "refUpdates": [
                      {
                        "name": "refs/heads/master",
                        "oldObjectId": "aad331d8d3b131fa9ae03cf5e53965b51942618a",
                        "newObjectId": "33b55f7cb7e7e245323987634f960cf4a6e6bc74"
                      }
                    ],
                    "repository": {
                      "id": "278d5cd2-584d-4b63-824a-2ba458937249",
                      "name": "Fabrikam-Fiber-Git",
                      "url": "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249",
                      "project": {
                        "id": "6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c",
                        "name": "Fabrikam-Fiber-Git",
                        "url": "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/projects/6ce954b1-ce1f-45d1-b94d-e6bf2464ba2c",
                        "state": "wellFormed"
                      },
                      "defaultBranch": "refs/heads/master",
                      "remoteUrl": "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_git/Fabrikam-Fiber-Git"
                    },
                    "pushedBy": {
                      "id": "00067FFED5C7AF52@Live.com",
                      "displayName": "Jamal Hartnett",
                      "uniqueName": "Windows Live ID\\fabrikamfiber4@hotmail.com"
                    },
                    "pushId": 14,
                    "date": "2014-05-02T19:17:13.3309587Z",
                    "url": "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/git/repositories/278d5cd2-584d-4b63-824a-2ba458937249/pushes/14"
                  },
                  "createdDate": "2016-09-20T02:28:06.9969612Z"
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
                    Resources.Log_Messages_TfsGitController_PostMethodInvoked_WithOrigin,
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

            var signal = (Signal)CreateSignalForCommit(text);
            if (signal == null)
            {
                if (HttpContext.Current != null)
                {
                    var body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_TfsGitController_PostMethodInputDataInvalid_WithOriginAndBody,
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
