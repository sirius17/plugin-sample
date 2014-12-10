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
    internal class InputArguments : IInputArguments
    {
        public InputArguments(IParameterCollection parameters)
        {
            this.Parameters = parameters;
        }

        public IParameterCollection Parameters { get; set; }

        public object this[int index]
        {
            get { return this.Parameters[index]; }
            set { this.Parameters[index] = value; }
        }

        public object this[string name]
        {
            get { return this.Parameters[name]; }
            set { this.Parameters[name] = value; }
        }
    }

}
