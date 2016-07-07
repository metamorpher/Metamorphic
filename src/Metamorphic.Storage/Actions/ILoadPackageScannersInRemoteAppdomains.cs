//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines the interface for objects that load <see cref="IScanActionPackageFiles"/> instances into a remote <see cref="AppDomain"/>.
    /// </summary>
    internal interface ILoadPackageScannersInRemoteAppDomains
    {
        /// <summary>
        /// Loads the <see cref="IScanActionPackages"/> object into the <c>AppDomain</c> in which the current
        /// object is currently loaded.
        /// </summary>
        /// <param name="repository">The object that contains all the part and part group information.</param>
        /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
        /// <returns>The newly created <see cref="IScanActionPackages"/> object.</returns>
        IScanActionPackageFiles Load(IStoreActions repository, ILogMessagesFromRemoteAppDomains logger);
    }
}
