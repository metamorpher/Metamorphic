//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores information about a condition under which a given signal applies to a rule.
    /// </summary>
    public class ConditionRuleDefinition
    {
        /// <summary>
        /// Gets or sets the name of the trigger parameter.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pattern to compare with.
        /// </summary>
        public string Pattern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comparison method.
        /// </summary>
        public string Type
        {
            get;
            set;
        }
    }
}