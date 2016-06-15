//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
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
    public sealed class PersistentSignalProcessorTest
    {
        [Test]
        public void CreateWithMissingBus()
        {
            var diag = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalDispenser(null, diag));
        }

        [Test]
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
            var type = new SignalTypeId(typeId);
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
            Assert.IsTrue(lastMessage.StartsWith("Processed"));
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
                    throw new Exception();
                };

            var publisher = new PersistentSignalDispenser(bus.Object, diag, new CurrentThreadTaskScheduler());
            publisher.OnItemAvailable += handler;

            var typeId = "a";
            var type = new SignalTypeId(typeId);
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
            Assert.IsTrue(lastMessage.StartsWith("Failed"));
        }
    }
}
