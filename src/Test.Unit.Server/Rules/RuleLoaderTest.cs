//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Metamorphic.Server.Rules
{
    [TestFixture]
    public sealed class RuleLoaderTest
    {
        public void IsValidWithActionWithInvalidId()
        { }

        public void IsValidWithActionWithInvalidTriggerParameterReference()
        { }

        public void IsValidWithCriteriaWithIncorrectName()
        { }

        public void IsValidWithCriteriaWithInvalidType()
        { }

        public void IsValidWithMissingAction()
        { }

        public void IsValidWithMissingName()
        { }

        public void IsValidWithMissingTrigger()
        { }

        public void IsValidWithTriggerWithInvalidType()
        { }

        public void IsValidWithTriggerWithPartialParameters()
        { }

        [Test]
        public void LoadRuleWithActionWithParameters()
        {
            var fileName = "ActionWithParameters.mmrule";

            var loader = new RuleLoader();
            var definition = loader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(0, definition.Condition.Count);
            Assert.AreEqual("Trigger", definition.Trigger.Type);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void LoadRuleWithActionWithParametersReferencingTrigger()
        { }

        [Test]
        public void LoadRuleWithContainsCriteria()
        { }

        [Test]
        public void LoadRuleWithEqualityCriteria()
        { }

        [Test]
        public void LoadRuleWithGreaterThanCriteria()
        { }

        [Test]
        public void LoadRuleWithLessThanCriteria()
        { }

        [Test]
        public void LoadRuleWithMatchRegexCriteria()
        { }

        [Test]
        public void LoadRuleWithNotContainsCriteria()
        { }

        [Test]
        public void LoadRuleWithNotEqualityCriteria()
        { }

        [Test]
        public void LoadRuleWithNotMatchRegexCriteria()
        { }

        [Test]
        public void LoadRuleWithSimpleTriggerAndNoCriteria()
        { }

        [Test]
        public void LoadRuleWithTriggerWithParametersAndNoCriteria()
        { }

        [Test]
        public void LoadWithActionWithoutReference()
        { }

        [Test]
        public void LoadWithCriteriaWithoutName()
        { }

        [Test]
        public void LoadWithCriteriaWithoutPattern()
        { }

        [Test]
        public void LoadWithCriteriaWithoutType()
        { }

        [Test]
        public void LoadWithEmptyFile()
        { }

        [Test]
        public void LoadWithEmptyFilePath()
        { }

        [Test]
        public void LoadWithMissingRuleEnabledSwitch()
        { }

        [Test]
        public void LoadWithMissingRuleDescription()
        { }

        [Test]
        public void LoadWithMissingRuleName()
        { }

        [Test]
        public void LoadWithNonExistingFilePath()
        { }

        [Test]
        public void LoadWithNullFilePath()
        { }

        [Test]
        public void LoadWithoutAction()
        { }

        [Test]
        public void LoadWithoutTrigger()
        { }

        [Test]
        public void LoadWithTriggerWithoutType()
        { }

        private static string RulePath()
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return path;
        }
    }
}
