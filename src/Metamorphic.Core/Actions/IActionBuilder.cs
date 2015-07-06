using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metamorphic.Core.Actions
{
    public interface IActionBuilder
    {
        ActionDefinition ToDefinition();
    }
}
