//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;

namespace Metamorphic.Storage.Rules
{
    /// <summary>
    /// Defines the interface for objects that store rules.
    /// </summary>
    internal interface IStoreRules
    {
        /// <summary>
        /// Adds a new <see cref="RuleDefinition"/> that was created from the given origin.
        /// </summary>
        /// <param name="origin">The origin of the data that was used to create the rule.</param>
        /// <param name="rule">The rule definition.</param>
        void Add(RuleOrigin origin, RuleDefinition rule);

        /// <summary>
        /// Removes the <see cref="RuleDefinition"/> that is associated with the given origin.
        /// </summary>
        /// <param name="origin">The origin of the rule.</param>
        void Remove(RuleOrigin origin);

        /// <summary>
        /// Returns a collection containing all rules that are applicable for the given signal type.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <returns>A collection containing all the rule definitions that apply to the given signal.</returns>
        RuleDefinition[] RulesForSignal(SignalTypeId sensorId);

        /// <summary>
        /// Updates an existing <see cref="RuleDefinition"/>.
        /// </summary>
        /// <param name="origin">The origin of the data that was used to create the rule.</param>
        /// <param name="rule">The rule definition.</param>
        void Update(RuleOrigin origin, RuleDefinition rule);
    }
}
