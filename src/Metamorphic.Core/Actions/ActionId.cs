//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Defines an ID for an action.
    /// </summary>
    [Serializable]
    public sealed class ActionId : Id<ActionId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionId"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ActionId(string value) 
            : base(value)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override ActionId Clone(string value)
        {
            return new ActionId(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}
