//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Metamorphic.Storage.Rules
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
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

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
        }

        [Test]
        public void CreateDefinitionFromFileWithActionWithParametersReferencingMultipleSignalParameters()
        {
            var fileName = "ActionWithParametersReferencingMultipleSignalParameters.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

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
        }

        [Test]
        public void CreateDefinitionFromFileWithActionWithParametersReferencingSignal()
        {
            var fileName = "ActionWithParametersReferencingSignal.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

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
        }

        [Test]
        public void CreateDefinitionFromFileWithEndsWithCondition()
        {
            var fileName = "EndsWithCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual("a", definition.Condition[0].Pattern);
                Assert.AreEqual("endswith", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void CreateDefinitionFromFileWithEqualsCondition()
        {
            var fileName = "EqualsCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual("a", definition.Condition[0].Pattern);
                Assert.AreEqual("equals", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void CreateDefinitionFromFileWithGreaterThanCondition()
        {
            var fileName = "GreaterThanCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual(10, definition.Condition[0].Pattern);
                Assert.AreEqual("greaterthan", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void CreateDefinitionFromFileWithLessThanCondition()
        {
            var fileName = "LessThanCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual(10, definition.Condition[0].Pattern);
                Assert.AreEqual("lessthan", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void CreateDefinitionFromFileWithMatchRegexCondition()
        {
            var fileName = "MatchRegexCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual("(.*)", definition.Condition[0].Pattern);
                Assert.AreEqual("matchregex", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void CreateDefinitionFromFileWithNotEqualsCondition()
        {
            var fileName = "NotEqualsCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual("a", definition.Condition[0].Pattern);
                Assert.AreEqual("notequals", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void CreateDefinitionFromFileWithNotMatchRegexCondition()
        {
            var fileName = "NotMatchRegexCondition.mmrule";
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = RuleLoader.CreateDefinition(reader);

                Assert.AreEqual("Name", definition.Name);
                Assert.AreEqual("Description", definition.Description);
                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual(1, definition.Condition.Count);
                Assert.AreEqual("bar", definition.Condition[0].Name);
                Assert.AreEqual("(.*)", definition.Condition[0].Pattern);
                Assert.AreEqual("notmatchregex", definition.Condition[0].Type);

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count);
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
                Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count);
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
                Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            }
        }

        [Test]
        public void LoadFromFileWithActionWithoutReference()
        {
            var fileName = "ActionWithoutReference.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithConditionWithoutName()
        {
            var fileName = "ConditionWithoutName.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithConditionWithoutPattern()
        {
            var fileName = "ConditionWithoutPattern.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithConditionWithoutType()
        {
            var fileName = "ConditionWithoutType.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithEmptyFile()
        {
            var fileName = "EmptyFile.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithEmptyFilePath()
        {
            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            Assert.Throws<ArgumentException>(() => loader.LoadFromFile(string.Empty));
        }

        [Test]
        public void LoadFromFileWithMissingRuleEnabledSwitch()
        {
            var fileName = "MissingEnabledFlag.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsFalse(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(2, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Action.Parameters["foo"]);
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("baz"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["baz"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("equals", definition.Condition[0].Type);
            Assert.AreEqual("bar", definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileWithMissingRuleName()
        {
            var fileName = "MissingName.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithNonExistingFilePath()
        {
            var fileName = "MissingFile.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            Assert.Throws<FileNotFoundException>(() => loader.LoadFromFile(Path.Combine(RulePath(), fileName)));
        }

        [Test]
        public void LoadFromFileWithNullFilePath()
        {
            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            Assert.Throws<ArgumentNullException>(() => loader.LoadFromFile(null));
        }

        [Test]
        public void LoadFromFileWithoutAction()
        {
            var fileName = "MissingAction.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithoutSignal()
        {
            var fileName = "MissingSignal.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileWithSignalWithoutId()
        {
            var fileName = "SignalWithoutId.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNull(definition);
        }

        [Test]
        public void LoadFromFileRuleWithoutCondition()
        {
            var fileName = "RuleWithoutCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(0, definition.Condition.Count());

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual(1, definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithEndsWithCondition()
        {
            var fileName = "RuleWithEndsWithCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("endswith", definition.Condition[0].Type);
            Assert.AreEqual("bar", definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithEqualsCondition()
        {
            var fileName = "RuleWithEqualsCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("equals", definition.Condition[0].Type);
            Assert.AreEqual(10, definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual(1, definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithGreaterThanCondition()
        {
            var fileName = "RuleWithGreaterThanCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("greaterthan", definition.Condition[0].Type);
            Assert.AreEqual(5, definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual(1, definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithLessThanCondition()
        {
            var fileName = "RuleWithLessThanCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("lessthan", definition.Condition[0].Type);
            Assert.AreEqual(15, definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual(20, definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithMatchRegexCondition()
        {
            var fileName = "RuleWithMatchRegexCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("matchregex", definition.Condition[0].Type);
            Assert.AreEqual(".*(bar).*", definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual("some_stuff", definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithMultipleConditionsOnSignalParametersIntoASingleActionParameterWithParametersMatchingConditions()
        {
            var fileName = "RuleWithConditionsOnSignalParameterIntoSingleActionParameter.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("foo"));
            Assert.AreEqual("{{signal.bar}} {{signal.baz}}", definition.Action.Parameters["foo"]);

            Assert.AreEqual(2, definition.Condition.Count());
            Assert.AreEqual("bar", definition.Condition[0].Name);
            Assert.AreEqual("startswith", definition.Condition[0].Type);
            Assert.AreEqual("stuff", definition.Condition[0].Pattern);
            Assert.AreEqual("baz", definition.Condition[1].Name);
            Assert.AreEqual("endswith", definition.Condition[1].Type);
            Assert.AreEqual("stuff", definition.Condition[1].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(2, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("bar"));
            Assert.AreEqual("stuff", definition.Signal.Parameters["bar"]);
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("baz"));
            Assert.AreEqual("otherstuff", definition.Signal.Parameters["baz"]);
        }

        [Test]
        public void LoadFromFileRuleWithNotEqualsCondition()
        {
            var fileName = "RuleWithNotEqualsCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("notequals", definition.Condition[0].Type);
            Assert.AreEqual(1, definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual(1, definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithNotMatchRegexCondition()
        {
            var fileName = "RuleWithNotMatchRegexCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("notmatchregex", definition.Condition[0].Type);
            Assert.AreEqual(".*(bar).*", definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromFileRuleWithStartsWithCondition()
        {
            var fileName = "RuleWithStartsWithCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            var definition = loader.LoadFromFile(Path.Combine(RulePath(), fileName));
            Assert.IsNotNull(definition);

            Assert.IsTrue(definition.Enabled);

            Assert.AreEqual("Action", definition.Action.Id);
            Assert.AreEqual(1, definition.Action.Parameters.Count());
            Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
            Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

            Assert.AreEqual(1, definition.Condition.Count());
            Assert.AreEqual("foo", definition.Condition[0].Name);
            Assert.AreEqual("startswith", definition.Condition[0].Type);
            Assert.AreEqual("bar", definition.Condition[0].Pattern);

            Assert.AreEqual("Signal", definition.Signal.Id);
            Assert.AreEqual(1, definition.Signal.Parameters.Count());
            Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
            Assert.AreEqual("bar", definition.Signal.Parameters["foo"]);
        }

        [Test]
        public void LoadFromMemoryWithEmptyFile()
        {
            var fileName = "EmptyFile.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = loader.LoadFromMemory(reader.ReadToEnd());
                Assert.IsNull(definition);
            }
        }

        [Test]
        public void LoadFromMemoryWithEmptyString()
        {
            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            Assert.Throws<ArgumentException>(() => loader.LoadFromMemory(string.Empty));
        }

        [Test]
        public void LoadFromMemoryWithNullContent()
        {
            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            Assert.Throws<ArgumentNullException>(() => loader.LoadFromMemory(null));
        }

        [Test]
        public void LoadFromMemoryRuleWithoutCondition()
        {
            var fileName = "RuleWithoutCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            using (var reader = new StreamReader(Path.Combine(RulePath(), fileName)))
            {
                var definition = loader.LoadFromMemory(reader.ReadToEnd());
                Assert.IsNotNull(definition);

                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count());
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
                Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

                Assert.AreEqual(0, definition.Condition.Count());

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count());
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
                Assert.AreEqual(1, definition.Signal.Parameters["foo"]);
            }
        }

        [Test]
        public void LoadFromStreamWithEmptyStream()
        {
            var fileName = "EmptyFile.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            using (var stream = new FileStream(Path.Combine(RulePath(), fileName), FileMode.Open, FileAccess.Read))
            {
                var definition = loader.LoadFromStream(stream);
                Assert.IsNull(definition);
            }
        }

        [Test]
        public void LoadFromStreamWithNullStream()
        {
            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            Assert.Throws<ArgumentNullException>(() => loader.LoadFromStream(null));
        }

        [Test]
        public void LoadFromStreamRuleWithoutCondition()
        {
            var fileName = "RuleWithoutCondition.mmrule";

            var loader = new RuleLoader(new SystemDiagnostics((l, m) => { }, null));
            using (var stream = new FileStream(Path.Combine(RulePath(), fileName), FileMode.Open, FileAccess.Read))
            {
                var definition = loader.LoadFromStream(stream);
                Assert.IsNotNull(definition);

                Assert.IsTrue(definition.Enabled);

                Assert.AreEqual("Action", definition.Action.Id);
                Assert.AreEqual(1, definition.Action.Parameters.Count());
                Assert.IsTrue(definition.Action.Parameters.ContainsKey("bar"));
                Assert.AreEqual("{{signal.foo}}", definition.Action.Parameters["bar"]);

                Assert.AreEqual(0, definition.Condition.Count());

                Assert.AreEqual("Signal", definition.Signal.Id);
                Assert.AreEqual(1, definition.Signal.Parameters.Count());
                Assert.IsTrue(definition.Signal.Parameters.ContainsKey("foo"));
                Assert.AreEqual(1, definition.Signal.Parameters["foo"]);
            }
        }
    }
}
