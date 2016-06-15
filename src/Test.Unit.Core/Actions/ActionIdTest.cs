//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Nunit.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metamorphic.Core.Actions
{
    [TestFixture]
    public sealed class ActionIdTest : EqualityContractVerifierTest
    {
        private sealed class ActionIdEqualityContractVerifier : EqualityContractVerifier<ActionId>
        {
            private readonly ActionId m_First = new ActionId("a");

            private readonly ActionId m_Second = new ActionId("b");

            protected override ActionId Copy(ActionId original)
            {
                return original.Clone();
            }

            protected override ActionId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ActionId SecondInstance
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

        private sealed class ActionIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ActionId> m_DistinctInstances
                = new List<ActionId>
                     {
                        new ActionId("a"),
                        new ActionId("b"),
                        new ActionId("c"),
                        new ActionId("d"),
                        new ActionId("e"),
                        new ActionId("f"),
                        new ActionId("g"),
                        new ActionId("h"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ActionIdHashcodeContractVerfier m_HashcodeVerifier = new ActionIdHashcodeContractVerfier();

        private readonly ActionIdEqualityContractVerifier m_EqualityVerifier = new ActionIdEqualityContractVerifier();

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
            ActionId first = null;
            var second = new ActionId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new ActionId("a");
            ActionId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ActionId first = null;
            ActionId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ActionId("a");
            var second = new ActionId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ActionId("b");
            var second = new ActionId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ActionId("a");
            var second = new ActionId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ActionId first = null;
            var second = new ActionId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new ActionId("a");
            ActionId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ActionId first = null;
            ActionId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ActionId("a");
            var second = new ActionId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ActionId("b");
            var second = new ActionId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ActionId("a");
            var second = new ActionId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new ActionId("a");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new ActionId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ActionId("a");
            object second = new ActionId("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ActionId("b");
            object second = new ActionId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ActionId("a");
            object second = new ActionId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new ActionId("a");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
