//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core;

namespace Metamorphic.Agent
{
    /// <summary>
    /// Defines the interface for objects that load <see cref="IExecuteActions"/> instances into a remote <see cref="AppDomain"/>.
    /// </summary>
    internal interface ILoadActionExecutorsInRemoteAppDomains
    {
        /// <summary>
        /// Loads the <see cref="IExecuteActions"/> object into the <c>AppDomain</c> in which the current
        /// object is currently loaded.
        /// </summary>
        /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
        /// <returns>The newly created <see cref="IExecuteActions"/> object.</returns>
        IExecuteActions Load(ILogMessagesFromRemoteAppDomains logger);
    }
}
