using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.WebHooks
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            UnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.AddNewExtension<AspectRegistrationExtension>();
            container.RegisterType<IDirectory, NopDirectory>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            var action = container.Resolve<IDirectory>();
            action.CreateUser(new User { Id = "123", UserName = "john.doe", Name = new UserName { FirstName = "John", LastName = "Doe" } });
            
            Console.WriteLine("Press any key to end..");
            Console.ReadKey(true);
        }
    }

    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public UserName Name { get; set; }
    }

    public class UserName
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public interface IDirectory
    {
        User CreateUser(User user);

        User GetUser(string userId);
    }

    public class NopDirectory : IDirectory
    {
        public User CreateUser(User user)
        {
            // Created user
            Console.WriteLine("Created the user.");
            return user;
        }


        public User GetUser(string userId)
        {
            throw new NotImplementedException();
        }
    }

    public static class Configuration
    {
        public static string PostUrl
        {
            get { return ConfigurationManager.AppSettings["post-url"] ?? "http://requestb.in/ptyy7ipt"; }
        }
    }

    public class PrinterAspect : VexiereAspect
    {
        protected override bool OnBeforeExecution(IInputArguments arguments, out object returnValue)
        {
            returnValue  = null;
            Console.WriteLine("Before aspect.");
            // Console.WriteLine("Argument is a {0}.", arguments[0].GetType().Name);
            return false;
        }

        protected override bool OnAfterExecution(IInputArguments args, object output, out object newOutput)
        {
            newOutput = null;
            Console.WriteLine("After aspect.");
            return false;
        }

        protected override MethodSignature GetMatchingMethod()
        {
            var method = new MethodSignature
            {
                TargetType = typeof(IDirectory),
                Name = "Create"
            };
            method.Parameters.Add(typeof(User));
            return method;
        }
    }


}
