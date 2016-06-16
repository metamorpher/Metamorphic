//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Actions;

namespace Metamorphic.Server.Actions
{
    /// <summary>
    /// Defines the interface for objects that store actions.
    /// </summary>
    internal interface IStoreActions
    {
        /// <summary>
        /// Returns the <see cref="ActionDefinition"/> which the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="action">The ID of the action</param>
        /// <returns>The action definition with the given ID.</returns>
        ActionDefinition Action(ActionId action);

        /// <summary>
        /// Adds a new <see cref="ActionDefinition"/> to the collection.
        /// </summary>
        /// <param name="definition">The action definition that should be added.</param>
        void Add(ActionDefinition definition);

        /// <summary>
        /// Returns a value indicating whether the storage has an <see cref="ActionDefinition"/>
        /// with the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="action">The ID of the action.</param>
        /// <returns>
        ///   <see langword="true" /> if the storage has a definition with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasActionFor(ActionId action);
    }
}
