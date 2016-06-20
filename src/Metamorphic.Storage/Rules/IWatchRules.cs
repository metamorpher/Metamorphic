//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Storage.Rules
{
    /// <summary>
    /// Defines the interface for objects that watch rule files.
    /// </summary>
    internal interface IWatchRules
    {
        /// <summary>
        /// Disables the loading of packages.
        /// </summary>
        void Disable();

        /// <summary>
        /// Enables the loading of packages.
        /// </summary>
        void Enable();
    }
}
