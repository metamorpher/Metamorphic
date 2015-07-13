using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metamorphic.Core.Signals;
using Moq;
using NUnit.Framework;

namespace Metamorphic.Server.Signals
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class SignalGeneratorStorageTest
    {
        [Test]
        public void Add()
        {
            var storage = new SignalGeneratorStorage();

            var id = new SignalTypeId("a");
            var generator = new Mock<IGenerateSignals>();
            {
                generator.Setup(g => g.Id)
                    .Returns(id);
            }
            storage.Add(generator.Object);

            Assert.IsTrue(storage.HasGeneratorFor(generator.Object.Id));
            Assert.AreSame(generator.Object, storage.Generator(generator.Object.Id));
        }

        [Test]
        public void AddWithDuplicateGenerator()
        {
            var storage = new SignalGeneratorStorage();

            var id = new ActionId("a");
            Action action = () => { };
            var definition = new ActionDefinition(
                id,
                new ActionParameterDefinition[0],
                action);
            storage.Add(definition);

            Assert.IsTrue(storage.HasActionFor(definition.Id));
            Assert.AreSame(definition, storage.Action(definition.Id));

            var otherDefinition = new ActionDefinition(
                id,
                new ActionParameterDefinition[0],
                action);
            Assert.Throws<DuplicateActionDefinitionException>(() => storage.Add(definition));
        }

        [Test]
        public void AddWithNullGenerator()
        {
            var storage = new SignalGeneratorStorage();
            Assert.Throws<ArgumentNullException>(() => storage.Add(null));
        }

        [Test]
        public void HasGeneratorForWithNullId()
        {
            var storage = new SignalGeneratorStorage();
            Assert.IsFalse(storage.HasGeneratorFor(null));
        }

        [Test]
        public void HasGeneratorForWithUnknownId()
        {
            var storage = new SignalGeneratorStorage();
            Assert.IsFalse(storage.HasGeneratorFor(new SignalTypeId("a")));
        }
    }
}
