﻿//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core.Signals;
using Microsoft.Owin.Hosting;

namespace Metamorphic.Server.Signals
{
    internal sealed class WebCallSignalGenerator : IGenerateSignals, IDisposable
    {
        private IDisposable m_Server;

        /// <summary>
        /// Gets the <see cref="SignalTypeId"/> for the signals generated by the current generator.
        /// </summary>
        public SignalTypeId Id
        {
            get;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Starts the signal generating process.
        /// </summary>
        public void Start()
        {
            var uri = "http://*:7070";
            m_Server = WebApp.Start<WebCallStartup>(uri);
        }

        /// <summary>
        /// Stops the signal generating process.
        /// </summary>
        public void Stop()
        {
            m_Server.Dispose();
            m_Server = null;
        }
    }
}
