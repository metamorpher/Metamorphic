//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;
using Metamorphic.Core;
using Metamorphic.Core.Commands;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;

namespace Metamorphic.Agent
{
    /// <summary>
    /// Defines an <see cref="IInitializeCommunicationInstances"/> for the example application.
    /// </summary>
    internal sealed class CommunicationInitializer : IInitializeCommunicationInstances
    {
        private static ApplicationInformation Info()
        {
            return new ApplicationInformation();
        }

        /// <summary>
        /// The dependency injection context that is used to resolve instances.
        /// </summary>
        private readonly IComponentContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationInitializer"/> class.
        /// </summary>
        /// <param name="context">The dependency injection context that is used to resolve instances.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public CommunicationInitializer(IComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }

        /// <summary>
        /// Registers all the commands that are provided by the current application.
        /// </summary>
        public void RegisterProvidedCommands()
        {
            // Have to publish some commands because otherwise the communication system doesn't let
            // us talk to the storage application
            var map = CommandMapper<IInfoCommandSet>.Create();
            map.From<int, int>((command, retry, timeout) => command.Info(retry, timeout))
                .To(() => Info());

            var collection = _context.Resolve<RegisterCommand>();
            collection(
                map.ToMap(),
                new[]
                    {
                        new SubjectGroupIdentifier(
                            CommunicationSubjects.Application,
                            CommunicationSubjects.ApplicationVersion,
                            CommunicationSubjects.ApplicationGroup),
                    });
        }

        /// <summary>
        /// Registers all the commands that the current application requires.
        /// </summary>
        public void RegisterRequiredCommands()
        {
            var registration = _context.Resolve<RegisterRequiredCommand>();
            registration(
                typeof(IActionCommandSet),
                new[]
                {
                    new SubjectGroupIdentifier(
                        CommunicationSubjects.Actions,
                        CommunicationSubjects.ActionVersion,
                        CommunicationSubjects.ActionGroup),
                });

            registration(
                typeof(IRuleCommandSet),
                new[]
                {
                    new SubjectGroupIdentifier(
                        CommunicationSubjects.Rule,
                        CommunicationSubjects.RuleVersion,
                        CommunicationSubjects.RuleGroup),
                });
        }

        /// <summary>
        /// Registers all the notifications that are provided by the current application.
        /// </summary>
        public void RegisterProvidedNotifications()
        {
            // Do nothing for now
        }

        /// <summary>
        /// Registers all the notifications that the current application requires.
        /// </summary>
        public void RegisterRequiredNotifications()
        {
            // No notifications required
        }

        /// <summary>
        /// Performs initialization routines that need to be performed before to the starting of the
        /// communication system.
        /// </summary>
        public void InitializeBeforeCommunicationSignIn()
        {
            // Do nothing for now ...
        }

        /// <summary>
        /// Performs initialization routines that need to be performed after the sign in of the
        /// communication system.
        /// </summary>
        public void InitializeAfterCommunicationSignIn()
        {
            // Do nothing for now ...
        }
    }
}
