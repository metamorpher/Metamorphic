//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using NuGet;
using NUnit.Framework;

namespace Metamorphic.Storage.Rules
{
    [TestFixture]
    public sealed class RuleCollectionTest
    {
        [Test]
        public void Add()
        {
            var collection = new RuleCollection();

            var ruleDefinition = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0"))), ruleDefinition);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());
        }

        [Test]
        public void AddWithExistingOrigin()
        {
            var collection = new RuleCollection();

            var packageName1 = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition1 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName1), ruleDefinition1);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());

            var packageName2 = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition2 = new RuleDefinition
                {
                    Name = "c",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName2), ruleDefinition2);

            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(2, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());
            Assert.AreSame(ruleDefinition2, matchingRules.Last());
        }

        [Test]
        public void AddWithExistingSensorId()
        {
            var collection = new RuleCollection();

            var packageName1 = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition1 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName1), ruleDefinition1);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());

            var packageName2 = new PackageName("d", new SemanticVersion("1.0.0"));
            var ruleDefinition2 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName2), ruleDefinition2);

            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.That(matchingRules, Is.EquivalentTo(new[] { ruleDefinition1, ruleDefinition2 }));
        }

        [Test]
        public void AddWithNullOrigin()
        {
            var collection = new RuleCollection();

            var rule = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            Assert.Throws<ArgumentNullException>(() => collection.Add(null, rule));
        }

        [Test]
        public void AddWithNullRule()
        {
            var collection = new RuleCollection();

            Assert.Throws<ArgumentNullException>(() => collection.Add(new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0"))), null));
        }

        [Test]
        public void Remove()
        {
            var collection = new RuleCollection();

            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName), ruleDefinition);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());

            collection.Remove(new RuleOrigin(packageName));
            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(0, matchingRules.Count());
        }

        [Test]
        public void RemoveWithMultipleRulesPresent()
        {
            var collection = new RuleCollection();

            var packageName1 = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition1 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName1), ruleDefinition1);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());

            var packageName2 = new PackageName("d", new SemanticVersion("1.0.0"));
            var ruleDefinition2 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName2), ruleDefinition2);

            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition2.Signal.Id));
            Assert.That(matchingRules, Is.EquivalentTo(new[] { ruleDefinition1, ruleDefinition2 }));

            collection.Remove(new RuleOrigin(packageName1));

            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition2.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition2, matchingRules.First());
        }

        [Test]
        public void RemoveWithNonExistingOrigin()
        {
            var collection = new RuleCollection();

            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName), ruleDefinition);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());

            collection.Remove(new RuleOrigin(new PackageName("d", new SemanticVersion("1.0.0"))));
            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());
        }

        [Test]
        public void RemoveWithNullOrigin()
        {
            var collection = new RuleCollection();

            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName), ruleDefinition);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());

            collection.Remove(null);
            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());
        }

        [Test]
        public void Update()
        {
            var collection = new RuleCollection();

            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition1 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName), ruleDefinition1);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());

            var ruleDefinition2 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Update(new RuleOrigin(packageName), ruleDefinition2);

            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition2, matchingRules.First());
        }

        [Test]
        public void UpdateWithNonExistingPackageName()
        {
            var collection = new RuleCollection();

            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition1 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName), ruleDefinition1);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());

            var ruleDefinition2 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Update(new RuleOrigin(new PackageName("d", new SemanticVersion("1.0.0"))), ruleDefinition2);

            matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());
        }

        [Test]
        public void UpdateWithNullPackageName()
        {
            var collection = new RuleCollection();

            var packageName = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition1 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(packageName), ruleDefinition1);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition1.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition1, matchingRules.First());

            var ruleDefinition2 = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, ruleDefinition2));
        }

        [Test]
        public void UpdateWithNullRuleDefinition()
        {
            var collection = new RuleCollection();

            var package = new PackageName("a", new SemanticVersion("1.0.0"));
            var ruleDefinition = new RuleDefinition
                {
                    Name = "b",
                    Action = new ActionRuleDefinition
                    {
                        Id = "c",
                        Parameters = new Dictionary<string, object>
                        {
                            ["a"] = "{{signal.a}}",
                            ["c"] = "{{signal.c}}",
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
            collection.Add(new RuleOrigin(package), ruleDefinition);

            var matchingRules = collection.RulesForSignal(new SignalTypeId(ruleDefinition.Signal.Id));
            Assert.AreEqual(1, matchingRules.Count());
            Assert.AreSame(ruleDefinition, matchingRules.First());

            Assert.Throws<ArgumentNullException>(() => collection.Update(new RuleOrigin(package), null));
        }
    }
}
