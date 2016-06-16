//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Rules;

namespace Metamorphic.Server.Rules
{
    /// <summary>
    /// Defines the interface for objects that load rule files.
    /// </summary>
    internal interface ILoadRules
    {
        /// <summary>
        /// Creates a new <see cref="Rule"/> object from the information in the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        /// <returns>A newly created rule instance.</returns>
        Rule Load(string filePath);
    }
}
