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
    public class AspectConfiguration
    {
        public static List<ISetupInterception> Aspects
        {
            get
            {
                return new List<ISetupInterception>()
                {
                    // new PrinterAspect(),
                    new WebHookFilter()
                };
            }
        }
    }

}
