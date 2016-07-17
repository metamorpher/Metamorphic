//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using Nuclei.Communication.Interaction;

namespace Metamorphic.Core.Commands
{
    /// <summary>
    /// Defines the interface for objects that provide a set of commands used to interact with rules.
    /// </summary>
    public interface IRuleCommandSet : ICommandSet
    {
        /// <summary>
        /// Returns a task that will contain the collection of <see cref="RuleDefinition"/> items that match the signal with the given <see cref="SignalTypeId"/>.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <param name="retryCount">The maximum number of times the message should be resend if the initial message send fails.</param>
        /// <param name="timeoutInMilliseconds">The maximum time allowed to expire between the message send and the reception of the response.</param>
        /// <returns>
        /// A task which will contain the collection of <see cref="RuleDefinition"/> items that match the signal with the given <see cref="SignalTypeId"/> if
        /// any exists.
        /// </returns>
        Task<RuleDefinition[]> RulesForSignal(
            SignalTypeId sensorId,
            [InvocationRetryCount]int retryCount = CommandConstants.DefaultRetryCount,
            [InvocationTimeout]int timeoutInMilliseconds = CommandConstants.DefaultTimeoutInMilliseconds);
    }
}
