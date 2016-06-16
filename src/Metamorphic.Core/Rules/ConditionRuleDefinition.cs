//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
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
        public object Pattern
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
