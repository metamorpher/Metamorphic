//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using YamlDotNet.Serialization;

namespace Metamorphic.Server.Rules
{
    /// <summary>
    /// Stores information about a condition under which a given signal applies to a rule.
    /// </summary>
    public class CriteriaDefinition
    {
        /// <summary>
        /// Gets or sets the name of the trigger parameter.
        /// </summary>
        [YamlMember(Alias = "name")]
        public string TriggerParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comparison method.
        /// </summary>
        [YamlMember(Alias = "type")]
        public string ComparisonMethod
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
    }
}