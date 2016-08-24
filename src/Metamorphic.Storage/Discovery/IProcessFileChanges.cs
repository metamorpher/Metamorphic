//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Metamorphic.Storage.Discovery
{
    /// <summary>
    /// Defines the interface for objects that handle file detection.
    /// </summary>
    internal interface IProcessFileChanges
    {
        /// <summary>
        /// Processes the added files.
        /// </summary>
        /// <param name="newFiles">The collection that contains the names of all the new files.</param>
        void Added(IEnumerable<string> newFiles);

        /// <summary>
        /// Processes the removed files.
        /// </summary>
        /// <param name="removedFiles">The collection that contains the names of all the files that were removed.</param>
        void Removed(IEnumerable<string> removedFiles);
    }
}
