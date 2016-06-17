//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Metamorphic.Core.Actions
{
    [TestFixture]
    public sealed class ActionParameterDefinitionTest
    {
        [Test]
        public void Create()
        {
            var name = "a";
            var definition = new ActionParameterDefinition(name);

            Assert.AreSame(name, definition.Name);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionParameterDefinition",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterDefinition(string.Empty));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionParameterDefinition",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterDefinition(null));
        }
    }
}
