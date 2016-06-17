//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores the information about an action reference and the parameter necessary for the given action.
    /// </summary>
    /// <remarks>
    /// The naming of the members of this class is linked to the contents of the rule files.
    /// </remarks>
    public class ActionRuleDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRuleDefinition"/> class.
        /// </summary>
        public ActionRuleDefinition()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the ID of the action that should be executed.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameters for the action.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This class is instantiated when we load the rules from the rule files through third-party code.")]
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }
}
