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

        public void CreateWithNullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => new ActionParameterValueMap(null, "a"));
        }
    }
}
