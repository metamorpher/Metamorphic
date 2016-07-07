//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication;

namespace Metamorphic.Core.Commands
{
    /// <summary>
    /// Defines the communication subjects for the Apollo application.
    /// </summary>
    public static class CommunicationSubjects
    {
        /// <summary>
        /// The subject group that is used to group all action related subjects.
        /// </summary>
        public const string ActionGroup
            = "Actions";

        /// <summary>
        /// The subject group that is used to group all application related subjects.
        /// </summary>
        public const string ApplicationGroup
            = "ApplicationInfo";

        /// <summary>
        /// The communication subject for applications that deal with action information.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly CommunicationSubject Actions
            = new CommunicationSubject("Metamorphic.Subject: Action");

        /// <summary>
        /// The version of the action subject group.
        /// </summary>
        public static readonly Version ActionVersion
            = new Version(1, 0);

        /// <summary>
        /// The communication subject for applications that deal with application information.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly CommunicationSubject Application
            = new CommunicationSubject("Metamorphic.Subject: Application");

        /// <summary>
        /// The version of the application subject group.
        /// </summary>
        public static readonly Version ApplicationVersion
            = new Version(1, 0);
    }
}
