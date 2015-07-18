//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server.Signals
{
    /// <summary>
    /// The controller that handles signals.
    /// </summary>
    public sealed class SignalController : ApiController
    {
        private readonly IQueueSignals m_Queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalController"/> class.
        /// </summary>
        /// <param name="signalQueue">The queue that holds all the signals.</param>
        internal SignalController(IQueueSignals signalQueue)
        {
            {
                Lokad.Enforce.Argument(() => signalQueue);
            }

            m_Queue = signalQueue;
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
        /// <param name="signalType">The id of the signal.</param>
        /// <param name="arguments">The arguments for the signal.</param>
        /// <returns>A http response message indicating whether the call was successful.</returns>
        [HttpPost]
        public HttpResponseMessage Trigger(string signalType, string arguments)
        {
            var signal = new Signal(
                new SignalTypeId(signalType),
                new Dictionary<string, object>
                {
                    ["arguments"] = arguments,
                });

            m_Queue.Enqueue(signal);

            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Accepted,
            };
        }
    }
}
