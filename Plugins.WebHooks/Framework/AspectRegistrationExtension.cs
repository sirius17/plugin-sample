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
    public class AspectRegistrationExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            AspectConfiguration.Aspects.ForEach(a => a.Setup(this.Container));
        }
    }
}
