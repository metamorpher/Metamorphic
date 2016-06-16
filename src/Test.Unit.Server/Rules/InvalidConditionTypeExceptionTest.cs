//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Metamorphic.Server.Rules
{
    [TestFixture]
    public sealed class InvalidConditionTypeExceptionTest : ExceptionContractVerifier<InvalidConditionTypeException>
    {
    }
}
