//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Newtonsoft.Json.Linq;

namespace Metamorphic.Server.Signals
{
    /// <summary>
    /// The controller that handles signals.
    /// </summary>
    public sealed class SignalController : ApiController
    {
        private readonly IPublishSignals _publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        internal SignalController(IPublishSignals signalPublisher)
        {
            {
                Lokad.Enforce.Argument(() => signalPublisher);
            }

            _publisher = signalPublisher;
        }

        /// <summary>
        /// Pings the controller to see if it is alive.
        /// </summary>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        [HttpGet]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Controller methods cannot be static.")]
        public HttpResponseMessage Ping()
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        /// <summary>
        /// Triggers a new signal.
        /// </summary>
        /// <param name="jsonData">The JSON data.</param>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        [HttpPost]
        public HttpResponseMessage Trigger([FromBody]JToken jsonData)
        {
            if (jsonData == null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
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
                StatusCode = System.Net.HttpStatusCode.Accepted,
            };
        }
    }
}
