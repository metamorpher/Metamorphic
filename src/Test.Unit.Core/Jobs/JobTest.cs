//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Actions;
using NUnit.Framework;

namespace Metamorphic.Core.Jobs
{
    [TestFixture]
    public sealed class JobTest
    {
        [Test]
        public void Create()
        {
            var type = new ActionId("a");
            var job = new Job(type, new Dictionary<string, object>());

            Assert.AreSame(type, job.Action);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Jobs.Job",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullAction()
        {
            Assert.Throws<ArgumentNullException>(() => new Job(null, new Dictionary<string, object>()));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Jobs.Job",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullParameters()
        {
            Assert.Throws<ArgumentNullException>(() => new Job(new ActionId("a"), null));
        }

        [Test]
        public void ContainsParameterWithNullString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.IsFalse(job.ContainsParameter(null));
        }

        [Test]
        public void ContainsParameterWithEmptyString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.IsFalse(job.ContainsParameter(string.Empty));
        }

        [Test]
        public void ContainsParameterWithNonExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.IsFalse(job.ContainsParameter("c"));
        }

        [Test]
        public void ContainsParameterWithExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.IsTrue(job.ContainsParameter("a"));
        }

        [Test]
        public void ParameterValueWithNullString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => job.ParameterValue(null));
        }

        [Test]
        public void ParameterValueWithEmptyString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => job.ParameterValue(string.Empty));
        }

        [Test]
        public void ParameterValueWithNonExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => job.ParameterValue("c"));
        }

        [Test]
        public void ParameterValueWithExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var job = new Job(type, parameters);

            Assert.AreEqual("b", job.ParameterValue("a"));
        }
    }
}
