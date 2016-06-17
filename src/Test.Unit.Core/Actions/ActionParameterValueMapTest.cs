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
    public sealed class ActionParameterValueMapTest
    {
        public void Create()
        {
            var value = "a";
            var definition = new ActionParameterDefinition("b");
            var map = new ActionParameterValueMap(definition, value);

            Assert.AreSame(definition, map.Parameter);
            Assert.AreSame(value, map.Value);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Actions.ActionParameterValueMap",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValueMap(null, "a"));
        }
    }
}
