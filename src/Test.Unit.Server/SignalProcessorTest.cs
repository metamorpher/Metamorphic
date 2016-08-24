//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using Moq;
using Nuclei.Diagnostics;
using NuGet;
using NUnit.Framework;
using Test.SourceOnly;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Server
{
    [TestFixture]
    public sealed class SignalProcessorTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullActionStorage()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    null,
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    new Mock<IRuleStorageProxy>().Object,
                    new Mock<IDispenseSignals>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullAppDomainBuilder()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    new Mock<IInstallPackages>().Object,
                    null,
                    a => null,
                    new Mock<IRuleStorageProxy>().Object,
                    new Mock<IDispenseSignals>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    new Mock<IRuleStorageProxy>().Object,
                    new Mock<IDispenseSignals>().Object,
                    null,
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullExecutorBuilder()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    null,
                    new Mock<IRuleStorageProxy>().Object,
                    new Mock<IDispenseSignals>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    new Mock<IRuleStorageProxy>().Object,
                    new Mock<IDispenseSignals>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullPackageInstaller()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    null,
                    (name, paths) => null,
                    a => null,
                    new Mock<IRuleStorageProxy>().Object,
                    new Mock<IDispenseSignals>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullRuleCollection()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    null,
                    new Mock<IDispenseSignals>().Object,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Server.SignalProcessor",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullSignalDispenser()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SignalProcessor(
                    new Mock<IActionStorageProxy>().Object,
                    new Mock<IInstallPackages>().Object,
                    (name, paths) => null,
                    a => null,
                    new Mock<IRuleStorageProxy>().Object,
                    null,
                    new SystemDiagnostics((l, m) => { }, null),
                    new Mock<IFileSystem>().Object));
        }

        [Test]
        public void ProcessWithNullSignal()
        {
            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => null,
                a => null,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(null));
            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithNoMatchingRule()
        {
            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(new RuleDefinition[0])
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => null,
                a => null,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);

            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSingleMatchingRule()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" },
                    { "c", "d" },
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("a"),
                                new ActionParameterDefinition("c"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
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
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["e"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingEndsWithCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "foo_bar" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "endswith"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingEqualsWithCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "bar" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "equals"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingGreaterThanCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", 15 }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = 10,
                                                Type = "greaterthan"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingLessThanCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", 5 }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = 10,
                                                Type = "lessthan"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingMatchRegexCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "foo_bar_baz" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = ".*(bar).*",
                                                Type = "matchregex"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingNotEqualsCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "bar" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "notequals"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingNotMatchRegexCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "stuff_bar_stuff" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = ".*(bar).*",
                                                Type = "notmatchregex"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterMatchingStartsWithCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "bar_foo" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "startswith"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingEndsWithCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "bar_foo" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "endswith"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingEqualsWithCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "baz" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "equals"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingGreaterThanCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", 5 }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = 10,
                                                Type = "greaterthan"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingLessThanCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", 15 }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = 10,
                                                Type = "lessthan"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingMatchRegexCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "foo_baz" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = ".*(bar).*",
                                                Type = "matchregex"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingNotEqualsCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "foo" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "notequals"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingNotMatchRegexCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "stuff_stuff" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = ".*(bar).*",
                                                Type = "notmatchregex"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }

        [Test]
        public void ProcessWithSignalWithSignalParameterNotMatchingStartsWithCondition()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "foo_bar" }
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}}",
                                            },
                                        },
                                    Condition = new List<ConditionRuleDefinition>
                                        {
                                            new ConditionRuleDefinition
                                            {
                                                Name = "signal_a",
                                                Pattern = "bar",
                                                Type = "startswith"
                                            }
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Never());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Never());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Never());
        }

        [Test]
        public void ProcessWithSignalWithSignalParametersIntoSingleActionParameter()
        {
            var type = new SignalTypeId("a");

            var actionId = new ActionId("powershell");
            var parameters = new Dictionary<string, object>
                {
                    { "signal_a", "b" },
                    { "signal_c", "d" },
                };
            var signal = new Signal(type, parameters);

            var actionStorage = new Mock<IActionStorageProxy>();
            {
                actionStorage.Setup(a => a.Action(It.IsAny<ActionId>()))
                    .Returns(
                         new ActionDefinition(
                            actionId,
                            "b",
                            "1.0.0",
                            "c",
                            "d",
                            new ActionParameterDefinition[]
                            {
                                new ActionParameterDefinition("action_a"),
                            }))
                    .Verifiable();
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()))
                    .Verifiable();
            }

            var rules = new Mock<IRuleStorageProxy>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new RuleDefinition[]
                        {
                            new RuleDefinition
                                {
                                    Name = "b",
                                    Description = "description",
                                    Action = new ActionRuleDefinition
                                        {
                                            Id = "c",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["action_a"] = "{{signal.signal_a}} {{signal.signal_c}}",
                                            },
                                        },
                                    Enabled = true,
                                    Signal = new SignalRuleDefinition
                                        {
                                            Id = "a",
                                            Parameters = new Dictionary<string, object>
                                            {
                                                ["signal_a"] = "f",
                                                ["signal_c"] = "g"
                                            },
                                        }
                                },
                        })
                    .Verifiable();
            }

            var executor = new Mock<IExecuteActions>();
            {
                executor.Setup(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()))
                    .Callback<Job, ActionDefinition>(
                        (j, a) =>
                        {
                            Assert.AreEqual(actionId, j.Action);
                            Assert.AreEqual(actionId, a.Id);
                        })
                    .Verifiable();
            }

            var loader = new Mock<ILoadActionExecutorsInRemoteAppDomains>();
            {
                loader.Setup(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()))
                    .Returns(executor.Object)
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var processor = new SignalProcessor(
                actionStorage.Object,
                packageInstaller.Object,
                (name, paths) => AppDomain.CurrentDomain,
                a => loader.Object,
                rules.Object,
                signals.Object,
                new SystemDiagnostics((l, m) => { }, null),
                fileSystem.Object);

            Assert.IsNotNull(processor);
            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));

            actionStorage.Verify(a => a.Action(It.IsAny<ActionId>()), Times.Once());
            packageInstaller.Verify(p => p.Install(It.IsAny<PackageName>(), It.IsAny<string>(), It.IsAny<Action<string, string, PackageName>>()), Times.Once());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
            loader.Verify(l => l.Load(It.IsAny<ILogMessagesFromRemoteAppDomains>()), Times.Once());
            executor.Verify(e => e.Execute(It.IsAny<Job>(), It.IsAny<ActionDefinition>()), Times.Once());
        }
    }
}
