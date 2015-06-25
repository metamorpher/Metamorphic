//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Nunit.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metamorphic.Core.Sensors
{
    [TestFixture]
    public sealed class SensorIdTest : EqualityContractVerifierTest
    {
        private sealed class SensorIdEqualityContractVerifier : EqualityContractVerifier<SensorId>
        {
            private readonly SensorId m_First = new SensorId("a");

            private readonly SensorId m_Second = new SensorId("b");

            protected override SensorId Copy(SensorId original)
            {
                return original.Clone();
            }

            protected override SensorId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SensorId SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class SensorIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SensorId> m_DistinctInstances
                = new List<SensorId>
                     {
                        new SensorId("a"),
                        new SensorId("b"),
                        new SensorId("c"),
                        new SensorId("d"),
                        new SensorId("e"),
                        new SensorId("f"),
                        new SensorId("g"),
                        new SensorId("h"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SensorIdHashcodeContractVerfier m_HashcodeVerifier = new SensorIdHashcodeContractVerfier();

        private readonly SensorIdEqualityContractVerifier m_EqualityVerifier = new SensorIdEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            SensorId first = null;
            var second = new SensorId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new SensorId("a");
            SensorId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            SensorId first = null;
            SensorId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new SensorId("a");
            var second = new SensorId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new SensorId("b");
            var second = new SensorId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new SensorId("a");
            var second = new SensorId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            SensorId first = null;
            var second = new SensorId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new SensorId("a");
            SensorId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            SensorId first = null;
            SensorId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new SensorId("a");
            var second = new SensorId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new SensorId("b");
            var second = new SensorId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new SensorId("a");
            var second = new SensorId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new SensorId("a");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new SensorId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new SensorId("a");
            object second = new SensorId("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new SensorId("b");
            object second = new SensorId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new SensorId("a");
            object second = new SensorId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new SensorId("a");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
