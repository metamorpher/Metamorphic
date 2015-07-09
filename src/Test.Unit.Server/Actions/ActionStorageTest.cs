//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core.Actions;
using NUnit.Framework;

namespace Metamorphic.Server.Actions
{
    [TestFixture]
    public sealed class ActionStorageTest
    {
        [Test]
        public void Add()
        {
            var storage = new ActionStorage();

            Action action = () => { };
            var definition = new ActionDefinition(
                new ActionId("a"),
                new ActionParameterDefinition[0],
                action);
            storage.Add(definition);

            Assert.IsTrue(storage.HasActionFor(definition.Id));
            Assert.AreSame(definition, storage.Action(definition.Id));
        }

        [Test]
        public void AddWithDuplicateDefinition()
        {
            var storage = new ActionStorage();

            var id = new ActionId("a");
            Action action = () => { };
            var definition = new ActionDefinition(
                id,
                new ActionParameterDefinition[0],
                action);
            storage.Add(definition);

            Assert.IsTrue(storage.HasActionFor(definition.Id));
            Assert.AreSame(definition, storage.Action(definition.Id));

            var otherDefinition = new ActionDefinition(
                id,
                new ActionParameterDefinition[0],
                action);
            Assert.Throws<DuplicateActionDefinitionException>(() => storage.Add(definition));
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
