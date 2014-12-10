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
    public abstract class VexiereAspect : ICallHandler, ISetupInterception
    {
        protected abstract bool OnBeforeExecution(IInputArguments arguments, out object returnValue);

        protected abstract MethodSignature GetMatchingMethod();

        protected abstract bool OnAfterExecution(IInputArguments args, object output, out object newOutput);

        // protected abstract void SetupRegistrations(IUnityContainer container);

        public int Order
        {
            get; set;
        }

        IMethodReturn ICallHandler.Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            object returnValue = null;
            var args = new InputArguments(input.Inputs);
            var hasReturnValue = this.OnBeforeExecution(args, out returnValue);
            if( hasReturnValue == true ) return input.CreateMethodReturn(args);
            var methodReturn = getNext().Invoke(input, getNext);
            hasReturnValue = this.OnAfterExecution(args, methodReturn.ReturnValue, out returnValue);
            if (hasReturnValue == true)
                return input.CreateMethodReturn(returnValue);
            else
                return methodReturn;
        }

        

        int ICallHandler.Order
        {
            get
            {
                return this.Order;
            }
            set
            {
                this.Order = value;
            }
        }

        void ISetupInterception.Setup(IUnityContainer container)
        {
            var signature = this.GetMatchingMethod();
            container
                .Configure<Interception>()
                .AddPolicy(Guid.NewGuid().ToString())
                .SetMatchingRule(signature)
                .AddCallHandler(this.GetType(), 
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor());
        }
    }

}
