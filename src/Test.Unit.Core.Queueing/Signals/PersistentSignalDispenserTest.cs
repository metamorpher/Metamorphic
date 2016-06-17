//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using EasyNetQ;
using Metamorphic.Core;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Moq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NUnit.Framework;

namespace Test.Unit.Core.Queueing.Signals
{
    [TestFixture]
    public sealed class PersistentSignalDispenserTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Queueing.Signals.PersistentSignalDispenser",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithMissingBus()
        {
            var diag = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalDispenser(null, diag));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Core.Queueing.Signals.PersistentSignalDispenser",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithMissingDiagnostics()
        {
            var bus = new Mock<IBus>();
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalDispenser(bus.Object, null));
        }

        [Test]
        public void ProcessWithNullSignal()
        {
            string queueName = string.Empty;
            Func<SignalData, Task> processAction = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.Receive<SignalData>(It.IsAny<string>(), It.IsAny<Func<SignalData, Task>>()))
                    .Callback<string, Func<SignalData, Task>>(
                        (q, a) =>
                        {
                            queueName = q;
                            processAction = a;
                        });
            }

            var diag = new SystemDiagnostics((l, m) => { }, null);

            var publisher = new PersistentSignalDispenser(bus.Object, diag, new CurrentThreadTaskScheduler());

            Assert.IsNotNull(publisher);
            Assert.IsNotNull(processAction);
            Assert.DoesNotThrow(() => processAction(null).Wait());
        }

        [Test]
        public void Process()
        {
            string queueName = string.Empty;
            Func<SignalData, Task> processAction = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.Receive<SignalData>(It.IsAny<string>(), It.IsAny<Func<SignalData, Task>>()))
                    .Callback<string, Func<SignalData, Task>>(
                        (q, a) =>
                        {
                            queueName = q;
                            processAction = a;
                        });
            }

            var lastLevel = LevelToLog.None;
            var lastMessage = string.Empty;
            var diag = new SystemDiagnostics(
                (l, m) =>
                {
                    lastLevel = l;
                    lastMessage = m;
                },
                null);

            Signal createdSignal = null;
            EventHandler<ItemEventArgs<Signal>> handler =
                (o, e) =>
                {
                    createdSignal = e.Item;
                };

            var publisher = new PersistentSignalDispenser(bus.Object, diag, new CurrentThreadTaskScheduler());
            publisher.OnItemAvailable += handler;

            var typeId = "a";
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signalData = new SignalData
            {
                SensorId = typeId,
                Parameters = parameters,
            };
            processAction(signalData).Wait();

            Assert.IsNotNull(createdSignal);

            var obj = ((ITranslateToDataObject<SignalData>)createdSignal).ToDataObject();
            Assert.AreEqual(typeId, obj.SensorId);
            Assert.That(obj.Parameters, Is.EquivalentTo(parameters));

            Assert.AreEqual(LevelToLog.Debug, lastLevel);
            Assert.IsTrue(lastMessage.StartsWith("Processed", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ProcessWithFailure()
        {
            string queueName = string.Empty;
            Func<SignalData, Task> processAction = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.Receive<SignalData>(It.IsAny<string>(), It.IsAny<Func<SignalData, Task>>()))
                    .Callback<string, Func<SignalData, Task>>(
                        (q, a) =>
                        {
                            queueName = q;
                            processAction = a;
                        });
            }

            var lastLevel = LevelToLog.None;
            var lastMessage = string.Empty;
            var diag = new SystemDiagnostics(
                (l, m) =>
                {
                    lastLevel = l;
                    lastMessage = m;
                },
                null);

            Signal createdSignal = null;
            EventHandler<ItemEventArgs<Signal>> handler =
                (o, e) =>
                {
                    createdSignal = e.Item;
                    throw new NotImplementedException();
                };

            var publisher = new PersistentSignalDispenser(bus.Object, diag, new CurrentThreadTaskScheduler());
            publisher.OnItemAvailable += handler;

            var typeId = "a";
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signalData = new SignalData
            {
                SensorId = typeId,
                Parameters = parameters,
            };
            Assert.Throws<AggregateException>(() => processAction(signalData).Wait());

            Assert.IsNotNull(createdSignal);

            var obj = ((ITranslateToDataObject<SignalData>)createdSignal).ToDataObject();
            Assert.AreEqual(typeId, obj.SensorId);
            Assert.That(obj.Parameters, Is.EquivalentTo(parameters));

            Assert.AreEqual(LevelToLog.Warn, lastLevel);
            Assert.IsTrue(lastMessage.StartsWith("Failed", StringComparison.OrdinalIgnoreCase));
        }
    }
}
