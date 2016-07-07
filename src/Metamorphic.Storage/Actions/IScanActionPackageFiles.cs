//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines the interface for objects that perform scanning of action packages.
    /// </summary>
    internal interface IScanActionPackageFiles
    {
        /// <summary>
        /// Scans the packages for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="packageName">The name of the NuGet package from which the files were retrieved.</param>
        /// <param name="packageVersion">The version of the NuGet package from which the files were retrieved.</param>
        /// <param name="filesToScan">
        /// The collection that contains the file paths to all the packages to be scanned.
        /// </param>
        void Scan(string packageName, string packageVersion, IEnumerable<string> filesToScan);
    }
}
