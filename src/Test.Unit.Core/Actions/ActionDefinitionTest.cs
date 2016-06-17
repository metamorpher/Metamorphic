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
    public sealed class ActionDefinitionTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullAction()
        {
            var id = new ActionId("a");
            var parameters = new ActionParameterDefinition[0];
            Assert.Throws<ArgumentNullException>(() => new ActionDefinition(id, parameters, null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullId()
        {
            var parameters = new ActionParameterDefinition[0];
            Action action = () => { };
            Assert.Throws<ArgumentNullException>(() => new ActionDefinition(null, parameters, action));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionDefinition",
            Justification = "Testing to see that the constructor throws.")]
        public void CreateWithNullParameters()
        {
            var id = new ActionId("a");
            Action action = () => { };
            Assert.Throws<ArgumentNullException>(() => new ActionDefinition(id, null, action));
        }

        [Test]
        public void Invoke()
        {
            var id = new ActionId("a");
            var parameters = new[]
                {
                    new ActionParameterDefinition("s"),
                };

            var wasInvoked = false;
            var value = string.Empty;
            Action<string> action = s =>
                {
                    value = s;
                    wasInvoked = true;
                };
            var definition = new ActionDefinition(id, parameters, action);

            var input = "input";
            var values = new[]
                {
                    new ActionParameterValueMap(parameters[0], input),
                };
            definition.Invoke(values);
            Assert.IsTrue(wasInvoked);
            Assert.AreSame(input, value);
        }

        [Test]
        public void InvokeWithMissingParameter()
        {
            var id = new ActionId("a");
            var parameters = new[]
                {
                    new ActionParameterDefinition("s"),
                };

            var value = string.Empty;
            Action<string> action = s =>
                {
                    value = s;
                };
            var definition = new ActionDefinition(id, parameters, action);

            var values = new ActionParameterValueMap[0];
            Assert.Throws<MissingActionParameterException>(() => definition.Invoke(values));
        }

        [Test]
        public void InvokeWithNullParameters()
        {
            var id = new ActionId("a");
            var parameters = new ActionParameterDefinition[0];

            Action action = () => { };
            var definition = new ActionDefinition(id, parameters, action);

            Assert.Throws<ArgumentNullException>(() => definition.Invoke(null));
        }
    }
}
