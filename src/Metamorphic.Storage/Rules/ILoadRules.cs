//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using Metamorphic.Core.Rules;

namespace Metamorphic.Storage.Rules
{
    /// <summary>
    /// Defines the interface for objects that load rule files.
    /// </summary>
    internal interface ILoadRules
    {
        /// <summary>
        /// Creates a new <see cref="RuleDefinition"/> object from the information in the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        /// <returns>A newly created rule definition.</returns>
        RuleDefinition LoadFromFile(string filePath);

        /// <summary>
        /// Creates a new <see cref="RuleDefinition"/> object from the information in the specified string.
        /// </summary>
        /// <param name="ruleDefinition">The full rule definition.</param>
        /// <returns>A newly created rule definition.</returns>
        RuleDefinition LoadFromMemory(string ruleDefinition);

        /// <summary>
        /// Creates a new <see cref="RuleDefinition"/> object from the information in the specified string.
        /// </summary>
        /// <param name="stream">The stream that contains the full rule definition.</param>
        /// <returns>A newly created rule definition.</returns>
        RuleDefinition LoadFromStream(Stream stream);
    }
}
