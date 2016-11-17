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
    /// The controller that handles the generation of signals from TFS work item POST requests.
    /// </summary>
    [VersionedApiRoute(template: "api/tfsworkitem", allowedVersion: 1)]
    public class TfsWorkItemController : ApiController
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that is used to publish the generated signals.
        /// </summary>
        private readonly IPublishSignals _publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsWorkItemController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalPublisher"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TfsWorkItemController(IPublishSignals signalPublisher, SystemDiagnostics diagnostics)
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
                  "subscriptionId": "61fa58ab-dced-4b53-a9dd-a987a3517216",
                  "notificationId": 8266,
                  "id": "27646e0e-b520-4d2b-9411-bba7524947cd",
                  "eventType": "workitem.updated",
                  "publisherId": "tfs",
                  "message": {
                    "text": "Bug #5 (Some great new idea!) updated by Jamal Hartnett.\r\n(http://fabrikam-fiber-inc.visualstudio.com/web/wi.aspx?pcguid=74e918bf-3376-436d-bd20-8e8c1287f465&id=5)"
                  },
                  "detailedMessage": {
                    "text": "Bug #5 (Some great new idea!) updated by Jamal Hartnett.\r\n(http://fabrikam-fiber-inc.visualstudio.com/web/wi.aspx?pcguid=74e918bf-3376-436d-bd20-8e8c1287f465&id=5)\r\n\r\n- New State: Approved\r\n"
                  },
                  "resource": {
                    "id": 2,
                    "workItemId": 0,
                    "rev": 2,
                    "revisedBy": null,
                    "revisedDate": "0001-01-01T00:00:00",
                    "fields": {
                      "System.Rev": {
                        "oldValue": "1",
                        "newValue": "2"
                      },
                      "System.AuthorizedDate": {
                        "oldValue": "2014-07-15T16:48:44.663Z",
                        "newValue": "2014-07-15T17:42:44.663Z"
                      },
                      "System.RevisedDate": {
                        "oldValue": "2014-07-15T17:42:44.663Z",
                        "newValue": "9999-01-01T00:00:00Z"
                      },
                      "System.State": {
                        "oldValue": "New",
                        "newValue": "Approved"
                      },
                      "System.Reason": {
                        "oldValue": "New defect reported",
                        "newValue": "Approved by the Product Owner"
                      },
                      "System.AssignedTo": {
                        "newValue": "Jamal Hartnet"
                      },
                      "System.ChangedDate": {
                        "oldValue": "2014-07-15T16:48:44.663Z",
                        "newValue": "2014-07-15T17:42:44.663Z"
                      },
                      "System.Watermark": {
                        "oldValue": "2",
                        "newValue": "5"
                      },
                      "Microsoft.VSTS.Common.Severity": {
                        "oldValue": "3 - Medium",
                        "newValue": "2 - High"
                      }
                    },
                    "_links": {
                      "self": {
                        "href": "http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/updates/2"
                      },
                      "parent": {
                        "href": "http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5"
                      },
                      "workItemUpdates": {
                        "href": "http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/updates"
                      }
                    },
                    "url": "http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/updates/2",
                    "revision": {
                      "id": 5,
                      "rev": 2,
                      "fields": {
                        "System.AreaPath": "FabrikamCloud",
                        "System.TeamProject": "FabrikamCloud",
                        "System.IterationPath": "FabrikamCloud\\Release 1\\Sprint 1",
                        "System.WorkItemType": "Bug",
                        "System.State": "New",
                        "System.Reason": "New defect reported",
                        "System.CreatedDate": "2014-07-15T16:48:44.663Z",
                        "System.CreatedBy": "Jamal Hartnett",
                        "System.ChangedDate": "2014-07-15T16:48:44.663Z",
                        "System.ChangedBy": "Jamal Hartnett",
                        "System.Title": "Some great new idea!",
                        "Microsoft.VSTS.Common.Severity": "3 - Medium",
                        "WEF_EB329F44FE5F4A94ACB1DA153FDF38BA_Kanban.Column": "New"
                      },
                      "url": "http://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/5/revisions/2"
                    }
                  },
                  "createdDate": "2016-09-20T04:50:18.6739333Z"
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
                    Resources.Log_Messages_TfsWorkItemController_PostMethodInvoked_WithOrigin,
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
                            Resources.Log_Messages_TfsWorkItemController_PostMethodInputDataInvalid_WithOriginAndBody,
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
            parameters.Add("id", dynamicObject.resource.revision.id.ToString());
            parameters.Add("state", dynamicObject.resource.revision.fields["System.State"].ToString());
            parameters.Add("title", dynamicObject.resource.revision.fields["System.Title"].ToString());
            parameters.Add("type", dynamicObject.resource.revision.fields["System.WorkItemType"].ToString());
            parameters.Add("areapath", dynamicObject.resource.revision.fields["System.AreaPath"].ToString());
            parameters.Add("teamproject", dynamicObject.resource.revision.fields["System.TeamProject"].ToString());

            var assignedTo = dynamicObject.resource.revision.fields["System.AssignedTo"];
            parameters.Add("assignedto", assignedTo != null ? assignedTo.ToString() : string.Empty);

            var signalType = "TfsWorkItemChange";
            var signal = new Signal(new SignalTypeId(signalType), parameters);

            _diagnostics.Log(
                LevelToLog.Warn,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TfsWorkItemController_PostMethodPublishingSignal_WithSignalInformation,
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
