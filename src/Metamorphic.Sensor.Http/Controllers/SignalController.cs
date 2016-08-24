//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
using Newtonsoft.Json.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Sensor.Http.Controllers
{
    /// <summary>
    /// The controller that handles the generation of signals from HTTP web requests.
    /// </summary>
    [VersionedApiRoute(template: "api/signal", allowedVersion: 1)]
    public sealed class SignalController : ApiController
    {
        private static object ConvertJsonValueToObject(JProperty t)
        {
            switch (t.Value.Type)
            {
                case JTokenType.None:
                    return null;
                case JTokenType.Object:
                    return null;
                case JTokenType.Array:
                    return t.Value.Children()
                        .Where(c => c is JProperty)
                        .Cast<JProperty>()
                        .Select(c => ConvertJsonValueToObject(c)).ToArray();
                case JTokenType.Constructor:
                    return null;
                case JTokenType.Property:
                    return ConvertJsonValueToObject((JProperty)t.Value);
                case JTokenType.Comment:
                    return null;
                case JTokenType.Integer:
                    return t.Value.Value<int>();
                case JTokenType.Float:
                    return t.Value.Value<double>();
                case JTokenType.String:
                    return t.Value.Value<string>();
                case JTokenType.Boolean:
                    return t.Value.Value<bool>();
                case JTokenType.Null:
                    return null;
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Date:
                    return t.Value.Value<DateTime>();
                case JTokenType.Raw:
                    return t.Value.Value<string>();
                case JTokenType.Bytes:
                    return null;
                case JTokenType.Guid:
                    return t.Value.Value<Guid>();
                case JTokenType.Uri:
                    return t.Value.Value<Uri>();
                case JTokenType.TimeSpan:
                    return t.Value.Value<TimeSpan>();
                default:
                    return null;
            }
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
        /// Initializes a new instance of the <see cref="SignalController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalPublisher"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public SignalController(IPublishSignals signalPublisher, SystemDiagnostics diagnostics)
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
        /// Returns the status of the controller.
        /// </summary>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Controller methods cannot be static.")]
        public HttpResponseMessage Get()
        {
            if ((ControllerContext == null) || (ControllerContext.Request == null))
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
                    Resources.Log_Messages_SignalController_GetMethodInvoked_WithOrigin,
                    clientIp));

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };
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
                    Resources.Log_Messages_SignalController_GetMethodInvoked_WithOrigin,
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

            var jsonData = JObject.Parse(text);
            if (jsonData == null)
            {
                if (HttpContext.Current != null)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_SignalController_PostMethodInputDataInvalid_WithOriginAndBody,
                            clientIp,
                            text));
                }

                return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = Resources.HttpResponseMessage_ExpectedJsonBody,
                    };
            }

            var signalType = jsonData.Children()
                .Where(t => t is JProperty)
                .Cast<JProperty>()
                .Where(t => t.Name.Equals("Type"))
                .Select(t => t.ToObject<string>())
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(signalType))
            {
                return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = Resources.HttpResponseMessage_MissingSignalType,
                    };
            }

            var arguments = jsonData.Children()
                .Where(t => t is JProperty)
                .Cast<JProperty>()
                .Where(t => !t.Name.Equals("Type"))
                .Select(t => new { Key = t.Name, Value = ConvertJsonValueToObject(t) })
                .Where(m => m.Value != null)
                .ToDictionary(
                    m => m.Key,
                    m => m.Value);

            var signal = new Signal(
                new SignalTypeId(signalType),
                arguments);

            _diagnostics.Log(
                LevelToLog.Warn,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_SignalController_PostMethodPublishingSignal_WithSignalInformation,
                    signalType,
                    string.Join(",", arguments.Select(x => x.Key + "=" + x.Value.ToString()))));
            _publisher.Publish(signal);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
            };
        }
    }
}
