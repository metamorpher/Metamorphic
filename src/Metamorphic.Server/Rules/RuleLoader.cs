//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using Metamorphic.Core.Rules;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Metamorphic.Server.Rules
{
    internal sealed class RuleLoader
    {
        private static readonly Regex s_TriggerParameterMatcher = new Regex(@"(?:{{trigger.)(.*?)(?:}})", RegexOptions.IgnoreCase);

        // private readonly Predicate<>

        public RuleLoader()
        {
        }

        internal RuleDefinition CreateDefinitionFromFile(string filePath)
        {
            using (var input = new StreamReader(filePath))
            {
                var deserializer = new Deserializer(namingConvention: new PascalCaseNamingConvention());
                var definition = deserializer.Deserialize<RuleDefinition>(input);

                return definition;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the current rule definition is valid or not.
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        internal bool IsValid(
            RuleDefinition definition,
            Predicate<string> doesActionIdExist,
            Predicate<string> doesTriggerTypeExist)
        {
            if (string.IsNullOrEmpty(definition.Name))
            {
                return false;
            }

            if (definition.Action == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.Action.Id) || !doesActionIdExist(definition.Action.Id))
            {
                return false;
            }

            if (definition.Action.Parameters != null)
            {
                foreach (var pair in definition.Action.Parameters)
                {
                    if (string.IsNullOrEmpty(pair.Value))
                    {
                        return false;
                    }

                    var match = s_TriggerParameterMatcher.Match(pair.Value);
                    if (match.Success)
                    {
                        if ((definition.Trigger.Parameters == null) || (!definition.Trigger.Parameters.ContainsKey(match.Value)))
                        {
                            return false;
                        }
                    }
                }
            }

            if (definition.Trigger == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(definition.Trigger.Type) || !doesTriggerTypeExist(definition.Trigger.Type))
            {
                return false;
            }

            if (definition.Trigger.Parameters != null)
            {
                foreach (var pair in definition.Trigger.Parameters)
                {
                    if (string.IsNullOrEmpty(pair.Value))
                    {
                        return false;
                    }
                }
            }

            foreach (var criteria in definition.Criteria)
            {
                if ((definition.Trigger.Parameters == null) || (!definition.Trigger.Parameters.ContainsKey(criteria.Name)))
                {
                    return false;
                }

                // criteria type exists
            }

            return true;
        }

        /// <summary>
        /// Creates a new <see cref="Rule"/> object from the information in the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        /// <returns>A newly created rule instance.</returns>
        public Rule Load(string filePath)
        {
            var definition = CreateDefinitionFromFile(filePath);
            if (!IsValid(definition))
            {

            }

            if (definition.Enabled)
            {
                // Action
                // Trigger
                // Conditions

                return new Rule(
                    definition.Name,
                    definition.Description,
                    );
            }

            return null;
        }
    }
}
