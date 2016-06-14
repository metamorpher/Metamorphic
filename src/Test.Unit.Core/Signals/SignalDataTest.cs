//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Metamorphic.Core.Signals
{
    [TestFixture]
    public sealed class SignalDataTest : EqualityContractVerifierTest
    {
        private sealed class SignalDataEqualityContractVerifier : EqualityContractVerifier<SignalData>
        {
            private readonly SignalData m_First = new SignalData();

            private readonly SignalData m_Second = new SignalData(
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
                    return m_First;
                }
            }

            protected override SignalData SecondInstance
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

        private sealed class SignalDataHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SignalData> m_DistinctInstances
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
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SignalDataHashcodeContractVerfier m_HashcodeVerifier = new SignalDataHashcodeContractVerfier();

        private readonly SignalDataEqualityContractVerifier m_EqualityVerifier = new SignalDataEqualityContractVerifier();

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
    }
}
