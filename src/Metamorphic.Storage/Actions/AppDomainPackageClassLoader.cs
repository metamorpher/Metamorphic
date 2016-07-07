//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines methods to load an <see cref="IScanActionPackages"/> object into a remote <c>AppDomain</c>.
    /// </summary>
    internal sealed class AppDomainPackageClassLoader : MarshalByRefObject, ILoadPackageScannersInRemoteAppDomains
    {
        /// <summary>
        /// Loads the <see cref="IScanActionPackages"/> object into the <c>AppDomain</c> in which the current
        /// object is currently loaded.
        /// </summary>
        /// <param name="repository">The object that contains all the part and part group information.</param>
        /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
        /// <returns>The newly created <see cref="IScanActionPackages"/> object.</returns>
        public IScanActionPackageFiles Load(IStoreActions repository, ILogMessagesFromRemoteAppDomains logger)
        {
            try
            {
                return new RemotePackageScanner(repository, logger);
            }
            catch (Exception e)
            {
                logger.Log(LevelToLog.Error, e.ToString());
                throw;
            }
        }
    }
}
