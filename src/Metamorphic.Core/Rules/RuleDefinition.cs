//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Metamorphic.Core.Rules;

namespace Metamorphic.Server.Rules
{
    internal class RuleDefinition
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public TriggerDefinition Trigger { get; set; }

        public List<CriteriaDefinition> Criteria { get; set; }

        public ActionDefinition Action { get; set; }

        internal Rule ToRule()
        {
            throw new NotImplementedException();
        }
    }
}