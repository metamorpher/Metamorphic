//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using Metamorphic.Core.Rules;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Metamorphic.Server.Rules
{
    internal sealed class RuleLoader
    {
        /// <summary>
        /// Creates a new <see cref="Rule"/> object from the information in the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the rule file.</param>
        /// <returns>A newly created rule instance.</returns>
        public Rule Load(string filePath)
        {
            using (var input = new StreamReader(filePath))
            {
                var deserializer = new Deserializer(namingConvention: new PascalCaseNamingConvention());
                //deserializer.RegisterTypeConverter();
                var definition = deserializer.Deserialize<RuleDefinition>(input);

                var rule = definition.ToRule();
                return rule;
            }
        }
    }
}
