//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

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
        private readonly IPublishSignals m_Publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalController"/> class.
        /// </summary>
        /// <param name="signalPublisher">The publisher that publishes all the signals.</param>
        internal SignalController(IPublishSignals signalPublisher)
        {
            {
                Lokad.Enforce.Argument(() => signalPublisher);
            }

            m_Publisher = signalPublisher;
        }

        /// <summary>
        /// Pings the controller to see if it is alive.
        /// </summary>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        [HttpGet]
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

            m_Publisher.Publish(signal);

            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Accepted,
            };
        }
    }
}
