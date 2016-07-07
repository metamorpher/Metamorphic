//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace Metamorphic.Core
{
    /// <summary>
    /// Stores information about the application.
    /// </summary>
    [Serializable]
    public sealed class ApplicationInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInformation"/> class.
        /// </summary>
        public ApplicationInformation()
        {
            ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public Version ApplicationVersion
        {
            get;
        }
    }
}
