//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalProcessor(null, diag));
        }

        [Test]
        public void CreateWithMissingDiagnostics()
        {
            var bus = new Mock<IBus>();
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalProcessor(bus.Object, null));
        }

        [Test]
        public void ProcessWithNullSignal()
        {
            var queueName = string.Empty;
            Action<SignalData> processAction = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.Receive<SignalData>(It.IsAny<string>(), It.IsAny<Action<SignalData>>()))
                    .Callback<string, Action<SignalData>>(
                        (q, a) => 
                        {
                            queueName = q;
                            processAction = a;
                        });
            }

            var diag = new SystemDiagnostics((l, m) => { }, null);

            var publisher = new PersistentSignalProcessor(bus.Object, diag);

            Assert.IsNotNull(processAction);
            Assert.DoesNotThrow(() => processAction(null));
        }

        [Test]
        public void Process()
        {
            string queueName = string.Empty;
            Action<SignalData> processAction = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.Receive<SignalData>(It.IsAny<string>(), It.IsAny<Action<SignalData>>()))
                    .Callback<string, Action<SignalData>>(
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

            var publisher = new PersistentSignalProcessor(bus.Object, diag);
            publisher.OnEnqueue += handler;

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
            processAction(signalData);

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
            Action<SignalData> processAction = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.Receive<SignalData>(It.IsAny<string>(), It.IsAny<Action<SignalData>>()))
                    .Callback<string, Action<SignalData>>(
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

            var publisher = new PersistentSignalProcessor(bus.Object, diag);
            publisher.OnEnqueue += handler;

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
            Assert.Throws<Exception>(() => processAction(signalData));

            Assert.IsNotNull(createdSignal);

            var obj = ((ITranslateToDataObject<SignalData>)createdSignal).ToDataObject();
            Assert.AreEqual(typeId, obj.SensorId);
            Assert.That(obj.Parameters, Is.EquivalentTo(parameters));

            Assert.AreEqual(LevelToLog.Warn, lastLevel);
            Assert.IsTrue(lastMessage.StartsWith("Failed"));
        }
    }
}
