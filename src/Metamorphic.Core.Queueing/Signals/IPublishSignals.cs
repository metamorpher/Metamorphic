//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Signals;

namespace Metamorphic.Core.Queueing.Signals
{
    /// <summary>
    /// Defines the interface for objects that publish signals.
    /// </summary>
    public interface IPublishSignals : IPublishItems<Signal>
    {
    }
}
