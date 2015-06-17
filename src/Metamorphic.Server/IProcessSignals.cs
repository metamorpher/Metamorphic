//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Metamorphic.Server
{
    /// <summary>
    /// Defines the interface for objects that handle the processing of signals.
    /// </summary>
    internal interface IProcessSignals
    {
        /// <summary>
        /// Starts the symbol indexing process.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the symbol indexing process.
        /// </summary>
        /// <param name="clearCurrentQueue">
        /// Indicates if the elements currently in the queue need to be processed before stopping or not.
        /// </param>
        /// <returns>A task that completes when the indexer has stopped.</returns>
        Task Stop(bool clearCurrentQueue);
    }
}