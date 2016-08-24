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
    /// Maps a parameter definition to its value.
    /// </summary>
    public sealed class ActionParameterValueMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterValueMap"/> class.
        /// </summary>
        /// <param name="parameter">The definition of the current parameter.</param>
        /// <param name="value">The value of the current parameter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameter"/> is <see langword="null" />.
        /// </exception>
        public ActionParameterValueMap(ActionParameterDefinition parameter, object value)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            Parameter = parameter;
            Value = value;
        }

        /// <summary>
        /// Gets the parameter for the current map.
        /// </summary>
        public ActionParameterDefinition Parameter
        {
            [DebuggerStepThrough]
            get;
        }

        /// <summary>
        /// Gets the value for the current parameter.
        /// </summary>
        public object Value
        {
            [DebuggerStepThrough]
            get;
        }
    }
}
