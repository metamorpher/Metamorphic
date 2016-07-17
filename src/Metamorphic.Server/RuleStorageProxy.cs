//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Metamorphic.Core.Commands;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;

namespace Metamorphic.Server
{
    internal sealed class RuleStorageProxy : IRuleStorageProxy
    {
        /// <summary>
        /// The object that is used to send commands to the remote application.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints _commandSender;

        /// <summary>
        /// The list of remote endpoints that may have the command sets we need.
        /// </summary>
        /// <remarks>
        /// In reality we only expect there to be one or none.
        /// </remarks>
        private readonly List<EndpointId> _knownEndpoints
            = new List<EndpointId>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleStorageProxy"/> class.
        /// </summary>
        /// <param name="commandSender">The object that is used to send commands to the remote application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandSender"/> is <see langword="null" />.
        /// </exception>
        public RuleStorageProxy(ISendCommandsToRemoteEndpoints commandSender)
        {
            if (commandSender == null)
            {
                throw new ArgumentNullException("commandSender");
            }

            _commandSender = commandSender;
            _commandSender.OnEndpointConnected += HandleEndpointConnected;
            _commandSender.OnEndpointDisconnected += HandleEndpointDisconnected;
        }

        private void HandleEndpointConnected(object sender, EndpointEventArgs e)
        {
            if (!_knownEndpoints.Contains(e.Endpoint))
            {
                _knownEndpoints.Add(e.Endpoint);
            }
        }

        private void HandleEndpointDisconnected(object sender, EndpointEventArgs e)
        {
            if (_knownEndpoints.Contains(e.Endpoint))
            {
                _knownEndpoints.Remove(e.Endpoint);
            }
        }

        /// <summary>
        /// Returns a collection containing all rules that are applicable for the given signal type.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor from which the signal originated.</param>
        /// <returns>A collection containing all the rule definitions that apply to the given signal.</returns>
        public IEnumerable<RuleDefinition> RulesForSignal(SignalTypeId sensorId)
        {
            foreach (var id in _knownEndpoints)
            {
                if (!_commandSender.HasCommandFor(id, typeof(IRuleCommandSet)))
                {
                    continue;
                }

                var command = _commandSender.CommandsFor<IRuleCommandSet>(id);
                if (command == null)
                {
                    continue;
                }

                var task = command.RulesForSignal(sensorId);

                try
                {
                    task.Wait();
                    return task.IsFaulted ? new RuleDefinition[0] : task.Result;
                }
                catch (AggregateException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
