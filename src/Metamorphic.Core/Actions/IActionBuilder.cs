//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Defines the interface for elements that build <see cref="ActionDefinition"/> instances.
    /// </summary>
    public interface IActionBuilder
    {
        /// <summary>
        /// Returns a new <see cref="ActionDefinition"/>.
        /// </summary>
        /// <returns>A new action definition.</returns>
        ActionDefinition ToDefinition();
    }
}
