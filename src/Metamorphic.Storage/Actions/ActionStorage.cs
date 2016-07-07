//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Metamorphic.Core.Actions;
using Metamorphic.Storage.Properties;
using NuGet;

namespace Metamorphic.Storage.Actions
{
    internal sealed class ActionStorage : IStoreActions
    {
        /// <summary>
        /// The collection of known actions.
        /// </summary>
        private readonly Dictionary<ActionId, ActionDefinition> _actions
            = new Dictionary<ActionId, ActionDefinition>();

        /// <summary>
        /// The collection of known packages.
        /// </summary>
        private readonly List<PackageName> _knownPackages
            = new List<PackageName>();

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Returns the <see cref="ActionDefinition"/> which the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="id">The ID of the action</param>
        /// <returns>The action definition with the given ID.</returns>
        public ActionDefinition Action(ActionId id)
        {
            if (!HasActionFor(id))
            {
                return null;
            }

            lock (_lock)
            {
                return _actions[id];
            }
        }

        /// <summary>
        /// Adds a new <see cref="ActionDefinition"/> to the collection.
        /// </summary>
        /// <param name="definition">The action definition that should be added.</param>
        public void Add(ActionDefinition definition)
        {
            {
                Lokad.Enforce.Argument(() => definition);
                Lokad.Enforce.With<DuplicateActionDefinitionException>(
                    !HasActionFor(definition.Id),
                    Resources.Exceptions_Messages_DuplicateActionDefinition);
            }

            lock (_lock)
            {
                _actions.Add(definition.Id, definition);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the storage has an <see cref="ActionDefinition"/>
        /// with the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasActionFor(ActionId id)
        {
            if (id == null)
            {
                return false;
            }

            lock (_lock)
            {
                return _actions.ContainsKey(id);
            }
        }

        /// <summary>
        /// Returns a collection containing the descriptions of all the known packages.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known packages.
        /// </returns>
        public IEnumerable<PackageName> KnownPackages()
        {
            lock (_lock)
            {
                return _knownPackages.AsReadOnly();
            }
        }

        /// <summary>
        /// Removes all the packages related to the given package files.
        /// </summary>
        /// <param name="deletedPackages">The collection of packages that were removed.</param>
        public void RemovePackages(IEnumerable<PackageName> deletedPackages)
        {
            if (deletedPackages == null)
            {
                return;
            }

            lock (_lock)
            {
                var packagesToDelete = _knownPackages
                    .Join(
                        deletedPackages,
                        knownPackage => knownPackage,
                        removedPackage => removedPackage,
                        (knownPackage, removedPackage) => knownPackage)
                    .ToList();
                foreach (var file in packagesToDelete)
                {
                    _knownPackages.Remove(file);
                }

                var actionsToDelete = _actions
                    .Join(
                        packagesToDelete,
                        pair => pair.Value.Package,
                        removedPackage => removedPackage,
                        (pair, removedPackage) => pair.Key)
                    .ToList();
                foreach (var id in actionsToDelete)
                {
                    _actions.Remove(id);
                }
            }
        }
    }
}
