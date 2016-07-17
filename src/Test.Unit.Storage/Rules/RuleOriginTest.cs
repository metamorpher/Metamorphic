//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Metamorphic.Storage.Rules;
using Nuclei.Nunit.Extensions;
using NuGet;
using NUnit.Framework;

namespace Test.Unit.Storage.Rules
{
    [TestFixture]
    public sealed class RuleOriginTest : EqualityContractVerifierTest
    {
        private sealed class RuleOriginEqualityContractVerifier : EqualityContractVerifier<RuleOrigin>
        {
            private readonly RuleOrigin _first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            private readonly RuleOrigin _second = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));

            protected override RuleOrigin Copy(RuleOrigin original)
            {
                return original.Clone();
            }

            protected override RuleOrigin FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override RuleOrigin SecondInstance
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

        private sealed class RuleOriginHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<RuleOrigin> _distinctInstances
                = new List<RuleOrigin>
                     {
                        new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0"))),
                        new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0"))),
                        new RuleOrigin(new PackageName("c", new SemanticVersion("1.0.0"))),
                        new RuleOrigin(new PackageName("d", new SemanticVersion("1.0.0"))),
                        new RuleOrigin(new PackageName("a", new SemanticVersion("1.1.0"))),
                        new RuleOrigin(new PackageName("b", new SemanticVersion("1.1.0"))),
                        new RuleOrigin(new PackageName("c", new SemanticVersion("1.1.0"))),
                        new RuleOrigin(new PackageName("d", new SemanticVersion("1.1.0"))),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly RuleOriginHashcodeContractVerfier _hashcodeVerifier = new RuleOriginHashcodeContractVerfier();

        private readonly RuleOriginEqualityContractVerifier _equalityVerifier = new RuleOriginEqualityContractVerifier();

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
            RuleOrigin first = null;
            var second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            RuleOrigin second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            RuleOrigin first = null;
            RuleOrigin second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            var second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));
            var second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            var second = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            RuleOrigin first = null;
            var second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            RuleOrigin second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            RuleOrigin first = null;
            RuleOrigin second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            var second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));
            var second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            var second = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            object second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));
            object second = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            object second = new RuleOrigin(new PackageName("b", new SemanticVersion("1.0.0")));

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new RuleOrigin(new PackageName("a", new SemanticVersion("1.0.0")));
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
