//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Metamorphic.Core.Signals
{
    [TestFixture]
    public sealed class SignalDataTest : EqualityContractVerifierTest
    {
        private sealed class SignalDataEqualityContractVerifier : EqualityContractVerifier<SignalData>
        {
            private readonly SignalData _first = new SignalData();

            private readonly SignalData _second = new SignalData(
                "b",
                new Dictionary<string, object>
                {
                    { "a", 1 },
                    { "b", 2 },
                });

            protected override SignalData Copy(SignalData original)
            {
                return new SignalData(original.SensorId, original.Parameters);
            }

            protected override SignalData FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override SignalData SecondInstance
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

        private sealed class SignalDataHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SignalData> _distinctInstances
                = new List<SignalData>
                     {
                        new SignalData(),

                        new SignalData("a"),
                        new SignalData("b"),
                        new SignalData("c"),
                        new SignalData("d"),

                        new SignalData(
                            "e",
                            new Dictionary<string, object>
                            {
                                { "ea", 1 },
                                { "eb", 2 },
                            }),
                        new SignalData(
                            "f",
                            new Dictionary<string, object>
                            {
                                { "fa", 1 },
                                { "fb", 2 },
                            }),
                        new SignalData(
                            "g",
                            new Dictionary<string, object>
                            {
                                { "ga", 1 },
                                { "gb", 2 },
                            }),
                        new SignalData(
                            "h",
                            new Dictionary<string, object>
                            {
                                { "ha", 1 },
                                { "hb", 2 },
                            }),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SignalDataHashcodeContractVerfier _hashcodeVerifier = new SignalDataHashcodeContractVerfier();

        private readonly SignalDataEqualityContractVerifier _equalityVerifier = new SignalDataEqualityContractVerifier();

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
    }
}
