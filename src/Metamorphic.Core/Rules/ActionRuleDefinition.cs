//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores the information about an action reference and the parameter necessary for the given action.
    /// </summary>
    public class ActionRuleDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRuleDefinition"/> class.
        /// </summary>
        public ActionRuleDefinition()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the ID of the action that should be executed.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameters for the action.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }
}