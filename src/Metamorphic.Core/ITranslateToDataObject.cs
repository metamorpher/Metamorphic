//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Metamorphic.Core
{
    /// <summary>
    /// Defines the interface for objects that can use their own data to create a data object.
    /// </summary>
    /// <typeparam name="T">The type of data object.</typeparam>
    public interface ITranslateToDataObject<T>
    {
        /// <summary>
        /// Creates a new data object with the data from the current object.
        /// </summary>
        /// <returns>A data object that represents the data on the current object.</returns>
        T ToDataObject();
    }
}
