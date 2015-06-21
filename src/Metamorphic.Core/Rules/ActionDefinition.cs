//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Metamorphic.Server.Rules
{
    /// <summary>
    /// Stores the information about an action reference and the parameter necessary for the given action.
    /// </summary>
    public class ActionDefinition
    {
        /// <summary>
        /// Gets or sets the ID of the action that should be executed.
        /// </summary>
        [YamlMember(Alias = "ref")]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameters for the action.
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get;
            set;
        }
    }
}