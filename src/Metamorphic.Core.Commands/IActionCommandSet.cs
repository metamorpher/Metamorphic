//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Metamorphic.Core.Actions;
using Nuclei.Communication.Interaction;

namespace Metamorphic.Core.Commands
{
    /// <summary>
    /// Defines the interface for objects that provide a set of commands used to interact with actions.
    /// </summary>
    public interface IActionCommandSet : ICommandSet
    {
        /// <summary>
        /// Returns a task that will contain the <see cref="ActionDefinition"/> for the action with the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <param name="retryCount">The maximum number of times the message should be resend if the initial message send fails.</param>
        /// <param name="timeoutInMilliseconds">The maximum time allowed to expire between the message send and the reception of the response.</param>
        /// <returns>
        /// A task which will contain the <see cref="ActionDefinition"/> for the action with the matching <see cref="ActionId"/> if
        /// it exists; otherwise the task will return <see langword="null" />.
        /// </returns>
        Task<ActionDefinition> Action(
            ActionId id,
            [InvocationRetryCount]int retryCount = CommandConstants.DefaultRetryCount,
            [InvocationTimeout]int timeoutInMilliseconds = CommandConstants.DefaultTimeoutInMilliseconds);

        /// <summary>
        /// Returns a task that will contain the <see cref="ActionDefinition"/> for the action with the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <param name="retryCount">The maximum number of times the message should be resend if the initial message send fails.</param>
        /// <param name="timeoutInMilliseconds">The maximum time allowed to expire between the message send and the reception of the response.</param>
        /// <returns>
        /// A task which will contain the <see cref="ActionDefinition"/> for the action with the matching <see cref="ActionId"/> if
        /// it exists; otherwise the task will return <see langword="null" />.
        /// </returns>
        Task<bool> HasActionFor(
            ActionId id,
            [InvocationRetryCount]int retryCount = CommandConstants.DefaultRetryCount,
            [InvocationTimeout]int timeoutInMilliseconds = CommandConstants.DefaultTimeoutInMilliseconds);
    }
}
