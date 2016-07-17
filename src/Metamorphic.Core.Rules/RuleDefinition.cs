//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores information that describes a rule.
    /// </summary>
    /// <remarks>
    /// The naming of the members of this class is linked to the contents of the rule files.
    /// </remarks>
    [Serializable]
    public sealed class RuleDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDefinition"/> class.
        /// </summary>
        public RuleDefinition()
        {
            Condition = new List<ConditionRuleDefinition>();
        }

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
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This class is instantiated when we create the rules from the rule files through third-party code.")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification = "Users of the object have to be able to add and remove items.")]
        public List<ConditionRuleDefinition> Condition
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
        public SignalRuleDefinition Signal
        {
            get;
            set;
        }
    }
}
