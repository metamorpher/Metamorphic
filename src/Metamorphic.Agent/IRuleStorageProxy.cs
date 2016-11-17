//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;

namespace Metamorphic.Agent
{
    /// <summary>
    /// Defines the interface for objects that provide a proxy to an rule storage collection.
    /// </summary>
    internal interface IRuleStorageProxy
    {
        /// <summary>
        /// Returns a collection containing all rules that are applicable for the given signal type.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <returns>A collection containing all the rule definitions that apply to the given signal.</returns>
        IEnumerable<RuleDefinition> RulesForSignal(SignalTypeId sensorId);
    }
}
