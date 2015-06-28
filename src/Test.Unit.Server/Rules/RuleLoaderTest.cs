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
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Metamorphic.Server.Rules
{
    [TestFixture]
    public sealed class RuleLoaderTest
    {
        public void IsValidWithActionWithInvalidId()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
                {
                    Name = "a",
                    Action = new ActionRuleDefinition
                        {
                            Id = "b",
                            Parameters = new Dictionary<string, object>(),
                        },
                    Condition = new List<ConditionRuleDefinition>(),
                    Enabled = true,
                    Signal = new SignalRuleDefinition
                        {
                            Id = "c",
                            Parameters = new Dictionary<string, object>(),
                        }
                };
            Assert.IsFalse(loader.IsValid(definition, s => false, s => true));
        }

        public void IsValidWithActionWithInvalidSignalParameterReference()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>
                    {
                        ["c"] = "{{signal.d}}"
                    },
                },
                Condition = new List<ConditionRuleDefinition>(),
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "d",
                    Parameters = new Dictionary<string, object>
                    {
                        ["e"] = "f"
                    },
                }
            };
            Assert.IsFalse(loader.IsValid(definition, s => true, s => true));
        }

        public void IsValidWithConditionWithIncorrectName()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Condition = new List<ConditionRuleDefinition>
                {
                    new ConditionRuleDefinition
                    {
                        Name = "c",
                        Pattern = "d",
                        Type
                    },
                },
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            Assert.IsFalse(loader.IsValid(definition, s => true, s => true));
        }

        public void IsValidWithConditionWithInvalidType()
        { }

        public void IsValidWithMissingAction()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
            {
                Name = "a",
                Action = null,
                Condition = new List<ConditionRuleDefinition>(),
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            Assert.IsFalse(loader.IsValid(definition, s => true, s => true));
        }

        public void IsValidWithMissingName()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
            {
                Name = string.Empty,
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Condition = new List<ConditionRuleDefinition>(),
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            Assert.IsFalse(loader.IsValid(definition, s => true, s => true));
        }

        public void IsValidWithMissingSignal()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Condition = new List<ConditionRuleDefinition>(),
                Enabled = true,
                Signal = null
            };
            Assert.IsFalse(loader.IsValid(definition, s => true, s => true));
        }

        public void IsValidWithSignalWithInvalidType()
        {
            var loader = new RuleLoader(
                s => false,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));

            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Condition = new List<ConditionRuleDefinition>(),
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            Assert.IsFalse(loader.IsValid(definition, s => true, s => false));
        }

        public void IsValidWithSignalWithPartialParameters()
        { }

        [Test]
        public void LoadRuleWithActionWithParameters()
        {
            var fileName = "Valid_ActionWithParameters.mmrule";

            var loader = new RuleLoader(
                s => true,
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(0, definition.Condition.Count);
            Assert.AreEqual("Signal", definition.Signal.Id);

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
