//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// The controller that handles the generatoin of signals from HTTP web requests.
    /// </summary>
    [Authorize]
    [VersionedApiRoute(template: "api/signal", allowedVersion: 1)]
    public sealed class SignalController : ApiController
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
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_SignalController_GetMethodInvoked_WithOrigin,
                    HttpContext.Current.Request.UserHostAddress));

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };
        }

        /// <summary>
        /// Triggers a new signal.
        /// </summary>
        /// <param name="jsonData">The JSON data.</param>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        public HttpResponseMessage Post([FromBody]JToken jsonData)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_SignalController_PostMethodInvoked_WithOrigin,
                    HttpContext.Current.Request.UserHostAddress));

            if (jsonData == null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                };
            }

            var signalType = jsonData.Children()
                .Where(t => t is JProperty)
                .Cast<JProperty>()
                .Where(t => t.Name.Equals("Type"))
                .Select(t => string.Join(" ", t.Children()))
                .FirstOrDefault();

            var arguments = jsonData.Children()
                .Where(t => t is JProperty)
                .Cast<JProperty>()
                .Where(t => !t.Name.Equals("Type"))
                .ToDictionary(
                    t => t.Name,
                    t => (object)string.Join(" ", t.Children()));

            var signal = new Signal(
                new SignalTypeId(signalType),
                arguments);

            _publisher.Publish(signal);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
            };
        }
    }
}
