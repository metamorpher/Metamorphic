//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines constants used with rules.
    /// </summary>
    public static class RuleConstants
    {
        /// <summary>
        /// The default relative path to the directory that contains individual rule files.
        /// </summary>
        public const string DefaultRuleLocation = "rules";

        /// <summary>
        /// The regular expression used to extract trigger parameters from a rule parameter.
        /// </summary>
        /// <remarks>
        /// A rule parameter may look like: <code>"This is a parameter value for: {{signal.TRIGGER_PARAMETER}}."</code>.
        /// The regular expression string is used to extract the <code>TRIGGER_PARAMETER</code> section, which is the
        /// name of the trigger parameter of which the value should be used to replace the <code>{{signal.TRIGGER_PARAMETER}}</code>
        /// section.
        /// </remarks>
        public const string TriggerParameterRegex = @"(?:{{signal.)(.*?)(?:}})";
    }
}
