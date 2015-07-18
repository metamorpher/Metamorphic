//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------


using System.Threading.Tasks;

namespace Metamorphic.Server.Jobs
{
    /// <summary>
    /// Defines the interface for objects that handle the processing of jobs.
    /// </summary>
    internal interface IProcessJobs
    {
        /// <summary>
        /// Starts the job executing process.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the job executing process.
        /// </summary>
        /// <param name="clearCurrentQueue">
        /// Indicates if the elements currently in the queue need to be processed before stopping or not.
        /// </param>
        /// <returns>A task that completes when the processor has stopped.</returns>
        Task Stop(bool clearCurrentQueue);
    }
}