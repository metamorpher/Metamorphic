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
    internal sealed class SignalController : ApiController
    {
        private readonly IQueueSignals m_Queue;

        public SignalController(IQueueSignals signalQueue)
        {
            {
                Lokad.Enforce.Argument(() => signalQueue);
            }

            m_Queue = signalQueue;
        }

        [HttpPost]
        public HttpResponseMessage Powershell(string scriptPath)
        {
            var signal = new Signal(
                new SignalTypeId("powershell"),
                new Dictionary<string, object>
                {
                    ["scriptPath"] = scriptPath,
                });

            m_Queue.Enqueue(signal);

            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Accepted,
            };
        }
    }
}
