//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Commands;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;

namespace Metamorphic.Server
{
    internal sealed class ActionStorageProxy : IActionStorageProxy
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
        /// Initializes a new instance of the <see cref="ActionStorageProxy"/> class.
        /// </summary>
        /// <param name="commandSender">The object that is used to send commands to the remote application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandSender"/> is <see langword="null" />.
        /// </exception>
        public ActionStorageProxy(ISendCommandsToRemoteEndpoints commandSender)
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
        /// Returns the <see cref="ActionDefinition"/> which the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="action">The ID of the action</param>
        /// <returns>The action definition with the given ID.</returns>
        public ActionDefinition Action(ActionId action)
        {
            foreach (var id in _knownEndpoints)
            {
                if (!_commandSender.HasCommandFor(id, typeof(IActionCommandSet)))
                {
                    continue;
                }

                var command = _commandSender.CommandsFor<IActionCommandSet>(id);
                if (command == null)
                {
                    continue;
                }

                var task = command.Action(action);

                try
                {
                    task.Wait();
                    return task.IsFaulted ? null : task.Result;
                }
                catch (AggregateException)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the storage has an <see cref="ActionDefinition"/>
        /// with the given <see cref="ActionId"/>.
        /// </summary>
        /// <param name="action">The ID of the action.</param>
        /// <returns>
        ///   <see langword="true" /> if the storage has a definition with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasActionFor(ActionId action)
        {
            foreach (var id in _knownEndpoints)
            {
                if (!_commandSender.HasCommandFor(id, typeof(IActionCommandSet)))
                {
                    continue;
                }

                var command = _commandSender.CommandsFor<IActionCommandSet>(id);
                if (command == null)
                {
                    continue;
                }

                var task = command.HasActionFor(action);

                try
                {
                    task.Wait();
                    return task.IsFaulted ? false : task.Result;
                }
                catch (AggregateException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
