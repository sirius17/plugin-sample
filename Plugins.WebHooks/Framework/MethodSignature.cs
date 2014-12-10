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
    public class MethodSignature
    {
        public static readonly MethodSignature All = new MethodSignature { Name = "*" };

        public MethodSignature()
        {
            this.Parameters = new List<Type>();
        }

        public Type TargetType { get; set; }

        public string Name { get; set; }

        public List<Type> Parameters { get; private set; }

        internal PolicyDefinition ConfigureType(PolicyDefinition definition)
        {
            return definition
                    .AddMatchingRule<TypeMatchingRule>(new InjectionConstructor( new InjectionParameter(this.TargetType)));
        }

        internal PolicyDefinition ConfigureMethodSignature(PolicyDefinition definition )
        {
            var rule = new MethodSignatureMatchingRule(this.Name, 
                this.Parameters.Select(x => x.Namespace + "." + x.Name) , 
                true);
            definition.AddMatchingRule(rule);
            // return definition.AddMatchingRule<MethodSignatureMatchingRule>(new InjectionConstructor(this.Name, this.Parameters.Select(t => t.AssemblyQualifiedName), true));
            //new MethodSignatureMatchingRule("", null, true);
            return definition;
        }
    }

}
