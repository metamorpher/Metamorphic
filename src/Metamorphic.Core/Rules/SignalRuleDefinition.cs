//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores information about a trigger in a rule.
    /// </summary>
    public class SignalRuleDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRuleDefinition"/> class.
        /// </summary>
        public SignalRuleDefinition()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// The collection of parameters for the trigger.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of trigger.
        /// </summary>
        public string Id
        {
            get;
            set;
        }
    }
}