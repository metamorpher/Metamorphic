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
using Newtonsoft.Json.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Sensor.Http.Controllers
{
    /// <summary>
    /// The controller that handles the generation of signals from Jenkins POST requests.
    /// </summary>
    [VersionedApiRoute(template: "api/jenkins", allowedVersion: 1)]
    public class JenkinsController : ApiController
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
        /// Initializes a new instance of the <see cref="JenkinsController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalPublisher"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public JenkinsController(IPublishSignals signalPublisher, SystemDiagnostics diagnostics)
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
                  "name": "BUILD_NAME",
                  "url": "job/Release/job/BUILD_NAME/",
                  "build": {
                    "full_url": "http://myserver/job/Release/job/BUILD_NAME/BUILD_NUMBER/",
                    "number": BUILD_NUMBER,
                    "queue_id": 1693,
                    "phase": "FINALIZED",
                    "status": "SUCCESS",
                    "url": "job/Release/job/BUILD_NAME/BUILD_NUMBER/",
                    "scm": {},
                    "parameters": {
                      "PARAMETER_1": "PARAMETER_1_VALUE",
                      "PARAMETER_2": "PARAMETER_2_VALUE",
                      "PARAMETER_3": "PARAMETER_3_VALUE"
                    },
                    "log": "",
                    "artifacts": {}
                  }
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
                    Resources.Log_Messages_JenkinsController_PostMethodInvoked_WithOrigin,
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
                            Resources.Log_Messages_JenkinsController_PostMethodInputDataInvalid_WithOriginAndBody,
                            HttpContext.Current.Request.UserHostAddress,
                            body));
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = Resources.HttpResponseMessage_ExpectedJsonBody,
                };
            }

            var parameters = (Dictionary<string, object>)dynamicObject.build.parameters.ToObject<Dictionary<string, object>>();
            parameters.Add("jobname", dynamicObject.name.ToString());
            parameters.Add("jobstatus", dynamicObject.build.status.ToString());
            parameters.Add("joburl", dynamicObject.build.full_url.ToString());

            var signalType = "JenkinsJobComplete";
            var signal = new Signal(new SignalTypeId(signalType), parameters);

            _diagnostics.Log(
                LevelToLog.Warn,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_JenkinsController_PostMethodPublishingSignal_WithSignalInformation,
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
