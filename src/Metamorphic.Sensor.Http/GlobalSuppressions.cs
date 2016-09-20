//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
[assembly: SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1649:File name must match first type name",
    Scope = "type",
    Target = "~T:Metamorphic.Sensor.Http.WebApiApplication",
    Justification = "This class is the entry point for the ASP WebAPI site and should stay in the global.asx file.")]
[assembly: SuppressMessage(
    "Microsoft.Naming",
    "CA1703:ResourceStringsShouldBeSpelledCorrectly",
    MessageId = "tfsgit",
    Scope = "resource",
    Target = "Metamorphic.Sensor.Http.Properties.Resources.resources",
    Justification = "It's an REST API method call.")]
