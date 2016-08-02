//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NuGet;

namespace Metamorphic.Storage.Discovery
{
    /// <summary>
    /// Defines the interface for objects that handle package detection.
    /// </summary>
    internal interface IProcessPackageChanges
    {
        /// <summary>
        /// Processes the added packages.
        /// </summary>
        /// <param name="newPackages">The collection that contains the names of all the new packages.</param>
        void Added(IEnumerable<PackageName> newPackages);

        /// <summary>
        /// Processes the removed packages.
        /// </summary>
        /// <param name="removedPackages">The collection that contains the names of all the packages that were removed.</param>
        void Removed(IEnumerable<PackageName> removedPackages);
    }
}
