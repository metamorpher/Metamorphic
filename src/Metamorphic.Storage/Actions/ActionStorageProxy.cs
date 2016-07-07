//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Actions;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    /// <summary>
    /// Defines a proxy for <see cref="IStoreActions"/> objects.
    /// </summary>
    /// <design>
    /// This class is meant to serve as a proxy for the real plugin repository in a remote <c>AppDomain</c>
    /// so that the <see cref="RemotePackageScanner"/> is able to refer to the plugin repository without
    /// needing duplicates of the repository.
    /// </design>
    internal sealed class ActionStorageProxy : MarshalByRefObject, IStoreActions
    {
        /// <summary>
        /// The object that stores all the part and part group information.
        /// </summary>
        private readonly IStoreActions _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionStorageProxy"/> class.
        /// </summary>
        /// <param name="repository">The object that stores all the parts and part groups.</param>
        public ActionStorageProxy(IStoreActions repository)
        {
            {
                Debug.Assert(repository != null, "The repository object should not be a null reference.");
            }

            _repository = repository;
        }

        /// <summary>
        /// Returns the <see cref="ActionDefinition"/> which the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="action">The ID of the action</param>
        /// <returns>The action definition with the given ID.</returns>
        public ActionDefinition Action(ActionId action)
        {
            return _repository.Action(action);
        }

        /// <summary>
        /// Adds a new <see cref="ActionDefinition"/> to the collection.
        /// </summary>
        /// <param name="definition">The action definition that should be added.</param>
        public void Add(ActionDefinition definition)
        {
            _repository.Add(definition);
        }

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
        public bool HasActionFor(ActionId action)
        {
            return _repository.HasActionFor(action);
        }

        /// <summary>
        /// Returns a collection containing the descriptions of all the known packages.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known packages.
        /// </returns>
        public IEnumerable<PackageName> KnownPackages()
        {
            return _repository.KnownPackages();
        }

        /// <summary>
        /// Removes all the packages related to the given package files.
        /// </summary>
        /// <param name="deletedPackages">The collection of packages that were removed.</param>
        public void RemovePackages(IEnumerable<PackageName> deletedPackages)
        {
            _repository.RemovePackages(deletedPackages);
        }
    }
}
