//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Core.Actions
{
    [TestFixture]
    public sealed class ActionDefinitionTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithEmptyPackageName()
        {
            Assert.Throws<ArgumentException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    string.Empty,
                    "1.0.0",
                    "a",
                    "b",
                    new ActionParameterDefinition[0]));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithEmptyPackageVersion()
        {
            Assert.Throws<ArgumentException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    "a",
                    string.Empty,
                    "a",
                    "b",
                    new ActionParameterDefinition[0]));
        }

        [Test]
        public void Create()
        {
            var id = new ActionId("a");
            var packageName = "a";
            var packageVersion = "1.0.0";
            var typeName = "a";
            var methodName = "b";
            var parameters = new ActionParameterDefinition[0];
            var definition = new ActionDefinition(
                id,
                packageName,
                packageVersion,
                typeName,
                methodName,
                parameters);

            Assert.AreEqual(id, definition.Id);
            Assert.AreEqual(new PackageName(packageName, new SemanticVersion(packageVersion)), definition.Package);
            Assert.AreEqual(typeName, definition.ActionType);
            Assert.AreEqual(methodName, definition.ActionMethod);
            Assert.That(definition.Parameters, Is.EquivalentTo(parameters));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullAssemblyName()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    "a",
                    "1.0.0",
                    null,
                    "a",
                    new ActionParameterDefinition[0]));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullId()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ActionDefinition(
                    null,
                    "a",
                    "1.0.0",
                    "a",
                    "b",
                    new ActionParameterDefinition[0]));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullMethod()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    "a",
                    "1.0.0",
                    "a",
                    null,
                    new ActionParameterDefinition[0]));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullPackageName()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    null,
                    "1.0.0",
                    "a",
                    "b",
                    new ActionParameterDefinition[0]));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullPackageVersion()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    "a",
                    null,
                    "a",
                    "b",
                    new ActionParameterDefinition[0]));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullParameters()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ActionDefinition(
                    new ActionId("a"),
                    "a",
                    "1.0.0",
                    "a",
                    "b",
                    null));
        }
    }
}
