using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.WebHooks
{
    public interface IInputArguments
    {
        object this[int index] { get; }

        object this[string name] { get; }
    }

    
}
