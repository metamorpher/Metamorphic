//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Metamorphic.Core.Signals
{
    [TestFixture]
    public sealed class SensorIdTest : EqualityContractVerifierTest
    {
        private sealed class SensorIdEqualityContractVerifier : EqualityContractVerifier<SignalTypeId>
        {
            private readonly SignalTypeId _first = new SignalTypeId("a");

            private readonly SignalTypeId _second = new SignalTypeId("b");

            protected override SignalTypeId Copy(SignalTypeId original)
            {
                return original.Clone();
            }

            protected override SignalTypeId FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override SignalTypeId SecondInstance
            {
                get
                {
                    return _second;
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
            private readonly IEnumerable<SignalTypeId> _distinctInstances
                = new List<SignalTypeId>
                     {
                        new SignalTypeId("a"),
                        new SignalTypeId("b"),
                        new SignalTypeId("c"),
                        new SignalTypeId("d"),
                        new SignalTypeId("e"),
                        new SignalTypeId("f"),
                        new SignalTypeId("g"),
                        new SignalTypeId("h"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SensorIdHashcodeContractVerfier _hashcodeVerifier = new SensorIdHashcodeContractVerfier();

        private readonly SensorIdEqualityContractVerifier _equalityVerifier = new SensorIdEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return _hashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return _equalityVerifier;
            }
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            SignalTypeId first = null;
            var second = new SignalTypeId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new SignalTypeId("a");
            SignalTypeId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            SignalTypeId first = null;
            SignalTypeId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new SignalTypeId("a");
            var second = new SignalTypeId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new SignalTypeId("b");
            var second = new SignalTypeId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new SignalTypeId("a");
            var second = new SignalTypeId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            SignalTypeId first = null;
            var second = new SignalTypeId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new SignalTypeId("a");
            SignalTypeId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            SignalTypeId first = null;
            SignalTypeId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new SignalTypeId("a");
            var second = new SignalTypeId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new SignalTypeId("b");
            var second = new SignalTypeId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new SignalTypeId("a");
            var second = new SignalTypeId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new SignalTypeId("a");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new SignalTypeId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new SignalTypeId("a");
            object second = new SignalTypeId("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new SignalTypeId("b");
            object second = new SignalTypeId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new SignalTypeId("a");
            object second = new SignalTypeId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new SignalTypeId("a");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
