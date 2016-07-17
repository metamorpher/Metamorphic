//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Rules;
using NUnit.Framework;

namespace Metamorphic.Storage.Rules
{
    [TestFixture]
    public sealed class RuleDefinitionCheckTest
    {
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

            var check = new RuleDefinitionCheck(definition);
            Assert.IsFalse(check.IsValid);
            Assert.AreEqual(1, check.Errors().Count);
        }

        [Test]
        public void IsValidWithConditionWithIncorrectConditionName()
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
                    Type = "equals"
                });

            var check = new RuleDefinitionCheck(definition);
            Assert.IsFalse(check.IsValid);
            Assert.AreEqual(1, check.Errors().Count);
        }

        [Test]
        public void IsValidWithConditionWithInvalidConditionType()
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
                    Parameters = new Dictionary<string, object>
                    {
                        ["d"] = "e",
                    },
                }
            };
            definition.Condition.Add(
                new ConditionRuleDefinition
                {
                    Name = "d",
                    Pattern = "f",
                    Type = "operator"
                });

            var check = new RuleDefinitionCheck(definition);
            Assert.IsFalse(check.IsValid);
            Assert.AreEqual(1, check.Errors().Count);
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

            var check = new RuleDefinitionCheck(definition);
            Assert.IsFalse(check.IsValid);
            Assert.AreEqual(1, check.Errors().Count);
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

            var check = new RuleDefinitionCheck(definition);
            Assert.IsFalse(check.IsValid);
            Assert.AreEqual(1, check.Errors().Count);
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

            var check = new RuleDefinitionCheck(definition);
            Assert.IsFalse(check.IsValid);
            Assert.AreEqual(1, check.Errors().Count);
        }
    }
}
