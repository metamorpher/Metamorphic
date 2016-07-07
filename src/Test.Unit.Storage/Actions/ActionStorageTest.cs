//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core.Actions;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Storage.Actions
{
    [TestFixture]
    public sealed class ActionStorageTest
    {
        [Test]
        public void Add()
        {
            var storage = new ActionStorage();

            var definition = new ActionDefinition(
                new ActionId("a"),
                "a",
                "1.0.0",
                "a",
                "b",
                new ActionParameterDefinition[0]);
            storage.Add(definition);

            Assert.IsTrue(storage.HasActionFor(definition.Id));
            Assert.AreSame(definition, storage.Action(definition.Id));
        }

        [Test]
        public void AddWithDuplicateDefinition()
        {
            var storage = new ActionStorage();

            var id = new ActionId("a");
            var definition = new ActionDefinition(
                new ActionId("a"),
                "a",
                "1.0.0",
                "a",
                "b",
                new ActionParameterDefinition[0]);
            storage.Add(definition);

            Assert.IsTrue(storage.HasActionFor(definition.Id));
            Assert.AreSame(definition, storage.Action(definition.Id));

            var otherDefinition = new ActionDefinition(
                id,
                "c",
                "2.0.0",
                "d",
                "e",
                new ActionParameterDefinition[0]);
            Assert.Throws<DuplicateActionDefinitionException>(() => storage.Add(otherDefinition));
        }

        [Test]
        public void AddWithNullDefinition()
        {
            var storage = new ActionStorage();
            Assert.Throws<ArgumentNullException>(() => storage.Add(null));
        }

        [Test]
        public void HasActionForWithNullId()
        {
            var storage = new ActionStorage();
            Assert.IsFalse(storage.HasActionFor(null));
        }

        [Test]
        public void HasActionForWithUnknownId()
        {
            var storage = new ActionStorage();
            Assert.IsFalse(storage.HasActionFor(new ActionId("a")));
        }
    }
}
