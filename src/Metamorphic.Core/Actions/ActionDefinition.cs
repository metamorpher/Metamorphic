//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Metamorphic.Core.Properties;
using NuGet;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Defines the methods required for the invocation of an action.
    /// </summary>
    [Serializable]
    public sealed class ActionDefinition
    {
        /// <summary>
        /// The ID of the NuGet package in which the current action is defined.
        /// </summary>
        private readonly string _packageId;

        /// <summary>
        /// The semantic version of the NuGet package in which the current action is defined.
        /// </summary>
        private readonly string _packageVersion;

        /// <summary>
        /// The collection of parameters for the action.
        /// </summary>
        private readonly List<ActionParameterDefinition> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDefinition"/> class.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <param name="packageName">The name of the NuGet package from which the files were retrieved.</param>
        /// <param name="packageVersion">The version of the NuGet package from which the files were retrieved.</param>
        /// <param name="actionAssemblyTypeName">The assembly qualified name of the <see cref="Type"/> that holds the action method.</param>
        /// <param name="actionMethodName">The name of the method that should be invoked when the action is executed.</param>
        /// <param name="parameters">The collection of parameters for the action.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="packageName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="packageVersion"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionAssemblyTypeName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="actionAssemblyTypeName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionMethodName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="actionMethodName"/> is an empty string..
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        public ActionDefinition(ActionId id, string packageName, string packageVersion, string actionAssemblyTypeName, string actionMethodName, ActionParameterDefinition[] parameters)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (packageName == null)
            {
                throw new ArgumentNullException("packageName");
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "packageName");
            }

            if (packageVersion == null)
            {
                throw new ArgumentNullException("packageVersion");
            }

            if (string.IsNullOrWhiteSpace(packageVersion))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "packageVersion");
            }

            if (actionAssemblyTypeName == null)
            {
                throw new ArgumentNullException("actionAssemblyTypeName");
            }

            if (string.IsNullOrWhiteSpace(actionAssemblyTypeName))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "actionAssemblyTypeName");
            }

            if (actionMethodName == null)
            {
                throw new ArgumentNullException("actionMethodName");
            }

            if (string.IsNullOrWhiteSpace(actionMethodName))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "actionMethodName");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            Id = id;
            ActionType = actionAssemblyTypeName;
            ActionMethod = actionMethodName;

            // Store the package information as serializable elements because NuGet.Core.PackageName is not serializable
            _packageId = packageName;
            _packageVersion = packageVersion;
            _parameters = new List<ActionParameterDefinition>(parameters);
        }

        /// <summary>
        /// Gets the assembly qualified type name of the type that contains the action method.
        /// </summary>
        public string ActionType
        {
            get;
        }

        /// <summary>
        /// Gets the name of the method that should be invoked when the action is executed.
        /// </summary>
        public string ActionMethod
        {
            get;
        }

        /// <summary>
        /// Gets the ID of the action.
        /// </summary>
        public ActionId Id
        {
            [DebuggerStepThrough]
            get;
        }

        /// <summary>
        /// Gets information about the NuGet package that contains the action.
        /// </summary>
        public PackageName Package
        {
            get
            {
                return new PackageName(_packageId, new SemanticVersion(_packageVersion));
            }
        }

        /// <summary>
        /// Gets the collection of parameters that are required for the current action to be invoked.
        /// </summary>
        public IReadOnlyCollection<ActionParameterDefinition> Parameters
        {
            get
            {
                return _parameters.AsReadOnly();
            }
        }
    }
}
