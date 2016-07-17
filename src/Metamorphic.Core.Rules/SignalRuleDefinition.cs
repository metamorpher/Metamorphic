//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Stores information about a trigger in a rule.
    /// </summary>
    [Serializable]
    public sealed class SignalRuleDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRuleDefinition"/> class.
        /// </summary>
        public SignalRuleDefinition()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the collection of parameters for the trigger.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This class is being used when we load rules from the rule files through third-party code.")]
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID of trigger.
        /// </summary>
        public string Id
        {
            get;
            set;
        }
    }
}
