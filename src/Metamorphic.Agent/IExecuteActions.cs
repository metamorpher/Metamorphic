//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;

namespace Metamorphic.Agent
{
    /// <summary>
    /// Defines the interface for objects that execute job actions.
    /// </summary>
    internal interface IExecuteActions
    {
        /// <summary>
        /// Executes the actions necessary to complete the given job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="action">The action that should be executed for the job.</param>
        void Execute(Job job, ActionDefinition action);
    }
}
