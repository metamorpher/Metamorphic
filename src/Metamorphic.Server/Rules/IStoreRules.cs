//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Metamorphic.Core.Rules;

namespace Metamorphic.Server.Rules
{
    internal interface IStoreRules
    {
        IEnumerable<Rule> RulesForSignal(string signalType);
    }
}