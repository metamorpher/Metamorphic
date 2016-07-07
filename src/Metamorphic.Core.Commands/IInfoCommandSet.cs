//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Nuclei.Communication.Interaction;

namespace Metamorphic.Core.Commands
{
    /// <summary>
    /// Defines the interface for objects that provide a set of commands used to provide information about the application.
    /// </summary>
    public interface IInfoCommandSet : ICommandSet
    {
        /// <summary>
        /// Returns a task that will contain the <see cref="ApplicationInformation"/> for the application.
        /// </summary>
        /// <param name="retryCount">The maximum number of times the message should be resend if the initial message send fails.</param>
        /// <param name="timeoutInMilliseconds">The maximum time allowed to expire between the message send and the reception of the response.</param>
        /// <returns>
        /// A task which will contain the <see cref="ApplicationInformation"/> for the application.
        /// </returns>
        Task<ApplicationInformation> Info(
            [InvocationRetryCount]int retryCount = CommandConstants.DefaultRetryCount,
            [InvocationTimeout]int timeoutInMilliseconds = CommandConstants.DefaultTimeoutInMilliseconds);
    }
}
