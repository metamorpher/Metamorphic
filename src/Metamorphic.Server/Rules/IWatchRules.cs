//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Server.Rules
{
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