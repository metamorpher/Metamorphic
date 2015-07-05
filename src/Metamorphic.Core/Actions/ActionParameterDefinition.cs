//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
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
        /// <param name="type">The type of the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public ActionParameterDefinition(Type type, string name)
        {
            {
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);
            }

            Type = type;
            Name = name;
        }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public Type Type
        {
            [DebuggerStepThrough]
            get;
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