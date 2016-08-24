//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Rules;
using Metamorphic.Storage.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Metamorphic.Storage.Rules
{
    internal sealed class RuleLoader : ILoadRules
    {
        // This method is internal only because we want to run unit tests against it.
        internal static RuleDefinition CreateDefinition(TextReader ruleDefinitionReader)
        {
            var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
            deserializer.TypeResolvers.Add(new ScalarYamlNodeTypeResolver());
            var definition = deserializer.Deserialize<RuleDefinition>(ruleDefinitionReader);

            return definition;
        }

        /// <summary>
        /// Returns a value indicating whether the current rule definition is valid or not.
        /// </summary>
        /// <param name="definition">The definition of the rule.</param>
        /// <returns>
        ///   <see langword="true" /> if the current rule applies to the given signal; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        internal static RuleDefinitionCheck IsValid(RuleDefinition definition)
        {
            return new RuleDefinitionCheck(definition);
        }

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleLoader"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that stores the diagnostics methods for the current application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public RuleLoader(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            _diagnostics = diagnostics;
        }

        private RuleDefinition CreateRule(TextReader reader, string invalidRuleDefinitionLogMessage)
        {
            var definition = CreateDefinition(reader);
            if (definition == null)
            {
                return null;
            }

            var check = IsValid(definition);
            if (!check.IsValid)
            {
                _diagnostics.Log(
                    LevelToLog.Warn,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RuleLoader_InvalidRuleDefinition_WithErrors,
                        invalidRuleDefinitionLogMessage,
                        check.Errors().Join(Environment.NewLine)));

                return null;
            }

            return definition;
        }

        /// <summary>
        /// Creates a new <see cref="RuleDefinition"/> object from the information in the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        /// <returns>A newly created rule definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is an empty string.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Thrown if <paramref name="filePath"/> cannot be found.
        /// </exception>
        public RuleDefinition LoadFromFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    Resources.Exceptions_Messages_FileNotFound,
                    filePath);
            }

            var invalidRuleDefinitionLogMessage = string.Format(
                CultureInfo.InvariantCulture,
                Resources.Log_Messages_RuleLoader_InvalidRuleDefinition_WithFilePath,
                filePath);

            using (var input = new StreamReader(filePath))
            {
                return CreateRule(input, invalidRuleDefinitionLogMessage);
            }
        }

        /// <summary>
        /// Creates a new <see cref="RuleDefinition"/> object from the information in the specified string.
        /// </summary>
        /// <param name="ruleDefinition">The full rule definition.</param>
        /// <returns>A newly created rule definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ruleDefinition"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="ruleDefinition"/> is an empty string.
        /// </exception>
        public RuleDefinition LoadFromMemory(string ruleDefinition)
        {
            if (ruleDefinition == null)
            {
                throw new ArgumentNullException("ruleDefinition");
            }

            if (string.IsNullOrWhiteSpace(ruleDefinition))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "ruleDefinition");
            }

            var invalidRuleDefinitionLogMessage = string.Format(
                CultureInfo.InvariantCulture,
                Resources.Log_Messages_RuleLoader_InvalidRuleDefinition_WithDefinitionText,
                ruleDefinition);

            using (var reader = new StringReader(ruleDefinition))
            {
                return CreateRule(reader, invalidRuleDefinitionLogMessage);
            }
        }

        /// <summary>
        /// Creates a new <see cref="RuleDefinition"/> object from the information in the specified string.
        /// </summary>
        /// <param name="stream">The stream that contains the full rule definition.</param>
        /// <returns>A newly created rule definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="stream"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="stream"/> does not allow reading.
        /// </exception>
        public RuleDefinition LoadFromStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_StreamShouldBeReadable,
                    "stream");
            }

            var invalidRuleDefinitionLogMessage = Resources.Log_Messages_RuleLoader_InvalidRuleDefinition_FromStream;
            using (var reader = new StreamReader(stream))
            {
                return CreateRule(reader, invalidRuleDefinitionLogMessage);
            }
        }
    }
}
