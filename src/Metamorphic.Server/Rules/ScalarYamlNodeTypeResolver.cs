//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Metamorphic.Server.Rules
{
    /// <summary>
    /// A type resolver for use with YAML nodes.
    /// </summary>
    /// <remarks>
    /// This class is required because YamlDotNet doesn't handle implicit types in a YAML file. It assumes that
    /// everything is a string, which is not technically correct. Essentially we want the deserializer to follow
    /// the JSON schema, so we implement that ourselves.
    /// </remarks>
    internal sealed class ScalarYamlNodeTypeResolver : INodeTypeResolver
    {
        bool INodeTypeResolver.Resolve(NodeEvent nodeEvent, ref Type currentType)
        {
            if (currentType == typeof(object))
            {
                var scalar = nodeEvent as Scalar;
                if ((scalar != null) && (scalar.Style == ScalarStyle.Plain))
                {
                    // Expressions taken from https://github.com/aaubry/YamlDotNet/blob/feat-schemas/YamlDotNet/Core/Schemas/JsonSchema.cs

                    if (Regex.IsMatch(scalar.Value, @"^(true|false)$", RegexOptions.IgnorePatternWhitespace))
                    {
                        currentType = typeof(bool);
                        return true;
                    }

                    if (Regex.IsMatch(scalar.Value, @"^-?(0|[1-9][0-9]*)$", RegexOptions.IgnorePatternWhitespace))
                    {
                        currentType = typeof(int);
                        return true;
                    }

                    if (Regex.IsMatch(scalar.Value, @"^-?(0|[1-9][0-9]*)(\.[0-9]*)?([eE][-+]?[0-9]+)?$", RegexOptions.IgnorePatternWhitespace))
                    {
                        currentType = typeof(double);
                        return true;
                    }

                    // Add more cases here if needed
                }
            }

            return false;
        }
    }
}
