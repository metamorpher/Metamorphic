//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Metamorphic.Server.Rules
{
    [TestFixture]
    public sealed class RuleLoaderTest
    {
        private static string RulePath()
        {
            var path = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"TestFiles");
            return path;
        }

        [Test]
        public void CreateDefinitionFromFileWithActionWithParameters()
        {
            var fileName = "ActionWithParameters.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

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
        public void CreateDefinitionFromFileWithActionWithParametersReferencingMultipleSignalParameters()
        {
            var fileName = "ActionWithParametersReferencingMultipleSignalParameters.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(0, definition.Condition.Count);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(2, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("baz"));
            Assert.AreEqual("otherstuff", definition.Signal.Parameters["baz"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("{{signal.bar}} {{signal.baz}}", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithActionWithParametersReferencingSignal()
        {
            var fileName = "ActionWithParametersReferencingSignal.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(0, definition.Condition.Count);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("{{signal.bar}}", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithEndsWithCondition()
        {
            var fileName = "EndsWithCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual("a", definition.Condition[0].Pattern);
            Assert.AreEqual("endswith", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithEqualsCondition()
        {
            var fileName = "EqualsCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual("a", definition.Condition[0].Pattern);
            Assert.AreEqual("equals", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithGreaterThanCondition()
        {
            var fileName = "GreaterThanCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual(10, definition.Condition[0].Pattern);
            Assert.AreEqual("greaterthan", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithLessThanCondition()
        {
            var fileName = "LessThanCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual(10, definition.Condition[0].Pattern);
            Assert.AreEqual("lessthan", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithMatchRegexCondition()
        {
            var fileName = "MatchRegexCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual("(.*)", definition.Condition[0].Pattern);
            Assert.AreEqual("matchregex", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithNotEqualsCondition()
        {
            var fileName = "NotEqualsCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual("a", definition.Condition[0].Pattern);
            Assert.AreEqual("notequals", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void CreateDefinitionFromFileWithNotMatchRegexCondition()
        {
            var fileName = "NotMatchRegexCondition.mmrule";

            var definition = RuleLoader.CreateDefinitionFromFile(Path.Combine(RulePath(), fileName));

            Assert.AreEqual("Name", definition.Name);
            Assert.AreEqual("Description", definition.Description);
            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual(1, definition.Condition.Count);
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual("(.*)", definition.Condition[0].Pattern);
            Assert.AreEqual("notmatchregex", definition.Condition[0].ComparisonMethod);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
        }

        [Test]
        public void IsValidWithActionWithInvalidId()
        {
            var definition = new RuleDefinition
                {
                    Name = "a",
                    Action = new ActionRuleDefinition
                        {
                            Id = "b",
                            Parameters = new Dictionary<string, object>(),
                        },
                    Enabled = true,
                    Signal = new SignalRuleDefinition
                        {
                            Id = "c",
                            Parameters = new Dictionary<string, object>(),
                        }
                };
            Assert.IsFalse(RuleLoader.IsValid(definition, s => false));
        }

        [Test]
        public void IsValidWithActionWithInvalidSignalParameterReference()
        {
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
            Assert.IsFalse(RuleLoader.IsValid(definition, s => true));
        }

        [Test]
        public void IsValidWithConditionWithIncorrectName()
        {
            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            definition.Condition.Add(
                new ConditionRuleDefinition
                {
                    Name = "b",
                    Pattern = "d",
                    ComparisonMethod = "equals"
                });
            Assert.IsFalse(RuleLoader.IsValid(definition, s => true));
        }

        [Test]
        public void IsValidWithConditionWithInvalidType()
        {
            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            definition.Condition.Add(
                new ConditionRuleDefinition
                {
                    Name = "c",
                    Pattern = "d",
                    ComparisonMethod = "operator"
                });
            Assert.IsFalse(RuleLoader.IsValid(definition, s => true));
        }

        [Test]
        public void IsValidWithMissingAction()
        {
            var definition = new RuleDefinition
            {
                Name = "a",
                Action = null,
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            Assert.IsFalse(RuleLoader.IsValid(definition, s => true));
        }

        [Test]
        public void IsValidWithMissingName()
        {
            var definition = new RuleDefinition
            {
                Name = string.Empty,
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Enabled = true,
                Signal = new SignalRuleDefinition
                {
                    Id = "c",
                    Parameters = new Dictionary<string, object>(),
                }
            };
            Assert.IsFalse(RuleLoader.IsValid(definition, s => true));
        }

        [Test]
        public void IsValidWithMissingSignal()
        {
            var definition = new RuleDefinition
            {
                Name = "a",
                Action = new ActionRuleDefinition
                {
                    Id = "b",
                    Parameters = new Dictionary<string, object>(),
                },
                Enabled = true,
                Signal = null
            };
            Assert.IsFalse(RuleLoader.IsValid(definition, s => true));
        }

        [Test]
        public void LoadWithActionWithoutReference()
        {
            var fileName = "ActionWithoutReference.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithConditionWithoutName()
        {
            var fileName = "ConditionWithoutName.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithConditionWithoutPattern()
        {
            var fileName = "ConditionWithoutPattern.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithConditionWithoutType()
        {
            var fileName = "ConditionWithoutType.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithEmptyFile()
        {
            var fileName = "EmptyFile.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithEmptyFilePath()
        {
            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(string.Empty);
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithMissingRuleEnabledSwitch()
        {
            var fileName = "MissingEnabledFlag.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithMissingRuleName()
        {
            var fileName = "MissingName.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithNonExistingFilePath()
        {
            var fileName = "MissingFile.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithNullFilePath()
        {
            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(null);
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithoutAction()
        {
            var fileName = "MissingAction.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithoutSignal()
        {
            var fileName = "MissingSignal.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadWithSignalWithoutId()
        {
            var fileName = "SignalWithoutId.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNull(rule);
        }

        [Test]
        public void LoadRuleWithoutCondition()
        {
            var fileName = "RuleWithoutCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = 10;
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithEndsWithCondition()
        {
            var fileName = "RuleWithEndsWithCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = "some_bar";
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithEqualsCondition()
        {
            var fileName = "RuleWithEqualsCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = 10;
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithGreaterThanCondition()
        {
            var fileName = "RuleWithGreaterThanCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = 10;
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithLessThanCondition()
        {
            var fileName = "RuleWithLessThanCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = 10;
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithMatchRegexCondition()
        {
            var fileName = "RuleWithMatchRegexCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = "bar_some";
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithMultipleConditionsOnSignalParametersIntoASingleActionParameterWithParametersMatchingConditions()
        {
            var fileName = "RuleWithConditionsOnSignalParameterIntoSingleActionParameter.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue1 = "stuff_bar";
            var parameterValue2 = "baz_somestuff";
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["bar"] = parameterValue1,
                    ["baz"] = parameterValue2,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("foo"));
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "{0} {1}", parameterValue1, parameterValue2), job.ParameterValue("foo"));
        }

        [Test]
        public void LoadRuleWithMultipleConditionsOnSignalParametersIntoASingleActionParameterWithParametersNotMatchingConditions()
        {
            var fileName = "RuleWithConditionsOnSignalParameterIntoSingleActionParameter.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue1 = "bar_stuff";
            var parameterValue2 = "somestuff_baz";
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["bar"] = parameterValue1,
                    ["baz"] = parameterValue2,
                });
            Assert.Throws<InvalidSignalForRuleException>(() => rule.ToJob(signal));
        }

        [Test]
        public void LoadRuleWithNotEqualsCondition()
        {
            var fileName = "RuleWithNotEqualsCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = 10;
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithNotMatchRegexCondition()
        {
            var fileName = "RuleWithNotMatchRegexCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = "some_stuff";
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }

        [Test]
        public void LoadRuleWithStartsWithCondition()
        {
            var fileName = "RuleWithStartsWithCondition.mmrule";

            var loader = new RuleLoader(
                s => true,
                new SystemDiagnostics((l, m) => { }, null));
            var rule = loader.Load(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(rule);

            var parameterValue = "bar_some";
            var signal = new Signal(
                new SignalTypeId("Signal"),
                new Dictionary<string, object>
                {
                    ["foo"] = parameterValue,
                });
            var job = rule.ToJob(signal);
            Assert.AreEqual(new ActionId("Action"), job.Action);
            Assert.AreEqual(1, job.ParameterNames().Count());
            Assert.IsTrue(job.ContainsParameter("bar"));
            Assert.AreEqual(parameterValue, job.ParameterValue("bar"));
        }
    }
}
