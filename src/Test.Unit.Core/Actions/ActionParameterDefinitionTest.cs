//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
        public void CreateWithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ActionParameterDefinition(string.Empty));
        }

        [Test]
        public void CreateWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterDefinition(null));
        }
    }
}
