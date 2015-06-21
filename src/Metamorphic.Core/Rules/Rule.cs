//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Properties;
using Metamorphic.Core.Signals;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines methods for transforming signals into work.
    /// </summary>
    public sealed class Rule
    {

        internal Rule(RuleDefinition definition)
        {
            {
                Lokad.Enforce.Argument(() => definition);
                Lokad.Enforce.With<InvalidRuleDefinitionException>(
                    definition.IsValid(),
                    Resources.Exceptions_Messages_InvalidRuleDefinition);
            }
        }

        /// <summary>
        /// Returns a value indicating whether or not the current rule applies to the given signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ShouldProcess(Signal signal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the signal identifier to which this rule applies.
        /// </summary>
        public string SignalType
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new <see cref="Job"/> based on the current rule and the given signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The newly created job.</returns>
        public Job ToJob(Signal signal)
        {
            throw new NotImplementedException();
        }
    }
}
