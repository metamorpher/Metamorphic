//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Core.Commands
{
    /// <summary>
    /// Defines constants used with commands.
    /// </summary>
    public static class CommandConstants
    {
        /// <summary>
        /// The default number of retries for a command invocation.
        /// </summary>
        public const int DefaultRetryCount = 3;

        /// <summary>
        /// The default amount of time, in milliseconds, that the command sender waits before it declares the command
        /// to have time-out.
        /// </summary>
        public const int DefaultTimeoutInMilliseconds = 500;
    }
}
