//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Defines an attribute that indicates that a methods is an action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [SuppressMessage(
        "Microsoft.Design",
        "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "The string parameter in the constructor is turned into an ActionId so there is a property for the parameter.")]
    public sealed class ActionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionAttribute"/> class.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        public ActionAttribute(string id)
        {
            Id = new ActionId(id);
        }

        /// <summary>
        /// Gets the ID of the action.
        /// </summary>
        public ActionId Id
        {
            get;
        }
    }
}
