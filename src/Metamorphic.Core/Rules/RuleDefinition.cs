//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores information that describes a <see cref="Rule"/>.
    /// </summary>
    public class RuleDefinition
    {
        /// <summary>
        /// Gets or sets the action for the rule.
        /// </summary>
        public ActionRuleDefinition Action
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the criteria that should applied to the trigger.
        /// </summary>
        public List<ConditionRuleDefinition> Criteria
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description for the rule.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rule is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the rule.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the signal definition for the rule.
        /// </summary>
        public TriggerRuleDefinition Trigger
        {
            get;
            set;
        }
    }
}