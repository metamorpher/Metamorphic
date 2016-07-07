//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core
{
    /// <summary>
    /// Defines the interface for objects that remote logging calls.
    /// </summary>
    public interface ILogMessagesFromRemoteAppDomains
    {
        /// <summary>
        /// Logs the given message with the given severity.
        /// </summary>
        /// <param name="severity">The importance of the log message.</param>
        /// <param name="message">The message.</param>
        void Log(LevelToLog severity, string message);
    }
}
