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

namespace Metamorphic.Core.Actions
{
    [TestFixture]
    public sealed class ActionIdTest : EqualityContractVerifierTest
    {
        private sealed class ActionIdEqualityContractVerifier : EqualityContractVerifier<ActionId>
        {
            private readonly ActionId _first = new ActionId("a");

            private readonly ActionId _second = new ActionId("b");

            protected override ActionId Copy(ActionId original)
            {
                return original.Clone();
            }

            protected override ActionId FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ActionId SecondInstance
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

        private sealed class ActionIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ActionId> _distinctInstances
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
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ActionIdHashcodeContractVerfier _hashcodeVerifier = new ActionIdHashcodeContractVerfier();

        private readonly ActionIdEqualityContractVerifier _equalityVerifier = new ActionIdEqualityContractVerifier();

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
