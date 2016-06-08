//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server.Rules
{
    /// <summary>
    /// Defines the interface for objects that store rules.
    /// </summary>
    internal interface IStoreRules
    {
        /// <summary>
        /// Adds a new <see cref="Rule"/> that was created from the given file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        void Add(string filePath, Rule rule);

        /// <summary>
        /// Removes the <see cref="Rule"/> that is associated with the given file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        void Remove(string filePath);

        /// <summary>
        /// Returns a collection containing all rules that are applicable for the given signal type.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <returns></returns>
        IEnumerable<Rule> RulesForSignal(SignalTypeId sensorId);

        /// <summary>
        /// Updates an existing <see cref="Rule"/>.
        /// </summary>
        /// <param name="filePath">The full path to the rule file that was used to create the rule.</param>
        /// <param name="rule">The rule.</param>
        void Update(string filePath, Rule rule);
    }
}