//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Actions;
using NUnit.Framework;

namespace Metamorphic.Core.Jobs
{
    [TestFixture]
    public sealed class JobTest
    {
        [Test]
        public void Construct()
        {
            var type = new ActionId("a");
            var Job = new Job(type, new Dictionary<string, object>());

            Assert.AreSame(type, Job.Action);
        }

        [Test]
        public void ContainsParameterWithNullString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.IsFalse(Job.ContainsParameter(null));
        }

        [Test]
        public void ContainsParameterWithEmptyString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.IsFalse(Job.ContainsParameter(string.Empty));
        }

        [Test]
        public void ContainsParameterWithNonExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.IsFalse(Job.ContainsParameter("c"));
        }

        [Test]
        public void ContainsParameterWithExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.IsTrue(Job.ContainsParameter("a"));
        }

        [Test]
        public void ParameterValueWithNullString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => Job.ParameterValue(null));
        }

        [Test]
        public void ParameterValueWithEmptyString()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => Job.ParameterValue(string.Empty));
        }

        [Test]
        public void ParameterValueWithNonExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.Throws<ParameterNotFoundException>(() => Job.ParameterValue("c"));
        }

        [Test]
        public void ParameterValueWithExistingParameter()
        {
            var type = new ActionId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var Job = new Job(type, parameters);

            Assert.AreEqual("b", Job.ParameterValue("a"));
        }
    }
}
