//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metamorphic.Core.Jobs;
using Metamorphic.Core.Signals;

namespace Metamorphic.Core.Rules
{
    public sealed class Rule
    {
        public Job ToJob(Signal signal)
        {
            throw new NotImplementedException();
        }
    }
}
