//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Properties;

namespace Metamorphic.Core.Jobs
{
    /// <summary>
    /// Defines an amount of work that should be done.
    /// </summary>
    [Serializable]
    public sealed class Job
    {
        // At a later stage we can add things like:
        // - Agent specific conditions (e.g. agent id, required apps, required agent tags etc.)
        // - Expiry times

        /// <summary>
        /// The collection of parameters for the current job. Note that all parameter names are
        /// stored in lower case so as to provide case-insensitive comparisons between the signal and
        /// rule parameter names.
        /// </summary>
        private readonly Dictionary<string, object> _parameters
            = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="action">The ID of the action that should be executed.</param>
        /// <param name="parameters">The collection containing the parameters for the action.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        public Job(ActionId action, IDictionary<string, object> parameters)
        {
            {
                Lokad.Enforce.Argument(() => action);
                Lokad.Enforce.Argument(() => parameters);
            }

            Action = action;
            foreach (var pair in parameters)
            {
                _parameters.Add(pair.Key.ToLower(), pair.Value);
            }
        }

        /// <summary>
        /// Gets the ID of the action that should be executed.
        /// </summary>
        public ActionId Action
        {
            get;
        }

        /// <summary>
        /// Returns a value indicating whether the current job has a parameter with the given name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ContainsParameter(string name)
        {
            if (name == null)
            {
                return false;
            }

            return _parameters.ContainsKey(name.ToLower());
        }

        /// <summary>
        /// Returns the collection of parameter names for the current job.
        /// </summary>
        /// <returns>The collection of parameter names.</returns>
        public IEnumerable<string> ParameterNames()
        {
            return _parameters.Keys;
        }

        /// <summary>
        /// Returns the parameter value for the given parameter for the current job.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value for the given parameter with the provided name.</returns>
        public object ParameterValue(string name)
        {
            if (!ContainsParameter(name))
            {
                throw new ParameterNotFoundException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_ParameterNotFound_WithName,
                        name));
            }

            return _parameters[name.ToLower()];
        }
    }
}
