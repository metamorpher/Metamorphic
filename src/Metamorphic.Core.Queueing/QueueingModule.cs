using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Metamorphic.Core.Queueing
{
    public sealed class QueueingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterStuff();
        }
    }
}
