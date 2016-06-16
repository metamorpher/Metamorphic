//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Jobs;
using Metamorphic.Server.Rules;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Metamorphic.Server
{
    [TestFixture]
    public class SignalProcessorTest
    {
        [Test]
        public void CreateWithNullJobQueue()
        {
            var rules = new Mock<IStoreRules>();
            var signals = new Mock<IDispenseSignals>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            Assert.Throws<ArgumentNullException>(() => new SignalProcessor(null, rules.Object, signals.Object, diag));
        }

        [Test]
        public void CreateWithNullRuleCollection()
        {
            var jobs = new Mock<IQueueJobs>();
            var signals = new Mock<IDispenseSignals>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            Assert.Throws<ArgumentNullException>(() => new SignalProcessor(jobs.Object, null, signals.Object, diag));
        }

        [Test]
        public void CreateWithNullSignalDispenser()
        {
            var jobs = new Mock<IQueueJobs>();
            var rules = new Mock<IStoreRules>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            Assert.Throws<ArgumentNullException>(() => new SignalProcessor(jobs.Object, rules.Object, null, diag));
        }

        [Test]
        public void CreateWithNullDiagnostics()
        {
            var jobs = new Mock<IQueueJobs>();
            var rules = new Mock<IStoreRules>();
            var signals = new Mock<IDispenseSignals>();

            Assert.Throws<ArgumentNullException>(() => new SignalProcessor(jobs.Object, rules.Object, signals.Object, null));
        }

        [Test]
        public void DispenseWithNullSignal()
        {
            var jobs = new Mock<IQueueJobs>();
            {
                jobs.Setup(j => j.Enqueue(It.IsAny<Job>()))
                    .Verifiable();
            }

            var rules = new Mock<IStoreRules>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            var processor = new SignalProcessor(jobs.Object, rules.Object, signals.Object, diag);

            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(null));
            jobs.Verify(j => j.Enqueue(It.IsAny<Job>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Never());
        }

        [Test]
        public void DispenseWithSignalWithNoMatchingRule()
        {
            var jobs = new Mock<IQueueJobs>();
            {
                jobs.Setup(j => j.Enqueue(It.IsAny<Job>()))
                    .Verifiable();
            }

            var rules = new Mock<IStoreRules>();
            {
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(new Rule[0])
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            var processor = new SignalProcessor(jobs.Object, rules.Object, signals.Object, diag);

            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);

            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));
            jobs.Verify(j => j.Enqueue(It.IsAny<Job>()), Times.Never());
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
        }

        [Test]
        public void DispenseWithSignalWithSingleMatchingRule()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>();
            var signal = new Signal(type, parameters);

            var jobs = new Mock<IQueueJobs>();
            {
                jobs.Setup(j => j.Enqueue(It.IsAny<Job>()))
                    .Verifiable();
            }

            var rules = new Mock<IStoreRules>();
            {
                var conditions = new Dictionary<string, Predicate<object>>();
                var ruleParameters = new Dictionary<string, ActionParameterValue>();
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new Rule[]
                        {
                            new Rule("b", "c", type, new ActionId("d"), conditions, ruleParameters),
                        })
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            var processor = new SignalProcessor(jobs.Object, rules.Object, signals.Object, diag);

            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));
            jobs.Verify(j => j.Enqueue(It.IsAny<Job>()), Times.Exactly(1));
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
        }

        [Test]
        public void DispenseWithSignalWithMultipleMatchingRules()
        {
            var type = new SignalTypeId("a");
            var parameters = new Dictionary<string, object>();
            var signal = new Signal(type, parameters);

            var jobs = new Mock<IQueueJobs>();
            {
                jobs.Setup(j => j.Enqueue(It.IsAny<Job>()))
                    .Verifiable();
            }

            var rules = new Mock<IStoreRules>();
            {
                var conditions = new Dictionary<string, Predicate<object>>();
                var ruleParameters = new Dictionary<string, ActionParameterValue>();
                rules.Setup(r => r.RulesForSignal(It.IsAny<SignalTypeId>()))
                    .Returns(
                        new Rule[]
                        {
                            new Rule("b", "c", type, new ActionId("d"), conditions, ruleParameters),
                            new Rule("e", "f", type, new ActionId("g"), conditions, ruleParameters)
                        })
                    .Verifiable();
            }

            var signals = new Mock<IDispenseSignals>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            var processor = new SignalProcessor(jobs.Object, rules.Object, signals.Object, diag);

            signals.Raise(s => s.OnItemAvailable += null, new ItemEventArgs<Signal>(signal));
            jobs.Verify(j => j.Enqueue(It.IsAny<Job>()), Times.Exactly(2));
            rules.Verify(r => r.RulesForSignal(It.IsAny<SignalTypeId>()), Times.Once());
        }
    }
}
