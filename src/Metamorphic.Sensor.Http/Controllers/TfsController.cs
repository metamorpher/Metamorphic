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
    /// The controller that handles the generation of signals from TFS POST requests.
    /// </summary>
    [VersionedApiRoute(template: "api/tfs", allowedVersion: 1)]
    public class TfsController : ApiController
    {
        private static string ParseBuildDefinitionNameFromBuildNumber(string buildNumber)
        {
            return buildNumber.Substring(0, buildNumber.LastIndexOf("_", StringComparison.Ordinal));
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
        /// Initializes a new instance of the <see cref="TfsController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalPublisher"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TfsController(IPublishSignals signalPublisher, SystemDiagnostics diagnostics)
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
                  "subscriptionId": "a24542e8-393a-4acb-b9d2-9417d72dd639",
                  "notificationId": 1,
                  "id": "f045fd98-3c2f-4e27-98be-285da87c25b4",
                  "eventType": "build.complete",
                  "publisherId": "tfs",
                  "message": {
                    "text": "Build MyCoolBuild_1.2.3_20191225.7 has been canceled"
                  },
                  "detailedMessage": {
                    "text": "Build MyCoolBuild_1.2.3_20191225.7 has been canceled"
                  },
                  "resource": {
                    "uri": "vstfs:///Build/Build/393935",
                    "id": 393935,
                    "buildNumber": "MyCoolBuild_1.2.3_20191225.7",
                    "url": "http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Builds/393935",
                    "startTime": "2016-08-11T03:04:09.87Z",
                    "finishTime": "2016-08-15T22:31:42.817Z",
                    "reason": "manual",
                    "status": "stopped",
                    "dropLocation": "\\\\tfsbuilds\\TFSBuilds\\MyCoolBuild_1.2.3\\MyCoolBuild_1.2.3_20191225.7",
                    "drop": {
                      "location": "\\\\tfsbuilds\\TFSBuilds\\MyCoolBuild_1.2.3\\MyCoolBuild_1.2.3_20191225.7",
                      "type": "localPath",
                      "url": "file://///tfsbuilds/TFSBuilds/MyCoolBuild_1.2.3/MyCoolBuild_1.2.3_20191225.7",
                      "downloadUrl": "file://///tfsbuilds/TFSBuilds/MyCoolBuild_1.2.3/MyCoolBuild_1.2.3_20191225.7"
                    },
                    "log": {
                      "type": "localPath",
                      "url": "file://///tfsbuilds/TFSBuilds/MyCoolBuild_1.2.3/MyCoolBuild_1.2.3_20191225.7/logs",
                      "downloadUrl": "file://///tfsbuilds/TFSBuilds/MyCoolBuild_1.2.3/MyCoolBuild_1.2.3_20191225.7/logs"
                    },
                    "sourceGetVersion": "C185823",
                    "lastChangedBy": {
                      "id": "ecff947f-38e4-498d-8740-6bfbfc995f0d",
                      "displayName": "John Doe",
                      "uniqueName": "MYDOMAIN\\John.Doe",
                      "url": "http://zzz:8080/tfs/MyProject/_apis/Identities/ecff947f-38e4-498d-8740-6bfbfc995f0d",
                      "imageUrl": "http://zzz:8080/tfs/MyProject/_api/_common/identityImage?id=ecff947f-38e4-498d-8740-6bfbfc995f0d"
                    },
                    "retainIndefinitely": false,
                    "hasDiagnostics": true,
                    "definition": {
                      "definitionType": "xaml",
                      "id": 2060,
                      "name": "FranceCountryPack_4.5.8",
                      "url": "http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Definitions/2060"
                    },
                    "queue": {
                      "queueType": "buildController",
                      "id": 12,
                      "name": "Default Controller - mymachine",
                      "url": "http://zzz:8080/tfs/MyProject/_apis/build/Queues/12"
                    },
                    "requests": [
                      {
                        "id": 480801,
                        "url": "http://zzz:8080/tfs/MyProject/c1114d4d-f88a-4702-a3c0-4e06b8b0a5d4/_apis/build/Requests/480801",
                        "requestedFor": {
                          "id": "dd44aced-2435-4fa8-a9f5-c0ca9d812fe8",
                          "displayName": "TFS Build Service",
                          "uniqueName": "MYDOMAIN\\BUILDAGENT"
                        }
                      }
                    ]
                  },
                  "resourceVersion": "1.0",
                  "createdDate": "2016-08-15T22:31:46.8089765Z"
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
                    Resources.Log_Messages_TfsController_PostMethodInvoked_WithOrigin,
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

            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(text);
            if (dynamicObject == null)
            {
                if (HttpContext.Current != null)
                {
                    var body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_TfsController_PostMethodInputDataInvalid_WithOriginAndBody,
                            HttpContext.Current.Request.UserHostAddress,
                            body));
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = Resources.HttpResponseMessage_ExpectedJsonBody,
                };
            }

            var parameters = new Dictionary<string, object>();
            parameters.Add("jobid", dynamicObject.resource.id.ToString());
            parameters.Add("jobname", ParseBuildDefinitionNameFromBuildNumber(dynamicObject.resource.buildNumber.ToString()));
            parameters.Add("jobstatus", dynamicObject.resource.status.ToString());
            parameters.Add("joburl", dynamicObject.resource.url.ToString());

            var signalType = "TfsJobComplete";
            var signal = new Signal(new SignalTypeId(signalType), parameters);

            _diagnostics.Log(
                LevelToLog.Warn,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TfsController_PostMethodPublishingSignal_WithSignalInformation,
                    signalType,
                    string.Join(",", parameters.Select(x => x.Key + "=" + x.Value.ToString()))));
            _publisher.Publish(signal);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
            };
        }
    }
}
