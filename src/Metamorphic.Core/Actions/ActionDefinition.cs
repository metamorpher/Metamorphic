//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Metamorphic.Core.Properties;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Defines the methods required for the invocation of an action.
    /// </summary>
    public sealed class ActionDefinition
    {
        /// <summary>
        /// The delegate that should be invoked when the action is invoked.
        /// </summary>
        private readonly Delegate m_ActionToExecute;

        /// <summary>
        /// The collection of parameters for the action.
        /// </summary>
        private readonly List<ActionParameterDefinition> m_Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDefinition"/> class.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <param name="parameters">The collection of parameters for the action.</param>
        /// <param name="actionToExecute">The action that should be invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionToExecute"/> is <see langword="null" />.
        /// </exception>
        public ActionDefinition(ActionId id, ActionParameterDefinition[] parameters, Delegate actionToExecute)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => parameters);
                Lokad.Enforce.Argument(() => actionToExecute);
            }

            Id = id;
            m_Parameters = new List<ActionParameterDefinition>(parameters);
            m_ActionToExecute = actionToExecute;
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
        /// Invokes the command and returns the command return value.
        /// </summary>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The return value for the command.</returns>
        public void Invoke(ActionParameterValueMap[] parameters)
        {
            {
                Lokad.Enforce.Argument(() => parameters);
            }

            var mappedParameterValues = new object[m_Parameters.Count];
            for (int i = 0; i < m_Parameters.Count; i++)
            {
                var expectedParameter = m_Parameters[i];
                var providedParameter = parameters.FirstOrDefault(
                    m => string.Equals(m.Parameter.Name, expectedParameter.Name, StringComparison.Ordinal));
                if (providedParameter == null)
                {
                    throw new MissingActionParameterException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_MissingActionParameter_WithParameterName,
                            expectedParameter.Name));
                }

                mappedParameterValues[i] = providedParameter.Value;
            }

            m_ActionToExecute.DynamicInvoke(mappedParameterValues);
        }
    }
}
