//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines the interface for objects that perform scanning of action packages.
    /// </summary>
    internal interface IScanActionPackages
    {
        /// <summary>
        /// Scans the packages for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="packagesToScan">
        /// The collection that contains the NuGet packages to be scanned.
        /// </param>
        void Scan(IEnumerable<PackageName> packagesToScan);
    }
}
