//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Actions;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines the interface for objects that store actions.
    /// </summary>
    internal interface IStoreActions
    {
        /// <summary>
        /// Returns the <see cref="ActionDefinition"/> which the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="id">The ID of the action</param>
        /// <returns>The action definition with the given ID.</returns>
        ActionDefinition Action(ActionId id);

        /// <summary>
        /// Adds a new <see cref="ActionDefinition"/> to the collection.
        /// </summary>
        /// <param name="definition">The action definition that should be added.</param>
        void Add(ActionDefinition definition);

        /// <summary>
        /// Returns a value indicating whether the storage has an <see cref="ActionDefinition"/>
        /// with the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>
        ///   <see langword="true" /> if the storage has a definition with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasActionFor(ActionId id);

        /// <summary>
        /// Returns a collection containing the descriptions of all the known packages.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known packages.
        /// </returns>
        IEnumerable<PackageName> KnownPackages();

        /// <summary>
        /// Removes all the packages related to the given package files.
        /// </summary>
        /// <param name="deletedPackages">The collection of packages that were removed.</param>
        void RemovePackages(IEnumerable<PackageName> deletedPackages);
    }
}
