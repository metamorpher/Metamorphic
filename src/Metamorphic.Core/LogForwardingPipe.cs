//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core
{
    /// <summary>
    /// Provides methods to forward log messages across an <c>AppDomain</c> boundary.
    /// </summary>
    public sealed class LogForwardingPipe : MarshalByRefObject, ILogMessagesFromRemoteAppDomains
    {
        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogForwardingPipe"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public LogForwardingPipe(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            _diagnostics = diagnostics;
        }

        /// <summary>
        /// Logs the given message with the given severity.
        /// </summary>
        /// <param name="severity">The importance of the log message.</param>
        /// <param name="message">The message.</param>
        public void Log(LevelToLog severity, string message)
        {
            _diagnostics.Log(
                severity,
                CoreConstants.LogPrefix,
                message);
        }
    }
}
