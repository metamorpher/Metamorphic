//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Metamorphic.Core.Actions;
using Moq;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Storage.Actions
{
    [TestFixture]
    public sealed class ActionStorageProxyTest
    {
        [Test]
        public void Action()
        {
            var definition = new ActionDefinition(
                new ActionId("a"),
                "a",
                "1.0.0",
                "a",
                "b",
                new ActionParameterDefinition[0]);

            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Action(It.IsAny<ActionId>()))
                    .Returns(definition)
                    .Verifiable();
            }

            var proxy = new ActionStorageProxy(storage.Object);
            Assert.AreSame(definition, proxy.Action(new ActionId("a")));
        }

        [Test]
        public void Add()
        {
            ActionDefinition storedAction = null;
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.Add(It.IsAny<ActionDefinition>()))
                    .Callback<ActionDefinition>(a => storedAction = a)
                    .Verifiable();
            }

            var proxy = new ActionStorageProxy(storage.Object);

            var definition = new ActionDefinition(
                new ActionId("a"),
                "a",
                "1.0.0",
                "a",
                "b",
                new ActionParameterDefinition[0]);
            proxy.Add(definition);

            Assert.AreSame(definition, storedAction);
        }

        [Test]
        public void HasActionFor()
        {
            var storage = new Mock<IStoreActions>();
            {
                storage.Setup(s => s.HasActionFor(It.IsAny<ActionId>()))
                    .Returns(true)
                    .Verifiable();
            }

            var proxy = new ActionStorageProxy(storage.Object);
            Assert.IsTrue(proxy.HasActionFor(new ActionId("a")));
        }
    }
}
