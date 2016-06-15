//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Rules;

namespace Metamorphic.Server.Rules
{
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