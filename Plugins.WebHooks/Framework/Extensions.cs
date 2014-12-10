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
    public static class Extensions
    {
        public static PolicyDefinition SetMatchingRule(this PolicyDefinition definition, MethodSignature signature)
        {
            // definition.AddMatchingRule<AlwaysMatch>();
            signature.ConfigureMethodSignature(definition);
            // signature.ConfigureType(definition);
            return definition;
        }
    }

}
