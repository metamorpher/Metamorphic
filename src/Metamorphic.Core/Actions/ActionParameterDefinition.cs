//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Stores information about a single parameter on an action method.
    /// </summary>
    public sealed class ActionParameterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public ActionParameterDefinition(string name)
        {
            {
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);
            }

            Name = name;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get;
        }
    }
}
