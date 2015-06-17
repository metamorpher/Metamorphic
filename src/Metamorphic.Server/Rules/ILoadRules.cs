﻿//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Server.Rules
{
    internal interface ILoadRules
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