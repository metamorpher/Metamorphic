//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        public ActionDefinition Action
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the criteria that should applied to the trigger.
        /// </summary>
        public List<CriteriaDefinition> Criteria
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
        /// Returns a value indicating whether the current rule definition is valid or not.
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        internal bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (Action == null)
            {
                return false;
            }

            // Action.Id ==> exists?

            if (Action.Parameters != null)
            {
                foreach (var actionParam in Action.Parameters)
                {
                    // Parameter must be complete
                    // If action has trigger parameter reference it should exist
                }
            }

            if (Trigger == null)
            {
                return false;
            }

            // Trigger.Type exists

            // Trigger.Parameters? -->> Must be complete

            foreach (var criteria in Criteria)
            {
                // criteria.Name matches existing trigger parameter
                // criteria.Type exists
                // 
            }

            return true;
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
        /// 
        /// </summary>
        /// <returns></returns>
        public Rule ToRule()
        {
            if (!IsValid())
            {
                throw new InvalidRuleDefinitionException();
            }

            return new Rule(this);
        }

        /// <summary>
        /// Gets or sets the signal definition for the rule.
        /// </summary>
        public TriggerDefinition Trigger
        {
            get;
            set;
        }
    }
}