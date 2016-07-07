//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines the interface for objects that watch one or more directories for new NuGet packages containing actions.
    /// </summary>
    internal interface IWatchPackages
    {
        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        void Disable();

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        void Enable();
    }
}
