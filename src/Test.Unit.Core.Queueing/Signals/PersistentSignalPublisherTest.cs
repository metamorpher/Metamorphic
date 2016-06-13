using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using EasyNetQ;
using Metamorphic.Core.Queueing;
using Metamorphic.Core.Queueing.Signals;
using Metamorphic.Core.Signals;
using Moq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NUnit.Framework;

namespace Test.Unit.Core.Queueing
{
    [TestFixture]
    public class PersistentSignalPublisherTest
    {
        [Test]
        public void CreateWithMissingBus()
        {
            var diag = new SystemDiagnostics((l, m) => { }, null);
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalPublisher(null, diag));
        }

        [Test]
        public void CreateWithMissingDiagnostics()
        {
            var bus = new Mock<IBus>();
            Assert.Throws<ArgumentNullException>(() => new PersistentSignalPublisher(bus.Object, null));
        }

        [Test]
        public void PublishWithNullSignal()
        {
            var bus = new Mock<IBus>();
            var diag = new SystemDiagnostics((l, m) => { }, null);

            var publisher = new PersistentSignalPublisher(bus.Object, diag);
            Assert.Throws<ArgumentNullException>(() => publisher.Publish(null));
        }

        [Test]
        public void PublishWithSignal()
        {
            string queueName = string.Empty;
            SignalData publishedSignalData = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.SendAsync(It.IsAny<string>(), It.IsAny<SignalData>()))
                    .Callback<string, SignalData>(
                        (t, s) => 
                        {
                            queueName = t;
                            publishedSignalData = s;
                        })
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
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

            var publisher = new PersistentSignalPublisher(bus.Object, diag);

            var typeId = "a";
            var type = new SignalTypeId(typeId);
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);
            publisher.Publish(signal);

            bus.Verify(b => b.SendAsync(It.IsAny<string>(), It.IsAny<SignalData>()), Times.Once());

            Assert.AreEqual(StoreNames.Signal, queueName);
            Assert.AreEqual(typeId, publishedSignalData.SensorId);
            Assert.That(publishedSignalData.Parameters, Is.EquivalentTo(parameters));

            Assert.AreEqual(LevelToLog.Debug, lastLevel);
            Assert.IsTrue(lastMessage.StartsWith("Published"));
        }

        [Test]
        public void PublishWithSignalAndFailedPublish()
        {
            string queueName = string.Empty;
            SignalData publishedSignalData = null;
            var bus = new Mock<IBus>();
            {
                bus.Setup(b => b.SendAsync(It.IsAny<string>(), It.IsAny<SignalData>()))
                    .Callback<string, SignalData>(
                        (t, s) =>
                        {
                            queueName = t;
                            publishedSignalData = s;
                        })
                    .Returns(Task.Factory.StartNew(
                        () => { throw new Exception(); },
                        new CancellationTokenSource().Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
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

            var publisher = new PersistentSignalPublisher(bus.Object, diag);

            var typeId = "a";
            var type = new SignalTypeId(typeId);
            var parameters = new Dictionary<string, object>
                {
                    { "a", "b" }
                };
            var signal = new Signal(type, parameters);
            publisher.Publish(signal);

            bus.Verify(b => b.SendAsync(It.IsAny<string>(), It.IsAny<SignalData>()), Times.Once());

            Assert.AreEqual(StoreNames.Signal, queueName);
            Assert.AreEqual(typeId, publishedSignalData.SensorId);
            Assert.That(publishedSignalData.Parameters, Is.EquivalentTo(parameters));

            Assert.AreEqual(LevelToLog.Warn, lastLevel);
            Assert.IsTrue(lastMessage.StartsWith("Failed"));
        }
    }
}
